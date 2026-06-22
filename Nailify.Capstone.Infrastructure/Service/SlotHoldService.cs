using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class SlotHoldService : ISlotHoldService
    {
        private readonly IDistributedCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISlotHoldConfiguration _config;
        private readonly ILogger<SlotHoldService> _logger;

        public SlotHoldService(
               IDistributedCache cache,
               IUnitOfWork unitOfWork,
               ISlotHoldConfiguration config,
               ILogger<SlotHoldService> logger)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _config = config;
            _logger = logger;
        }

        public async Task ConsumeHoldAsync(string holdToken)
        {
            var tokenKey = $"{_config.KeyPrefix}:token:{holdToken}";
            var mappingJson = await _cache.GetStringAsync(tokenKey);
            if (!string.IsNullOrEmpty(mappingJson))
            {
                var mapping = JsonSerializer.Deserialize<TokenMapping>(mappingJson);
                if (mapping != null)
                    await _cache.RemoveAsync(mapping.SlotKey);
                await _cache.RemoveAsync(tokenKey);
            }
        }

        public async Task<ApiResult<SlotHoldResponseDTO>> GetHoldStatusAsync(string holdToken)
        {
            var tokenKey = $"{_config.KeyPrefix}:token:{holdToken}";
            var mappingJson = await _cache.GetStringAsync(tokenKey);

            if (string.IsNullOrEmpty(mappingJson))
            {
                var x = new SlotHoldResponseDTO
                {
                    HoldToken = holdToken,
                    RemainingSeconds = 0,
                    IsHeld = false
                };

                return new ApiSuccessResult<SlotHoldResponseDTO>(x, "Mã giữ chỗ không tồn tại hoặc đã hết hạn");
            }
            var mapping = JsonSerializer.Deserialize<TokenMapping>(mappingJson);
            var holdJson = await _cache.GetStringAsync(mapping!.SlotKey);
            if (string.IsNullOrEmpty(holdJson))
            {
                var y = new SlotHoldResponseDTO
                {
                    HoldToken = holdToken,
                    RemainingSeconds = 0,
                    IsHeld = false
                };
                return new ApiSuccessResult<SlotHoldResponseDTO>(y, "Mã giữ chỗ không tồn tại hoặc đã hết hạn.");
            }
            var hold = JsonSerializer.Deserialize<SlotHoldData>(holdJson);
            var remaining = (int)(hold.ExpiresAt - DateTime.UtcNow).TotalSeconds;
            var response = new SlotHoldResponseDTO
            {
                HoldToken = holdToken,
                ExpiresAt = hold.ExpiresAt,
                RemainingSeconds = Math.Max(remaining, 0),
                IsHeld = remaining > 0
            };
            return new ApiSuccessResult<SlotHoldResponseDTO>(response, "Lấy thông tin giữ chỗ thành công");
        }

        private async Task<SlotHoldData?> GetConflictingHoldAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid customerId)
        {
            var maxDuration = TimeSpan.FromHours(4);
            var checkStart = startTime - maxDuration;
            if (checkStart < TimeSpan.Zero)
            {
                checkStart = TimeSpan.Zero;
            }

            var startMinutes = (int)checkStart.TotalMinutes;
            if (startMinutes % 30 != 0)
            {
                startMinutes = (startMinutes / 30) * 30;
            }
            checkStart = TimeSpan.FromMinutes(startMinutes);

            var stepTimes = new List<TimeSpan>();
            var current = checkStart;
            while (current < endTime)
            {
                stepTimes.Add(current);
                current = current.Add(TimeSpan.FromMinutes(30));
            }
            // Gửi yêu cầu tìm kiếm song song lên Redis để tối ưu hiệu năng
            var fetchTasks = stepTimes.Select(t => _cache.GetStringAsync(BuildSlotKey(artistId, date, t))).ToList();
            var jsonResults = await Task.WhenAll(fetchTasks);

            foreach (var json in jsonResults)
            {
                if (string.IsNullOrEmpty(json))
                {
                    continue;
                }

                var hold = JsonSerializer.Deserialize<SlotHoldData>(json);
                if (hold == null)
                {
                    continue;
                }

                var holdEndTime = hold.StartTime.Add(TimeSpan.FromMinutes(hold.EstimatedDurationMinutes));
                if (startTime < holdEndTime && endTime > hold.StartTime)
                {
                    // Tìm thấy giữ chỗ bị đè của khách hàng KHÁC
                    if (hold.CustomerId != customerId)
                    {
                        return hold;
                    }
                }
            }

            return null;
        }

        public async Task<ApiResult<SlotHoldResponseDTO>> HoldSlotAsync(Guid customerId, HoldSlotRequestDTO request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<SlotHoldResponseDTO>("Danh sách dịch vụ/mẫu nail giữ chỗ không được trống.");
            }

            var durationMinutes = 0;
            foreach (var item in request.BookingItems)
            {
                var itemDuration = 0;
                if (item.NailVariantId.HasValue)
                {
                    var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(item.NailVariantId.Value);
                    if (variant != null)
                    {
                        itemDuration += (variant.Duration ?? 60);
                    }
                }

                if (item.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                    if (service != null)
                    {
                        itemDuration += service.Duration;
                    }
                }

                if (item.CustomerNailId.HasValue)
                {
                    var customNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(item.CustomerNailId.Value);
                    if (customNail != null)
                    {
                        itemDuration += (customNail.Duration ?? 60);
                    }
                }

                durationMinutes += itemDuration * item.Quantity;
            }

            if (durationMinutes == 0)
            {
                durationMinutes = 30; // Mặc định nếu tổng thời gian bằng 0
            }

            var endTime = request.StartTime.Add(TimeSpan.FromMinutes(durationMinutes));

            // Check DB conflict
            var conflict = await _unitOfWork.BookingRepository.HasBookingConflictAsync(request.NailArtistId, request.BookingDate, request.StartTime, endTime);
            if (conflict)
            {
                return new ApiErrorResult<SlotHoldResponseDTO>("Thợ đã có lịch hẹn trong khung giờ này");
            }

            // Kiểm tra xem có khách hàng nào khác đang giữ slot đè lên khung giờ này không
            var conflictingHold = await GetConflictingHoldAsync(request.NailArtistId, request.BookingDate, request.StartTime, endTime, customerId);
            if (conflictingHold != null)
            {
                var remaining = (int)(conflictingHold.ExpiresAt - DateTime.UtcNow).TotalSeconds;
                return new ApiErrorResult<SlotHoldResponseDTO>($"Slot đang được giữ bởi khách hàng khác. Vui lòng thử lại sau {Math.Max(remaining, 0)} giây.");
            }

            // Check if the current customer already holds this exact slot to renew/extend it
            var slotKey = BuildSlotKey(request.NailArtistId, request.BookingDate, request.StartTime);
            _logger.LogInformation("HoldSlotAsync: Built slotKey = {slotKey} for artistId = {artistId}, date = {date:yyyy-MM-dd}, startTime = {startTime}", slotKey, request.NailArtistId, request.BookingDate, request.StartTime);
            var existingJson = await _cache.GetStringAsync(slotKey);

            if(!string.IsNullOrEmpty(existingJson))
            {
                var existing = JsonSerializer.Deserialize<SlotHoldData>(existingJson);
                if(existing != null && existing.CustomerId == customerId)
                {
                    // Khách hàng đang giữ chỗ, gia hạn thời gian giữ chỗ
                    var refreshExpiry = DateTime.UtcNow.AddSeconds(_config.HoldDurationSeconds);
                    existing.ExpiresAt = refreshExpiry;
                    existing.EstimatedDurationMinutes = durationMinutes;
                    await SetCacheAsync(slotKey, existing);
                    _logger.LogInformation("HoldSlotAsync: Renewed hold for key = {slotKey}, expiresAt = {expiresAt}", slotKey, refreshExpiry);

                    var x = new SlotHoldResponseDTO
                    {
                        HoldToken = existing.HoldToken,
                        ExpiresAt = refreshExpiry,
                        RemainingSeconds = _config.HoldDurationSeconds,
                        IsHeld = true
                    };
                    return new ApiSuccessResult<SlotHoldResponseDTO>(x, "Đã gia hạn giữ chỗ thành công");
                }
            }
            // Nếu không có giữ chỗ nào, tạo giữ chỗ mới
            var holdToken = Guid.NewGuid().ToString("N");
            var expiresAt = DateTime.UtcNow.AddSeconds(_config.HoldDurationSeconds);
            var holdData = new SlotHoldData
            {
                CustomerId = customerId,
                HoldToken = holdToken,
                ArtistId = request.NailArtistId,
                SalonId = request.SalonId,
                BookingDate = request.BookingDate,
                StartTime = request.StartTime,
                EstimatedDurationMinutes = durationMinutes,
                ExpiresAt = expiresAt
            };
           
            await SetCacheAsync(slotKey, holdData);
            _logger.LogInformation("HoldSlotAsync: Created hold for key = {slotKey}, holdToken = {holdToken}, expiresAt = {expiresAt}", slotKey, holdToken, expiresAt);
            // Tạo mapping từ holdToken ngược lại slotKey
            await SetCacheAsync($"{_config.KeyPrefix}:token:{holdToken}", new TokenMapping { SlotKey = slotKey }, _config.HoldDurationSeconds);

            var response = new SlotHoldResponseDTO
            {
                HoldToken = holdToken,
                ExpiresAt = expiresAt,
                RemainingSeconds = _config.HoldDurationSeconds,
                IsHeld = true
            };

            return new ApiSuccessResult<SlotHoldResponseDTO>(response, "Giữ chỗ thành công");
        }

        public async Task<bool> IsSlotHeldAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            _logger.LogInformation("IsSlotHeldAsync: Checking slot for artistId = {artistId}, date = {date:yyyy-MM-dd}, startTime = {startTime}, endTime = {endTime}", artistId, date, startTime, endTime);
            var maxDuration = TimeSpan.FromHours(4);
            var checkStart = startTime - maxDuration;
            if (checkStart < TimeSpan.Zero)
            {
                checkStart = TimeSpan.Zero;
            }

            var startMinutes = (int)checkStart.TotalMinutes;
            if (startMinutes % 30 != 0)
            {
                startMinutes = (startMinutes / 30) * 30;
            }
            checkStart = TimeSpan.FromMinutes(startMinutes);

            var stepTimes = new List<TimeSpan>();
            var current = checkStart;
            while (current < endTime)
            {
                stepTimes.Add(current);
                current = current.Add(TimeSpan.FromMinutes(30));
            }

            var stepKeys = stepTimes.Select(t => BuildSlotKey(artistId, date, t)).ToList();
            _logger.LogInformation("IsSlotHeldAsync: Generated {count} step keys to check: {keys}", stepKeys.Count, string.Join(", ", stepKeys));
            
            var fetchTasks = stepKeys.Select(k => _cache.GetStringAsync(k)).ToList();
            var jsonResults = await Task.WhenAll(fetchTasks);

            for (int i = 0; i < stepKeys.Count; i++)
            {
                var key = stepKeys[i];
                var json = jsonResults[i];

                if (string.IsNullOrEmpty(json))
                {
                    _logger.LogInformation("IsSlotHeldAsync: Key = {key} is empty in cache", key);
                    continue;
                }

                _logger.LogInformation("IsSlotHeldAsync: Key = {key} found in cache with value: {json}", key, json);
                var hold = JsonSerializer.Deserialize<SlotHoldData>(json);
                if (hold == null)
                {
                    continue;
                }

                var holdEndTime = hold.StartTime.Add(TimeSpan.FromMinutes(hold.EstimatedDurationMinutes));
                if (startTime < holdEndTime && endTime > hold.StartTime)
                {
                    _logger.LogInformation("IsSlotHeldAsync: Slot is HELD! Conflict found with hold starting at {start} (duration {duration} mins) ending at {end}", hold.StartTime, hold.EstimatedDurationMinutes, holdEndTime);
                    return true;
                }
            }

            _logger.LogInformation("IsSlotHeldAsync: Slot is NOT held. No conflict found.");
            return false;
        }

        public async Task<ApiResult<bool>> ReleaseSlotAsync(Guid customerId, string holdToken)
        {
            var tokenKey = $"{_config.KeyPrefix}:token:{holdToken}";
            var mappingJson = await _cache.GetStringAsync(tokenKey);

            if(string.IsNullOrEmpty(mappingJson))
            {
                return new ApiErrorResult<bool>("Mã giữ chỗ không tồn tại hoặc đã hết hạn");
            }

            var mapping = JsonSerializer.Deserialize<TokenMapping>(mappingJson);
            if(mapping == null)
            {
                return new ApiErrorResult<bool>("Mã giữ chỗ không hợp lệ");
            }
            var holdJson = await _cache.GetStringAsync(mapping.SlotKey);
            if (!string.IsNullOrEmpty(holdJson))
            {
                var hold = JsonSerializer.Deserialize<SlotHoldData>(holdJson);
                if (hold != null && hold.CustomerId != customerId)
                {
                    return new ApiErrorResult<bool>("Bạn không có quyền giải phóng giữ chỗ này");
                }
                await _cache.RemoveAsync(mapping.SlotKey);
            }
            await _cache.RemoveAsync(tokenKey);
            return new ApiResult<bool>(true, "Đã hủy giữ chỗ thành công.");
        }

        public async Task<bool> ValidateHoldTokenAsync(string holdToken, Guid customerId, Guid artistId, DateTime date, TimeSpan startTime)
        {
            var tokenKey = $"{_config.KeyPrefix}:token:{holdToken}";
            var mappingJson = await _cache.GetStringAsync(tokenKey);
            if(string.IsNullOrEmpty(mappingJson))
            {
               return false;
            }
            var mapping = JsonSerializer.Deserialize<TokenMapping>(mappingJson);
            if(mapping == null)
            {
                return false;
            }

            var holdJson = await _cache.GetStringAsync(mapping.SlotKey);
            if (string.IsNullOrEmpty(holdJson))
            {
                return false;
            }

            var response = JsonSerializer.Deserialize<SlotHoldData>(holdJson);
            return response != null 
                && response.CustomerId == customerId 
                && response.HoldToken == holdToken
                && response.ArtistId == artistId
                && response.BookingDate.Date == date.Date
                && response.StartTime == startTime;
        }
        /// <summary>
        /// Tạo chuỗi Key duy nhất để định danh slot trên Redis.
        /// Định dạng: slot_hold:{artistId}:{yyyyMMdd}:{hhmm}
        /// </summary>
        private string BuildSlotKey(Guid artistId, DateTime date, TimeSpan startTime)
            => $"{_config.KeyPrefix}:{artistId}:{date:yyyyMMdd}:{startTime:hhmm}";
        /// <summary>
        /// Chuyển đổi Object sang JSON và lưu vào Redis kèm thời gian hết hạn tự động (TTL).
        /// </summary>
        /// <param name="key">Khóa định danh trong Redis</param>
        /// <param name="data">Dữ liệu cần lưu</param>
        /// <param name="ttlSeconds">Thời gian tự hủy (giây). Nếu null sẽ lấy mặc định là 5 phút.</param>
        private async Task SetCacheAsync<T>(string key, T data, int? ttlSeconds = null)
        {
            var json = JsonSerializer.Serialize(data);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds ?? _config.HoldDurationSeconds)
            };

            await _cache.SetStringAsync(key, json, options);
        }
        /// <summary>
        /// Cấu trúc dữ liệu chi tiết của một slot giữ chỗ (dùng để lưu xuống Redis dưới dạng JSON).
        /// </summary>
        private class SlotHoldData
        {
            public Guid CustomerId { get; set; }
            public string HoldToken { get; set; } = string.Empty;
            public Guid ArtistId { get; set; }
            public Guid SalonId { get; set; }
            public DateTime BookingDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public int EstimatedDurationMinutes { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
        /// <summary>
        /// Bản đồ ánh xạ (Mapping) từ holdToken ngược lại SlotKey.
        /// Giúp tìm kiếm nhanh thông tin slot trên Redis khi Frontend chỉ gửi lên holdToken.
        /// </summary>
        private class TokenMapping
        {
            public string SlotKey { get; set; } = string.Empty;
        }
    }
}
