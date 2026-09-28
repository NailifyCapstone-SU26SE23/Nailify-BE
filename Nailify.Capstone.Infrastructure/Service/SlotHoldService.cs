using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class SlotHoldService : ISlotHoldService
    {
        private readonly IDistributedCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISlotHoldConfiguration _config;
        private readonly ILogger<SlotHoldService> _logger;
        private readonly IBookingSchedulingService _bookingSchedulingService;
        private readonly IScheduledJobService _scheduledJobService;
        private readonly INotificationService _notificationService;
        public SlotHoldService(
               IDistributedCache cache,
               IUnitOfWork unitOfWork,
               ISlotHoldConfiguration config,
               ILogger<SlotHoldService> logger,
               IBookingSchedulingService bookingSchedulingService,
               IScheduledJobService scheduledJobService,
               INotificationService notificationService)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _config = config;
            _logger = logger;
            _bookingSchedulingService = bookingSchedulingService;
            _scheduledJobService = scheduledJobService;
            _notificationService = notificationService;
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
                Guid salonId = Guid.Empty;
                TimeSpan startTime = TimeSpan.Zero;

                if(holdToRemove != null)
                {
                    salonId = holdToRemove.SalonId;
                    startTime = holdToRemove.StartTime;
                    activeHolds.Remove(holdToRemove);
                    await SaveHoldToRedisAsync(redisListKey, activeHolds);
                }
                await _cache.RemoveAsync(tokenKey);
                await _unitOfWork.CommitTransactionAsync();
                
                if (holdToRemove != null)
                {
                    try
                    {
                        await _notificationService.SendNotificationToAllAsync("SlotStatusChanged", new
                        {
                            SalonId = salonId,
                            ArtistId = mapping.ArtistId,
                            BookingDate = mapping.BookingDate.ToString("yyyy-MM-dd"),
                            StartTime = startTime.ToString(@"hh\:mm"),
                            Action = "Booked"
                        });
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi gửi SignalR SlotStatusChanged (Booked) cho token {HoldToken}", holdToken);
                    }
                }
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
                   request.BookingItems.ToList(),
                   customerId,
                   excludingHoldToken: null);
                if (isConflict)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    
                    var redisList = BuildSlotKey(request.NailArtistId, request.BookingDate);
                    var activeHoldsSlot = await GetActiveHoldsFromRedisAsync(redisList);
                    var overlappingHold = activeHoldsSlot.FirstOrDefault(x => x.StartTime < endTime 
                                                                     && x.StartTime.Add(TimeSpan.FromMinutes(x.EstimatedDurationMinutes)) > request.StartTime);
                    if(overlappingHold != null)
                    {
                        var waitersKey = $"{_config.KeyPrefix}:waiters:{overlappingHold.HoldToken}";
                        var waitersJson = await _cache.GetStringAsync(waitersKey);
                        var waiters = string.IsNullOrEmpty(waitersJson)
                            ? new List<Guid>()
                            : JsonSerializer.Deserialize<List<Guid>>(waitersJson) ?? new List<Guid>();
                        if (!waiters.Contains(customerId))
                        {
                            waiters.Add(customerId);
                            await _cache.SetStringAsync(waitersKey, JsonSerializer.Serialize(waiters), new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_config.HoldDurationSeconds + 60)
                            });
                        }
                    }
                    return new ApiErrorResult<SlotHoldResponseDTO>("Thợ đã đầy lịch trong khoảng thời gian này, vui lòng chọn giờ khác.");
                }
                
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
                    existingHold.BookingItems = request.BookingItems.ToList();
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
                        ExpiresAt = expiresAt,
                        BookingItems = request.BookingItems.ToList()
                    };
                    activeHolds.Add(x);
                }
                
                await SaveHoldToRedisAsync(redisListKey, activeHolds);

                var tokenKey = $"{_config.KeyPrefix}:token:{holdToken}";
                var mapping = new TokenMapping { ArtistId = request.NailArtistId, BookingDate = request.BookingDate, SlotKey = holdToken };
                await SetCacheAsync(tokenKey, mapping, _config.HoldDurationSeconds);
                await _unitOfWork.CommitTransactionAsync();
                
                _scheduledJobService.Schedule<ISlotHoldService>(
                                                                x => x.ReleaseHoldAndNotifyWaitersAsync(holdToken),
                                                                TimeSpan.FromSeconds(_config.HoldDurationSeconds)
                );
                
                try
                {
                    await _notificationService.SendNotificationToAllAsync("SlotStatusChanged", new
                    {
                        SalonId = request.SalonId,
                        ArtistId = request.NailArtistId,
                        BookingDate = request.BookingDate.ToString("yyyy-MM-dd"),
                        StartTime = request.StartTime.ToString(@"hh\:mm"),
                        Action = "Held"
                    });
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Lỗi gửi SignalR SlotStatusChanged (Held) cho token {HoldToken}", holdToken);
                }
                
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
                TimeSpan startTime = TimeSpan.Zero; 
                Guid salonId = Guid.Empty;
                
                if (holdToRemove != null)
                {
                    if(holdToRemove.CustomerId != customerId)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return new ApiErrorResult<bool>("Bạn không có quyền giải phóng giữ chỗ này.");
                    }
                    startTime = holdToRemove.StartTime;
                    salonId = holdToRemove.SalonId;
                    activeHolds.Remove(holdToRemove);
                    await SaveHoldToRedisAsync(redisListKey, activeHolds);
                }
                await _cache.RemoveAsync(tokenKey);
                await _unitOfWork.CommitTransactionAsync();
                
                if (holdToRemove != null)
                {
                    await NotifyWaitersInternalAsync(holdToken, mapping.ArtistId, mapping.BookingDate, startTime);
                    try
                    {
                        await _notificationService.SendNotificationToAllAsync("SlotStatusChanged", new
                        {
                            SalonId = salonId,
                            ArtistId = mapping.ArtistId,
                            BookingDate = mapping.BookingDate.ToString("yyyy-MM-dd"),
                            StartTime = startTime.ToString(@"hh\:mm"),
                            Action = "Released"
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi gửi SignalR SlotStatusChanged (Released) cho token {HoldToken}", holdToken);
                    }
                }
                
                return new ApiResult<bool>(true, "Đã hủy giữ chỗ thành công.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
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
        
        private string BuildSlotKey(Guid artistId, DateTime date)
            => $"{_config.KeyPrefix}:list:{artistId}:{date:yyyyMMdd}";
            
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
            var itemList = items.ToList();
            if (!itemList.Any()) return 30;
            var variantIds = itemList.Where(x => x.NailVariantId.HasValue).Select(x => x.NailVariantId!.Value).Distinct().ToList();
            var serviceIds = itemList.Where(x => x.ServiceId.HasValue).Select(x => x.ServiceId!.Value).Distinct().ToList();
            var customRequestIds = itemList.Where(x => x.CustomerNailRequestId.HasValue).Select(x => x.CustomerNailRequestId!.Value).Distinct().ToList();

            var variantsMap = variantIds.Any() 
                ? (await _unitOfWork.NailVariantRepository.GetNailVariantsByIdsAsync(variantIds)).ToDictionary(x => x.NailVariantId) 
                : new Dictionary<int, NailVariant>();

            var servicesMap = serviceIds.Any()
                ? (await _unitOfWork.ServicesRepository.GetServicesByIdsAsync(serviceIds)).ToDictionary(x => x.ServiceId) 
                : new Dictionary<Guid, Domain.Entities.Services>();

            var customRequestsList = customRequestIds.Any() 
                ? await _unitOfWork.CustomerNailRequestRepository.GetCustomerNailRequestsByIdsAsync(customRequestIds)
                : new List<CustomerNailRequest>();
            var customRequestsMap = customRequestsList.ToDictionary(x => x.CustomerNailRequestId);

            var durationMinutes = 0;
            foreach (var x in itemList)
            {
                var itemDuration = 0;
                if (x.NailVariantId.HasValue && variantsMap.TryGetValue(x.NailVariantId.Value, out var variant))
                {    
                    itemDuration += (variant.Duration ?? 60);
                }

                if (x.ServiceId.HasValue && servicesMap.TryGetValue(x.ServiceId.Value, out var service))
                {       
                    itemDuration += service.Duration;
                }

                if (x.CustomerNailRequestId.HasValue && customRequestsMap.TryGetValue(x.CustomerNailRequestId.Value, out var customNailRequest))
                {
                    if (customNailRequest.SalonId == salonId && (customNailRequest.Status == CustomerNailStatus.Approved || customNailRequest.Status == CustomerNailStatus.Quoted))
                    {
                        itemDuration += customNailRequest.Duration ?? customNailRequest.CustomerNail?.Duration ?? 60;
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

        private async Task<bool> CheckCapacityConflictInternalAsync(NailArtist artist, DateTime date, TimeSpan startTime, List<BookingItemRequestDTO> requestItems, Guid customerId, string? excludingHoldToken)
        {
            int capacity = artist.ConcurrentCapacity;
            var salonId = artist.Account?.SalonId ?? Guid.Empty;

            var currentProcs = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(requestItems, salonId);
            var newSegments = _bookingSchedulingService.BuildProcedureTimeline(currentProcs, startTime);

            var simulatedSegments = new List<ProcedureScheduleSegment>();
            var redisListKey = BuildSlotKey(artist.NailArtistId, date);
            var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);
            var overlappingHolds = activeHolds
                .Where(h => h.CustomerId != customerId && (excludingHoldToken == null || h.HoldToken != excludingHoldToken))
                .ToList();

            foreach (var hold in overlappingHolds)
            {
                var holdProcs = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(hold.BookingItems, salonId);
                var holdTimeline = _bookingSchedulingService.BuildProcedureTimeline(holdProcs, hold.StartTime);
                simulatedSegments.AddRange(holdTimeline);
            }

            var activeNotifiedWaitlists = await _unitOfWork.BookingWaitlistRepository.GetActiveNotifiedWaitlistsAsync(artist.NailArtistId, date);
            foreach (var w in activeNotifiedWaitlists)
            {
                var waitlistItems = w.WaitlistItems.Select(x => new BookingItemRequestDTO
                {
                    ServiceId = x.ServiceId,
                    NailVariantId = x.NailVariantId,
                    Quantity = x.Quantity
                }).ToList();
                var waitlistProcs = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(waitlistItems, salonId);
                var waitlistTimeline = _bookingSchedulingService.BuildProcedureTimeline(waitlistProcs, w.RequestedStartTime);
                simulatedSegments.AddRange(waitlistTimeline);
            }

            return await _bookingSchedulingService.HasSimulationConflictAsync(
                artist.NailArtistId,
                date,
                newSegments,
                simulatedSegments,
                capacity
            );
        }

        public async Task ReleaseHoldAndNotifyWaitersAsync(string holdToken)
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
                await _unitOfWork.NailArtistRepository.GetArtistWithLockAsync(mapping.ArtistId);
                var redisListKey = BuildSlotKey(mapping.ArtistId, mapping.BookingDate);
                var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);
                var holdToRemove = activeHolds.FirstOrDefault(x => x.HoldToken == holdToken);
                
                TimeSpan startTime = TimeSpan.Zero;
                Guid salonId = Guid.Empty;
                
                if (holdToRemove != null)
                {
                    startTime = holdToRemove.StartTime;
                    salonId = holdToRemove.SalonId;
                    activeHolds.Remove(holdToRemove);
                    await SaveHoldToRedisAsync(redisListKey, activeHolds);
                }
                
                await _cache.RemoveAsync(tokenKey);
                await _unitOfWork.CommitTransactionAsync();
                
                if (holdToRemove != null)
                {
                    await NotifyWaitersInternalAsync(holdToken, mapping.ArtistId, mapping.BookingDate, startTime);
                    try
                    {
                        await _notificationService.SendNotificationToAllAsync("SlotStatusChanged", new
                        {
                            SalonId = salonId,
                            ArtistId = mapping.ArtistId,
                            BookingDate = mapping.BookingDate.ToString("yyyy-MM-dd"),
                            StartTime = startTime.ToString(@"hh\:mm"),
                            Action = "Released"
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi gửi SignalR SlotStatusChanged (Released auto-expired) cho token {HoldToken}", holdToken);
                    }
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi giải phóng giữ chỗ hết hạn: {HoldToken}", holdToken);
                throw;
            }
        }

        private async Task NotifyWaitersInternalAsync(string holdToken, Guid artistId, DateTime bookingDate, TimeSpan startTime)
        {
           var waitersKey = $"{_config.KeyPrefix}:waiters:{holdToken}";
           var waitersJson = await _cache.GetStringAsync(waitersKey);
            if (string.IsNullOrEmpty(waitersJson))
            {
                return; 
            }

            var waiters = JsonSerializer.Deserialize<List<Guid>>(waitersJson);
            if (waiters == null || !waiters.Any())
            {
                return; 
            }
            try
            {
                var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(artistId);
                var artistName = artist != null ? $"{artist.Account.FirstName} {artist.Account.LastName}" : "Thợ nail";

                foreach (var waiterId in waiters)
                {
                    await _notificationService.SendNotificationToUserAsync(
                        waiterId.ToString(),
                        "WaitlistPromoted",
                        new
                        {
                            ArtistName = artistName,
                            BookingDate = bookingDate.ToString("dd/MM/yyyy"),
                            StartTime = startTime.ToString(@"hh\:mm"),
                            Message = $"Lịch hẹn ngày {bookingDate:dd/MM/yyyy} lúc {startTime:hh\\:mm} với thợ {artistName} đã được giải phóng. Bạn hãy nhanh tay đăng ký giữ chỗ!"
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi thông báo cho waiters của token {HoldToken}", holdToken);
            }
            finally
            {
                await _cache.RemoveAsync(waitersKey);
            }
        }
        
        public async Task<List<(TimeSpan Start, TimeSpan End)>> GetActiveHoldRangesAsync(Guid artistId, DateTime date, Guid? customerId = null, string? excludingHoldToken = null)
        {
            var redisListKey = BuildSlotKey(artistId, date);
            var activeHolds = await GetActiveHoldsFromRedisAsync(redisListKey);
            return activeHolds
                .Where(x => (!customerId.HasValue || x.CustomerId != customerId.Value)
                         && (string.IsNullOrEmpty(excludingHoldToken) || x.HoldToken != excludingHoldToken))
                .Select(x => (Start: x.StartTime, End: x.StartTime.Add(TimeSpan.FromMinutes(x.EstimatedDurationMinutes))))
                .ToList();
        }

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
            public List<BookingItemRequestDTO> BookingItems { get; set; } = new();
        }
        
        private class TokenMapping
        {
            public Guid ArtistId { get; set; }
            public DateTime BookingDate { get; set; }
            public string SlotKey { get; set; } = string.Empty;
        }
    }
}
