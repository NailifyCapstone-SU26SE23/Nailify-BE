using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Org.BouncyCastle.Asn1.Ocsp;
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
            if (string.IsNullOrEmpty(mappingJson)) 
            {
                return;
            }
            var mapping = JsonSerializer.Deserialize<TokenMapping>(mappingJson);
            if(mapping == null)
            {
                return;
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Lock
                await _unitOfWork.NailArtistRepository.GetArtistWithLockAsync(mapping.ArtistId);

                var redisListKey = BuildSlotKey(mapping.ArtistId, mapping.BookingDate);
                var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);

                var holdToRemove = activeHolds.FirstOrDefault(x => x.HoldToken == holdToken);
                if(holdToRemove != null)
                {
                    activeHolds.Remove(holdToRemove);
                    await SaveHoldToRedisAsync(redisListKey, activeHolds);
                }
                await _cache.RemoveAsync(tokenKey);
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
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
            var redisListKey = BuildSlotKey(mapping!.ArtistId, mapping.BookingDate);
            var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);
            var hold = activeHolds.FirstOrDefault(x => x.HoldToken == holdToken);
            if(hold == null)
            {
                var x = new SlotHoldResponseDTO
                {
                    HoldToken = holdToken,
                    RemainingSeconds = 0,
                    IsHeld = false
                };
                return new ApiSuccessResult<SlotHoldResponseDTO>(x, "Mã giữ chỗ không tồn tại hoặc đã hết hạn.");
            }
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
        public async Task<ApiResult<SlotHoldResponseDTO>> HoldSlotAsync(Guid customerId, HoldSlotRequestDTO request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<SlotHoldResponseDTO>("Danh sách dịch vụ/mẫu nail giữ chỗ không được trống.");
            }

            int durationMinutes = await CalculateTotalDuratonAysnc(request.BookingItems, request.SalonId);

            var endTime = request.StartTime.Add(TimeSpan.FromMinutes(durationMinutes));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var artist = await _unitOfWork.NailArtistRepository.GetArtistWithLockAsync(request.NailArtistId);
                if(artist == null || artist.Status != "Active")
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<SlotHoldResponseDTO>("Thợ nail không hoạt động hoặc không tồn tại.");
                }
                var isConflict = await CheckCapacityConflictInternalAsync(
                   artist,
                   request.BookingDate,
                   request.StartTime,
                   endTime,
                   customerId,
                   excludingHoldToken: null);
                if (isConflict)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<SlotHoldResponseDTO>("Thợ đã đầy lịch trong khoảng thời gian này, vui lòng chọn giờ khác.");
                }
                // Đọc danh sách Holds hiện tại từ Redis
                var redisListKey = BuildSlotKey(request.NailArtistId, request.BookingDate);
                var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);
                var existingHold = activeHolds.FirstOrDefault(x => x.CustomerId == customerId && x.StartTime == request.StartTime);
                string holdToken;
                var expiresAt = DateTime.UtcNow.AddSeconds(_config.HoldDurationSeconds);
                if (existingHold != null)
                {
                    holdToken = existingHold.HoldToken;
                    existingHold.ExpiresAt = expiresAt;
                    existingHold.EstimatedDurationMinutes = durationMinutes;
                }
                else
                {
                    holdToken = Guid.NewGuid().ToString("N");
                    var x = new SlotHoldData
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
                    activeHolds.Add(x);
                }
                // Lưu lại Redis
                await SaveHoldToRedisAsync(redisListKey, activeHolds);

                // Lưu token mapping
                var tokenKey = $"{_config.KeyPrefix}:token:{holdToken}";
                var mapping = new TokenMapping { ArtistId = request.NailArtistId, BookingDate = request.BookingDate, SlotKey = holdToken };
                await SetCacheAsync(tokenKey, mapping, _config.HoldDurationSeconds);
                await _unitOfWork.CommitTransactionAsync();
                var response = new SlotHoldResponseDTO
                {
                    HoldToken = holdToken,
                    ExpiresAt = expiresAt,
                    RemainingSeconds = _config.HoldDurationSeconds,
                    IsHeld = true
                };
                return new ApiSuccessResult<SlotHoldResponseDTO>(response, "Giữ chỗ thành công.");
            }
            catch (Exception ex) 
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình HoldSlotAsync");
                return new ApiErrorResult<SlotHoldResponseDTO>("Có lỗi hệ thống xảy ra khi giữ chỗ.");
            }
        }

        public async Task<bool> IsSlotHeldAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var redisListKey = BuildSlotKey(artistId, date);
            var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);
            return activeHolds.Any(x => x.StartTime < endTime
                                   && x.StartTime.Add(TimeSpan.FromMinutes(x.EstimatedDurationMinutes)) > startTime);
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
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.NailArtistRepository.GetArtistWithLockAsync(mapping.ArtistId);

                var redisListKey = BuildSlotKey(mapping.ArtistId, mapping.BookingDate);
                var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);

                var holdToRemove = activeHolds.FirstOrDefault(x => x.HoldToken == holdToken);
                if(holdToRemove != null)
                {
                    if(holdToRemove.CustomerId != customerId)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return new ApiErrorResult<bool>("Bạn không có quyền giải phóng giữ chỗ này.");
                    }
                    activeHolds.Remove(holdToRemove);
                    await SaveHoldToRedisAsync(redisListKey, activeHolds);
                }

                await _cache.RemoveAsync(tokenKey);
                await _unitOfWork.CommitTransactionAsync();
                return new ApiResult<bool>(true, "Đã hủy giữ chỗ thành công.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
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
            var redistListKey = BuildSlotKey(mapping.ArtistId, mapping.BookingDate);
            var activeHolds = await GetActiveHoldsFromRedisAsync(redistListKey);

            var response = activeHolds.FirstOrDefault(x => x.HoldToken == holdToken);
            return response != null 
                && response.CustomerId == customerId 
                && response.ArtistId == artistId
                && response.BookingDate.Date == date.Date
                && response.StartTime == startTime;
        }
        /// <summary>
        /// Tạo chuỗi Key duy nhất để định danh slot trên Redis.
        /// Định dạng: slot_hold:{artistId}:{yyyyMMdd}:{hhmm}
        /// </summary>
        private string BuildSlotKey(Guid artistId, DateTime date)
            => $"{_config.KeyPrefix}:list:{artistId}:{date:yyyyMMdd}";
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
        private async Task<int> CalculateTotalDuratonAysnc(IEnumerable<BookingItemRequestDTO> items, Guid salonId)
        {
            var durationMinutes = 0;
            foreach (var x in items)
            {
                var itemDuration = 0;
                if (x.NailVariantId.HasValue)
                {
                    var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(x.NailVariantId.Value);
                    if (variant != null)
                    {
                        itemDuration += (variant.Duration ?? 60);
                    }
                }

                if (x.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(x.ServiceId.Value);
                    if (service != null)
                    {
                        itemDuration += service.Duration;
                    }
                }

                if (x.CustomerNailId.HasValue)
                {
                   var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetApprovedRequestAsync(x.CustomerNailId.Value, salonId);
                   if(customNailRequest != null && customNailRequest.Duration.HasValue)
                    {
                        itemDuration += customNailRequest.Duration.Value;
                    }
                    else
                    {
                        var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(x.CustomerNailId.Value);
                        if(customerNail != null)
                        {
                            itemDuration += (customerNail.Duration ?? 60);
                        }
                    }
                }

                durationMinutes += itemDuration * x.Quantity;
            }
            return durationMinutes == 0 ? 30 : durationMinutes;
        }
        private async Task SaveHoldToRedisAsync(string redisListKey, List<SlotHoldData> holds)
        {
            var json = JsonSerializer.Serialize(holds);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) // Hết hạn sau 24h
            };
            await _cache.SetStringAsync(redisListKey, json, options);
        }
        private async Task<List<SlotHoldData>> GetActiveHoldsFromRedisAsync(string redisListKey)
        {
            var json = await _cache.GetStringAsync(redisListKey);
            if (string.IsNullOrEmpty(json)) return new List<SlotHoldData>();
            var list = JsonSerializer.Deserialize<List<SlotHoldData>>(json) ?? new List<SlotHoldData>();
            return list.Where(h => h.ExpiresAt > DateTime.UtcNow).ToList();
        }
        private async Task<bool> CheckCapacityConflictInternalAsync(
            NailArtist artist, DateTime date, TimeSpan startTime, TimeSpan endTime, Guid customerId, string? excludingHoldToken)
        {
            int capacity = artist.ConcurrentCapacity;
            // 1. Lấy tất cả bookings từ DB
            var dbBookings = await _unitOfWork.BookingRepository.GetBookingsByArtistAndDateAsync(artist.NailArtistId, date);
            var overlappingBookings = dbBookings
                .Where(x => x.StartTime < endTime && x.StartTime.Add(TimeSpan.FromMinutes(x.TotalDuration)) > startTime)
                .ToList();
            // 2. Lấy holds từ Redis
            var redisListKey = BuildSlotKey(artist.NailArtistId, date);
            var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);
            var overlappingHolds = activeHolds
                                   .Where(h => h.CustomerId != customerId
                                          && (excludingHoldToken == null || h.HoldToken != excludingHoldToken)
                                          && h.StartTime < endTime
                                          && h.StartTime.Add(TimeSpan.FromMinutes(h.EstimatedDurationMinutes)) > startTime)
                                   .ToList();
            // 2.5. Lấy danh sách hàng chờ đang ở trạng thái Notified (Đang có 15 phút xác nhận)
            var activeNotifiedWaitlists = await _unitOfWork.BookingWaitlistRepository.GetActiveNotifiedWaitlistsAsync(artist.NailArtistId, date);
            var overlappingWaitlists = activeNotifiedWaitlists
                .Where(x => x.RequestedStartTime < endTime
                         && x.RequestedStartTime.Add(TimeSpan.FromMinutes(x.EstimatedDuration)) > startTime)
                .ToList();

            // 3. Quét các điểm mốc (Test Points)
            var testPoints = new List<TimeSpan> { startTime };
            foreach (var x in overlappingBookings)
            {
                if (x.StartTime > startTime && x.StartTime < endTime)
                {
                    testPoints.Add(x.StartTime);
                }
            }
            foreach (var x in overlappingHolds)
            {
                if (x.StartTime > startTime && x.StartTime < endTime)
                {
                    testPoints.Add(x.StartTime);
                }
            }
            foreach (var w in overlappingWaitlists)
            {
                if (w.RequestedStartTime > startTime && w.RequestedStartTime < endTime)
                {
                    testPoints.Add(w.RequestedStartTime);
                }
            }
            // 4. Đếm số lượng trùng lặp tại các điểm mốc
            foreach (var t in testPoints)
            {
                int dbCount = overlappingBookings.Count(x => x.StartTime <= t && x.StartTime.Add(TimeSpan.FromMinutes(x.TotalDuration)) > t);
                int holdCount = overlappingHolds.Count(x => x.StartTime <= t && x.StartTime.Add(TimeSpan.FromMinutes(x.EstimatedDurationMinutes)) > t);
                int waitlistCount = overlappingWaitlists.Count(x => x.RequestedStartTime <= t && x.RequestedStartTime.Add(TimeSpan.FromMinutes(x.EstimatedDuration)) > t);
                if (dbCount + holdCount +waitlistCount >= capacity)
                {
                    return true; // Hết chỗ
                }
            }
            return false;
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
            public Guid ArtistId { get; set; }
            public DateTime BookingDate { get; set; }
            public string SlotKey { get; set; } = string.Empty;
        }
    }
}
