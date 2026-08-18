using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Exceptions;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class BookingProcedureService : IBookingProcedureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IBookingSchedulingService _bookingSchedulingService;

        public BookingProcedureService(
                                        IUnitOfWork unitOfWork,
                                        IMapper mapper,
                                        INotificationService notificationService,
                                        IBookingSchedulingService bookingSchedulingService
                                      )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _bookingSchedulingService = bookingSchedulingService;
        }

        public async Task<ApiResult<BookingProcedureResponseDTO>> ClaimProcedureStepAsync(Guid bookingProcedureId, Guid accountId)
        {
            var procedure = await _unitOfWork.BookingProcedureRepository.GetProcedureWithBookingItemAsync(bookingProcedureId, trackChanges: true);
            if (procedure == null)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Không tìm thấy bước quy trình yêu cầu.");
            }

            if (procedure.Status != BookingProcedureStatus.Pending || procedure.AssignedArtistId.HasValue)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Công đoạn này đã được nhận bởi thợ khác hoặc không ở trạng thái chờ nhận.");
            }

            var otherProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(procedure.BookingItem.BookingId);
            var customerIsBusy = otherProcedures.Any(x =>
                x.Status == BookingProcedureStatus.InProgress
                && x.BookingProcedureId != bookingProcedureId
                && (!x.ActualStartTime.HasValue || (DateTime.UtcNow.AddHours(7) - x.ActualStartTime.Value).TotalMinutes < x.ActiveDuration)
            );
            if (customerIsBusy)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Khách hàng này đang được thực hiện một công đoạn khác. Không thể nhận thêm công đoạn lúc này.");
            }

            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistByAccountIdAsync(accountId);
            if (artist == null)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Tài khoản đăng nhập không liên kết với thợ nail nào.");
            }

            var artistId = artist.NailArtistId;
            if (artist.ConcurrentCapacity == 1)
            {
                var isBusy = await _unitOfWork.BookingProcedureRepository.HasAnyInProgressProcedureAsync(artistId);
                if (isBusy)
                {
                    return new ApiErrorResult<BookingProcedureResponseDTO>(
                        $"Thợ {artist.Account.FirstName} {artist.Account.LastName} đang bận thực hiện công đoạn khác. " +
                        "Vui lòng hoàn thành công việc hiện tại trước khi nhận công đoạn mới.");
                }
            }
            // Đảm bảo tính tuần tự (Bước trước phải hoàn thành thì mới được nhận bước sau)
            if (procedure.StepOrder > 1)
            {
                var allProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingItemIdAsync(procedure.BookingItemId);
                var blockingProcedures = allProcedures.Where(x => x.StepOrder < procedure.StepOrder && x.IsRequired && !x.CanOverlap)
                    .ToList();
                foreach (var x in blockingProcedures)
                {
                    if (x.Status != BookingProcedureStatus.Completed && x.Status != BookingProcedureStatus.Skipped)
                    {
                        if (x.Status == BookingProcedureStatus.InProgress && x.ActualStartTime.HasValue)
                        {
                            var activeTime = DateTime.UtcNow.AddHours(7) - x.ActualStartTime.Value;
                            if (activeTime.TotalMinutes >= x.ActiveDuration)
                            {
                                // Đã làm xong phần active, khách đang ngồi ngâm/chờ gel khô -> không block bước tiếp theo
                                continue;
                            }
                        }
                        return new ApiErrorResult<BookingProcedureResponseDTO>(
                            $"Không thể bắt đầu bước này. Bước trước đó '{x.ProcedureName}' chưa hoàn thành hoặc chưa kết thúc phần việc thợ cần thao tác.");
                    }
                }
            }

            procedure.Status = BookingProcedureStatus.InProgress;
            procedure.CompletedById = null;
            procedure.AssignedArtistId = artistId;
            procedure.ActualStartTime = DateTime.UtcNow.AddHours(7);
            procedure.CompletedAt = null;

            try
            {
                _unitOfWork.BookingProcedureRepository.Update(procedure);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ConcurrencyException)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Công đoạn này vừa được một thợ khác nhận trước đó. Vui lòng tải lại danh sách.");
            }

            var updatedProcs = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingItemIdAsync(procedure.BookingItemId);
            var targetProc = updatedProcs.First(x => x.BookingProcedureId == bookingProcedureId);
            var response = _mapper.Map<BookingProcedureResponseDTO>(targetProc);
            return new ApiSuccessResult<BookingProcedureResponseDTO>(response, "Nhận công đoạn thành công. Hãy bắt đầu phục vụ.");
        }

        public async Task DuplicateProceduresForBookingItemAsync(BookingItem item)
        {
            int currentStepOrder = 1;

            // 1. Quy trình mẫu móng NailVariant
            if (item.NailVariantId.HasValue)
            {
                var activeNailProcedures = (await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(item.NailVariantId.Value))
                    .OrderBy(x => x.StepOrder)
                    .ToList();

                if (activeNailProcedures.Any())
                {
                    int variantTargetDuration = item.Duration;
                    if (variantTargetDuration <= 0)
                    {
                        var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(item.NailVariantId.Value);
                        variantTargetDuration = variant?.Duration ?? 0;
                    }

                    int totalCatalogDuration = activeNailProcedures.Sum(x => x.Procedure.Duration ?? 0);

                    if (variantTargetDuration > 0 && totalCatalogDuration > 0 && variantTargetDuration != totalCatalogDuration)
                    {
                        double scaleFactor = (double)variantTargetDuration / totalCatalogDuration;
                        int accumulatedDuration = 0;
                        int count = activeNailProcedures.Count;

                        for (int i = 0; i < count; i++)
                        {
                            var x = activeNailProcedures[i];
                            int catalogDuration = x.Procedure.Duration ?? 0;
                            int scaledDuration = (int)Math.Max(1, Math.Round(catalogDuration * scaleFactor));

                            if (i == count - 1)
                            {
                                scaledDuration = Math.Max(1, variantTargetDuration - accumulatedDuration);
                            }
                            else
                            {
                                accumulatedDuration += scaledDuration;
                            }

                            int scaledActive = (int)Math.Min(scaledDuration, Math.Max(1, Math.Round(x.Procedure.ActiveDuration * scaleFactor)));
                            int scaledPassive = Math.Max(0, scaledDuration - scaledActive);

                            var bookingProcedure = new BookingProcedure
                            {
                                BookingItemId = item.BookingItemId,
                                ProcedureId = x.ProcedureId,
                                ProcedureName = x.Procedure.Name,
                                StepOrder = currentStepOrder++,
                                Duration = scaledDuration,
                                ActiveDuration = scaledActive,
                                PassiveDuration = scaledPassive,
                                CanOverlap = x.Procedure.CanOverlap,
                                IsRequired = x.Procedure.IsRequired,
                                IsMainStep = x.Procedure.IsMainStep,
                                Status = BookingProcedureStatus.Pending
                            };
                            await _unitOfWork.BookingProcedureRepository.CreateAsync(bookingProcedure);
                        }
                    }
                    else
                    {
                        foreach (var x in activeNailProcedures)
                        {
                            var bookingProcedure = new BookingProcedure
                            {
                                BookingItemId = item.BookingItemId,
                                ProcedureId = x.ProcedureId,
                                ProcedureName = x.Procedure.Name,
                                StepOrder = currentStepOrder++,
                                Duration = x.Procedure.Duration ?? 0,
                                ActiveDuration = x.Procedure.ActiveDuration,
                                PassiveDuration = x.Procedure.PassiveDuration,
                                CanOverlap = x.Procedure.CanOverlap,
                                IsRequired = x.Procedure.IsRequired,
                                IsMainStep = x.Procedure.IsMainStep,
                                Status = BookingProcedureStatus.Pending
                            };
                            await _unitOfWork.BookingProcedureRepository.CreateAsync(bookingProcedure);
                        }
                    }
                }
            }

            // 2. Quy trình mẫu móng custom (CustomerNailRequest)
            if (item.CustomerNailRequestId.HasValue)
            {
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(item.CustomerNailRequestId.Value);
                var customNail = customNailRequest == null
                    ? null
                    : await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);

                var activeNailProcedures = customNail != null
                    ? (await _unitOfWork.NailProcedureRepository.GetActiveProceduresByCustomerNailIdAsync(customNail.CustomerNailId)).OrderBy(x => x.StepOrder).ToList()
                    : new List<NailProcedure>();

                int targetDuration = customNailRequest?.Duration ?? customNail?.Duration ?? 60;

                if (activeNailProcedures != null && activeNailProcedures.Any())
                {
                    int totalCatalogDuration = activeNailProcedures.Sum(x => x.Procedure.Duration ?? 0);

                    if (targetDuration > 0 && totalCatalogDuration > 0 && targetDuration != totalCatalogDuration)
                    {
                        double scaleFactor = (double)targetDuration / totalCatalogDuration;
                        int accumulatedDuration = 0;
                        int count = activeNailProcedures.Count;

                        for (int i = 0; i < count; i++)
                        {
                            var x = activeNailProcedures[i];
                            int catalogDuration = x.Procedure.Duration ?? 0;
                            int scaledDuration = (int)Math.Max(1, Math.Round(catalogDuration * scaleFactor));

                            if (i == count - 1)
                            {
                                scaledDuration = Math.Max(1, targetDuration - accumulatedDuration);
                            }
                            else
                            {
                                accumulatedDuration += scaledDuration;
                            }

                            int scaledActive = (int)Math.Min(scaledDuration, Math.Max(1, Math.Round(x.Procedure.ActiveDuration * scaleFactor)));
                            int scaledPassive = Math.Max(0, scaledDuration - scaledActive);

                            var bookingProcedure = new BookingProcedure
                            {
                                BookingItemId = item.BookingItemId,
                                ProcedureId = x.ProcedureId,
                                ProcedureName = x.Procedure.Name,
                                StepOrder = currentStepOrder++,
                                Duration = scaledDuration,
                                ActiveDuration = scaledActive,
                                PassiveDuration = scaledPassive,
                                CanOverlap = x.Procedure.CanOverlap,
                                IsRequired = x.Procedure.IsRequired,
                                IsMainStep = x.Procedure.IsMainStep,
                                Status = BookingProcedureStatus.Pending
                            };
                            await _unitOfWork.BookingProcedureRepository.CreateAsync(bookingProcedure);
                        }
                    }
                    else
                    {
                        foreach (var x in activeNailProcedures)
                        {
                            var bookingProcedure = new BookingProcedure
                            {
                                BookingItemId = item.BookingItemId,
                                ProcedureId = x.ProcedureId,
                                ProcedureName = x.Procedure.Name,
                                StepOrder = currentStepOrder++,
                                Duration = x.Procedure.Duration ?? 0,
                                ActiveDuration = x.Procedure.ActiveDuration,
                                PassiveDuration = x.Procedure.PassiveDuration,
                                CanOverlap = x.Procedure.CanOverlap,
                                IsRequired = x.Procedure.IsRequired,
                                IsMainStep = x.Procedure.IsMainStep,
                                Status = BookingProcedureStatus.Pending
                            };
                            await _unitOfWork.BookingProcedureRepository.CreateAsync(bookingProcedure);
                        }
                    }
                }
                else
                {
                    var bookingProcedure = new BookingProcedure
                    {
                        BookingItemId = item.BookingItemId,
                        ProcedureName = customNail?.Name ?? "Mẫu móng custom",
                        StepOrder = currentStepOrder++,
                        Duration = targetDuration,
                        ActiveDuration = targetDuration,
                        PassiveDuration = 0,
                        CanOverlap = false,
                        IsRequired = true,
                        IsMainStep = true,
                        Status = BookingProcedureStatus.Pending
                    };
                    await _unitOfWork.BookingProcedureRepository.CreateAsync(bookingProcedure);
                }
            }

            // 3. Quy trình làm dáng móng (ShapeMethodConfig)
            if (item.ShapeMethodConfigId.HasValue)
            {
                var shapeMethodConfig = await _unitOfWork.ShapeMethodConfigRepository.GetByIdAsync(item.ShapeMethodConfigId.Value);
                if (shapeMethodConfig != null)
                {
                    var bookingProcedure = new BookingProcedure
                    {
                        BookingItemId = item.BookingItemId,
                        ProcedureName = $"Tạo dáng & làm móng: {shapeMethodConfig.Name}",
                        StepOrder = currentStepOrder++,
                        Duration = shapeMethodConfig.Duration,
                        ActiveDuration = shapeMethodConfig.Duration,
                        PassiveDuration = 0,
                        CanOverlap = false,
                        IsRequired = true,
                        IsMainStep = true,
                        Status = BookingProcedureStatus.Pending
                    };
                    await _unitOfWork.BookingProcedureRepository.CreateAsync(bookingProcedure);
                }
            }

            // 4. Quy trình dịch vụ phụ đi kèm (Service)
            if (item.ServiceId.HasValue)
            {
                var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                if (service != null)
                {
                    var bookingProcedure = new BookingProcedure
                    {
                        BookingItemId = item.BookingItemId,
                        ProcedureName = service.Name,
                        StepOrder = currentStepOrder++,
                        Duration = service.Duration,
                        ActiveDuration = service.Duration,
                        PassiveDuration = 0,
                        CanOverlap = false,
                        IsRequired = true,
                        IsMainStep = true,
                        Status = BookingProcedureStatus.Pending
                    };
                    await _unitOfWork.BookingProcedureRepository.CreateAsync(bookingProcedure);
                }
            }
        }

        public async Task<ApiResult<List<BookingProcedureResponseDTO>>> GetProceduresByBookingItemIdAsync(Guid bookingItemId)
        {
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingItemIdAsync(bookingItemId);
            var response = _mapper.Map<List<BookingProcedureResponseDTO>>(procedures);
            return new ApiSuccessResult<List<BookingProcedureResponseDTO>>(response, "Lấy danh sách quy trình thành công.");
        }

        public async Task<ApiResult<BookingProcedureResponseDTO>> UpdateProcedureStatusAsync(Guid bookingProcedureId, Guid artistId, BookingProcedureStatus status)
        {
            var existbooking = await _unitOfWork.BookingProcedureRepository.GetProcedureWithBookingItemAsync(bookingProcedureId, trackChanges: true);
            if (existbooking == null)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Không tìm thấy bước quy trình yêu cầu.");
            }
            var existartist = await _unitOfWork.NailArtistRepository.ExistsAsync(a => a.NailArtistId == artistId);
            if (!existartist)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Không tìm thấy thông tin thợ nail.");
            }

            if (status == BookingProcedureStatus.InProgress && existbooking.Status != BookingProcedureStatus.InProgress)
            {
                var otherProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(existbooking.BookingItem.BookingId);
                var customerIsBusy = otherProcedures.Any(x =>
                    x.Status == BookingProcedureStatus.InProgress
                    && x.BookingProcedureId != bookingProcedureId
                    && (!x.ActualStartTime.HasValue || (DateTime.UtcNow.AddHours(7) - x.ActualStartTime.Value).TotalMinutes < x.ActiveDuration)
                );
                if (customerIsBusy)
                {
                    return new ApiErrorResult<BookingProcedureResponseDTO>("Khách hàng này đang được thực hiện một công đoạn khác. Không thể chuyển công đoạn này sang InProgress.");
                }

                // Đảm bảo tính tuần tự (Bước trước phải hoàn thành thì mới được nhận bước sau)
                if (existbooking.StepOrder > 1)
                {
                    var allProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingItemIdAsync(existbooking.BookingItemId);
                    var blockingProcedures = allProcedures.Where(x => x.StepOrder < existbooking.StepOrder && x.IsRequired && !x.CanOverlap)
                        .ToList();
                    foreach (var x in blockingProcedures)
                    {
                        if (x.Status != BookingProcedureStatus.Completed && x.Status != BookingProcedureStatus.Skipped)
                        {
                            if (x.Status == BookingProcedureStatus.InProgress && x.ActualStartTime.HasValue)
                            {
                                var activeTime = DateTime.UtcNow.AddHours(7) - x.ActualStartTime.Value;
                                if (activeTime.TotalMinutes >= x.ActiveDuration)
                                {
                                    // Đã làm xong phần active, khách đang ngồi ngâm/chờ gel khô -> không block bước tiếp theo
                                    continue;
                                }
                            }
                            return new ApiErrorResult<BookingProcedureResponseDTO>(
                                $"Không thể bắt đầu bước này. Bước trước đó '{x.ProcedureName}' chưa hoàn thành hoặc chưa kết thúc phần việc thợ cần thao tác.");
                        }
                    }
                }
            }

            existbooking.Status = status;
            if (status == BookingProcedureStatus.Completed)
            {
                existbooking.CompletedAt = DateTime.UtcNow.AddHours(7);
                existbooking.ActualEndTime = DateTime.UtcNow.AddHours(7);
                if (!existbooking.ActualStartTime.HasValue)
                {
                    existbooking.ActualStartTime = DateTime.UtcNow.AddHours(7);
                }
                existbooking.CompletedById = artistId;
                try
                {
                    var allProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingItemIdAsync(existbooking.BookingItemId);

                    var nextProcedure = allProcedures
                                                    .Where(x => x.StepOrder > existbooking.StepOrder
                                                            && x.Status != BookingProcedureStatus.Completed
                                                            && x.Status != BookingProcedureStatus.Skipped)
                                                    .OrderBy(x => x.StepOrder)
                                                    .FirstOrDefault();

                    if (nextProcedure != null && nextProcedure.AssignedArtistId.HasValue)
                    {
                        var currentArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(artistId);
                        var currentArtistName = currentArtist != null ? $"{currentArtist.Account.FirstName} {currentArtist.Account.LastName}" : "Thợ nail";

                        var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(existbooking.BookingItem.BookingId);
                        var customerName = booking != null ? $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}" : "Khách hàng";

                        var nextArtistAccountId = nextProcedure.AssignedArtist.AccountId;

                        await _notificationService.SendNotificationToUserAsync(
                            nextArtistAccountId.ToString(),
                            "NextStepReady",
                            new
                            {
                                BookingProcedureId = nextProcedure.BookingProcedureId,
                                BookingItemId = nextProcedure.BookingItemId,
                                Message = $"Thợ {currentArtistName} đã hoàn thành bước '{existbooking.ProcedureName}' cho khách {customerName}. Mời bạn vào thực hiện bước tiếp theo '{nextProcedure.ProcedureName}'."
                            }
                        );
                    }
                }
                catch (Exception ex)
                {
                    // Tránh việc lỗi SignalR/thông tin phụ làm gián đoạn luồng chính
                    Console.WriteLine($"Error sending SignalR notification for next step: {ex.Message}");
                }
            }
            else if (status == BookingProcedureStatus.InProgress)
            {
                existbooking.CompletedAt = null;
                if (!existbooking.ActualStartTime.HasValue)
                {
                    existbooking.ActualStartTime = DateTime.UtcNow.AddHours(7);
                }
                existbooking.ActualEndTime = null;
                existbooking.AssignedArtistId = artistId;
                existbooking.CompletedById = null;
            }
            else if (status == BookingProcedureStatus.Pending)
            {
                existbooking.CompletedAt = null;
                existbooking.ActualStartTime = null;
                existbooking.ActualEndTime = null;
                existbooking.AssignedArtistId = artistId;
                existbooking.Status = BookingProcedureStatus.Pending;
                existbooking.CompletedById = null;
            }
            else
            {
                existbooking.CompletedAt = null;
                existbooking.CompletedById = null;
                existbooking.ActualStartTime = null;
                existbooking.ActualEndTime = null;
            }

            try
            {
                _unitOfWork.BookingProcedureRepository.Update(existbooking);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ConcurrencyException)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Dữ liệu đã bị thay đổi bởi một tác vụ khác. Vui lòng tải lại trang.");
            }

            var updatedProc = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingItemIdAsync(existbooking.BookingItemId);
            var mapper = updatedProc.First(x => x.BookingProcedureId == bookingProcedureId);
            var response = _mapper.Map<BookingProcedureResponseDTO>(mapper);
            return new ApiSuccessResult<BookingProcedureResponseDTO>(response, "Cập nhật trạng thái bước quy trình thành công.");
        }

        public async Task<ApiResult<List<IdleArtistResponseDTO>>> GetAvailableArtistsForProcedureAsync(Guid bookingProcedureId)
        {
            var procedure = await _unitOfWork.BookingProcedureRepository.GetProcedureWithBookingItemAsync(bookingProcedureId);

            if (procedure == null)
            {
                return new ApiErrorResult<List<IdleArtistResponseDTO>>("Không tìm thấy bước quy trình.");
            }

            var salonId = procedure.BookingItem.Booking.SalonId;

            // 1. Lấy danh sách kỹ năng yêu cầu cho mẫu nail (nếu có) từ repository chuyên biệt
            var requiredSkills = new List<NailRequiredSkill>();
            if (procedure.BookingItem.NailVariantId.HasValue)
            {
                requiredSkills = await _unitOfWork.NailRequiredSkillRepository.GetSkillsByDesignIdAsync(procedure.BookingItem.NailVariantId.Value);
            }

            // 2. Lấy tất cả thợ hoạt động tại salon kèm theo danh sách kỹ năng của họ từ repository
            var artists = await _unitOfWork.NailArtistRepository.GetArtistsWithSkillsBySalonIdAsync(salonId);

            var responseList = new List<IdleArtistResponseDTO>();

            foreach (var artist in artists)
            {
                // A. Kiểm tra bận (IsFree): thợ ko có bất kỳ BookingProcedure nào ở trạng thái InProgress
                var isBusy = await _unitOfWork.BookingProcedureRepository.HasAnyInProgressProcedureAsync(artist.NailArtistId);
                var isFree = !isBusy;

                // B. Kiểm tra trình độ (IsQualified)
                bool isQualified = true;
                foreach (var reqSkill in requiredSkills)
                {
                    var artistSkill = artist.NailArtistSkills.FirstOrDefault(x => x.SkillTypeId == reqSkill.SkillTypeId);
                    if (artistSkill == null || artistSkill.Level < reqSkill.RequiredLevel)
                    {
                        isQualified = false;
                        break;
                    }
                }

                responseList.Add(new IdleArtistResponseDTO
                {
                    NailArtistId = artist.NailArtistId,
                    Name = artist.Account != null ? $"{artist.Account.FirstName} {artist.Account.LastName}" : "Thợ nail",
                    IsFree = isFree,
                    IsQualified = isQualified
                });
            }

            return new ApiSuccessResult<List<IdleArtistResponseDTO>>(responseList, "Lấy danh sách thợ rảnh thành công.");
        }

        public async Task<ApiResult<List<BookingProcedureResponseDTO>>> GetArtistActiveProceduresAsync(Guid artistId)
        {
            var procedures = await _unitOfWork.BookingProcedureRepository.GetActiveProceduresByArtistIdAsync(artistId);
            var response = _mapper.Map<List<BookingProcedureResponseDTO>>(procedures);
            return new ApiSuccessResult<List<BookingProcedureResponseDTO>>(response, "Lấy danh sách công việc của thợ thành công.");
        }

        public async Task<ApiResult<List<BookingProcedureResponseDTO>>> GetClaimableProceduresAsync(Guid salonId)
        {
            var procedures = await _unitOfWork.BookingProcedureRepository.GetClaimableProceduresBySalonIdAsync(salonId);
            var response = _mapper.Map<List<BookingProcedureResponseDTO>>(procedures);
            return new ApiSuccessResult<List<BookingProcedureResponseDTO>>(response, "Lấy danh sách công việc có thể nhận thành công.");
        }
  
        public async Task<ApiResult<OnsiteAddonSimulationResponseDTO>> SimulateOnsiteAddonAsync(SimulateOnsiteAddonRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId);
            if (booking == null)
            {
                return new ApiErrorResult<OnsiteAddonSimulationResponseDTO>("Không tìm thấy booking yêu cầu.");
            }
            if (booking.Status != BookingStatus.CheckedIn && booking.Status != BookingStatus.InProgress)
            {
                return new ApiErrorResult<OnsiteAddonSimulationResponseDTO>("Chỉ có thể phát sinh dịch vụ khi khách đã Check-in hoặc đang làm.");
            }
            if (!booking.NailArtistId.HasValue)
            {
                return new ApiErrorResult<OnsiteAddonSimulationResponseDTO>("Lịch hẹn chưa được gán thợ chính.");
            }
            if (request.AddonItems == null || !request.AddonItems.Any())
            {
                return new ApiErrorResult<OnsiteAddonSimulationResponseDTO>("Vui lòng chọn ít nhất một dịch vụ hoặc mẫu móng phát sinh.");
            }

            var addonNames = new List<string>();
            int totalDurationMinutes = 0;
            decimal totalAddonPrice = 0;

            foreach (var item in request.AddonItems)
            {
                if (item.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                    if (service != null)
                    {
                        addonNames.Add(service.Name);
                        totalDurationMinutes += service.Duration;
                        totalAddonPrice += service.Price;
                    }
                }
                else if (item.NailVariantId.HasValue)
                {
                    var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(item.NailVariantId.Value);
                    if (variant != null)
                    {
                        addonNames.Add(variant.Name);
                        totalDurationMinutes += (variant.Duration ?? 60);
                        totalAddonPrice += variant.Price;
                    }
                }
            }
            if (!addonNames.Any())
            {
                return new ApiErrorResult<OnsiteAddonSimulationResponseDTO>("Không tìm thấy thông tin các dịch vụ chọn thêm.");
            }
            var primaryArtistId = booking.NailArtistId.Value;
            var primaryArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(primaryArtistId);
            var primaryArtistName = primaryArtist != null ? $"{primaryArtist.Account.FirstName} {primaryArtist.Account.LastName}" : "Thợ chính";

            // 2. Tính toán thời gian bắt đầu của danh sách dịch vụ phát sinh
            var existingProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId);
            var timeline = _bookingSchedulingService.BuildProcedureTimeline(existingProcedures, booking.StartTime);


            var addonStartTime = timeline.Any() ? timeline.Max(x => x.EndTime) : booking.StartTime;
            var addonEndTime = addonStartTime.Add(TimeSpan.FromMinutes(totalDurationMinutes));

            var newSegment = new ProcedureScheduleSegment
            {
                BookingProcedureId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                AssignedArtistId = primaryArtistId,
                IsMainStep = true,
                StartTime = addonStartTime,
                EndTime = addonEndTime,
                ArtistBusyStart = addonStartTime,
                ArtistBusyEnd = addonStartTime.Add(TimeSpan.FromMinutes(totalDurationMinutes + 1)),
                CanOverlap = false,
                TransitionBuffer = 1
            };

            var hasPrimaryConflict = await _bookingSchedulingService.HasSimulationConflictAsync(
              primaryArtistId,
              booking.BookingDate,
              new List<ProcedureScheduleSegment> { newSegment },
              new List<ProcedureScheduleSegment>(),
              capacity: primaryArtist?.ConcurrentCapacity ?? 1,
              excludingBookingId: booking.BookingId
            );

            string joinedNames = string.Join(", ", addonNames);
            var response = new OnsiteAddonSimulationResponseDTO
            {
                PrimaryArtistId = primaryArtistId,
                PrimaryArtistName = primaryArtistName,
                AddonNames = addonNames,
                TotalAddonDurationMinutes = totalDurationMinutes,
                TotalAddonPrice = totalAddonPrice,
                NewTotalDurationMinutes = booking.TotalDuration + totalDurationMinutes,
                NewTotalPrice = (booking.TotalPrice ?? 0) + totalAddonPrice
            };
            // Thợ chính rảnh
            if (!hasPrimaryConflict)
            {
                response.HasConflict = false;
                response.CanMultiArtistSplit = false;
                response.RecommendationMessage = $"Thợ {primaryArtistName} rảnh từ {addonStartTime:hh\\:mm} - {addonEndTime:hh\\:mm}. Có thể thêm các dịch vụ phát sinh: [{joinedNames}].";

                return new ApiSuccessResult<OnsiteAddonSimulationResponseDTO>(response, "Giả lập thành công: Thợ chính rảnh khung giờ này.");
            }
            // Thợ chính bận
            response.HasConflict = true;
            var activeArtist = await _unitOfWork.NailArtistRepository.GetArtistsWithSkillsBySalonIdAsync(booking.SalonId);
            var candidateSecondaryArtist = activeArtist.Where(x => x.NailArtistId != primaryArtistId).ToList();

            NailArtist? availableSecondary = null;
            foreach (var candidate in candidateSecondaryArtist)
            {
                var secondarySegment = new ProcedureScheduleSegment
                {
                    BookingProcedureId = Guid.NewGuid(),
                    BookingId = booking.BookingId,
                    AssignedArtistId = candidate.NailArtistId,
                    IsMainStep = true,
                    StartTime = addonStartTime,
                    EndTime = addonEndTime,
                    ArtistBusyStart = addonStartTime,
                    ArtistBusyEnd = addonStartTime.Add(TimeSpan.FromMinutes(totalDurationMinutes + 1)),
                    CanOverlap = false,
                    TransitionBuffer = 1
                };

                var hasSecondaryConflict = await _bookingSchedulingService.HasSimulationConflictAsync(
                                            candidate.NailArtistId,
                                            booking.BookingDate,
                                            new List<ProcedureScheduleSegment> { secondarySegment },
                                            new List<ProcedureScheduleSegment>(),
                                            capacity: candidate.ConcurrentCapacity,
                                            excludingBookingId: booking.BookingId
                );

                if (!hasSecondaryConflict)
                {
                    availableSecondary = candidate;
                    break;
                }
            }
            if (availableSecondary != null)
            {
                var secondaryName = availableSecondary.Account != null ? $"{availableSecondary.Account.FirstName} {availableSecondary.Account.LastName}" : "Thơ phụ";
                response.CanMultiArtistSplit = true;
                response.SuggestedSecondaryArtistId = availableSecondary.NailArtistId;
                response.SuggestedSecondaryArtistName = secondaryName;
                response.WarningMessage = $"Thợ {primaryArtistName} bận ca tiếp theo lúc {addonStartTime:hh\\:mm}.";
                response.RecommendationMessage = $"Gợi ý Lễ tân: Bàn giao Khách D cho Thợ phụ {secondaryName} làm các dịch vụ phát sinh [{joinedNames}] ({totalDurationMinutes}p) từ {addonStartTime:hh\\:mm}.";

                await _notificationService.SendNotificationToSalonStaffAsync(
                    booking.Salon.ToString(),
                    "OnsiteAddonConflictNotification",
                    new
                    {
                        BookingId = booking.BookingId,
                        PrimaryArtistId = primaryArtistId,
                        PrimaryArtistName = primaryArtistName,
                        AddonNames = addonNames,
                        DurationMinutes = totalDurationMinutes,
                        CanMultiArtistSplit = true,
                        SuggestedSecondaryArtistId = availableSecondary.NailArtistId,
                        SuggestedSecondaryArtistName = secondaryName,
                        Message = response.RecommendationMessage
                    }
                );
                return new ApiSuccessResult<OnsiteAddonSimulationResponseDTO>(response, "Thợ chính bận ca tiếp theo. Đã gửi cảnh báo và gợi ý Thợ phụ tới Lễ tân.");
            }
            // Ko có thợ rảnh => hẹn lịch khác
            response.CanMultiArtistSplit = false;
            var alternativeTime = addonEndTime.Add(TimeSpan.FromSeconds(30));
            response.SuggestedAlternativeTime = alternativeTime;

            response.WarningMessage = $"Thợ {primaryArtistName} bận ca tiếp theo và Salon không có Thợ phụ rảnh khung giờ {addonStartTime:hh\\:mm}.";
            response.RecommendationMessage = $"Gợi ý Lễ tân: Trao đổi hẹn khách làm các dịch vụ phát sinh [{joinedNames}] lúc {alternativeTime:hh\\:mm} sau khi xong ca trùng.";
            await _notificationService.SendNotificationToSalonStaffAsync(
                                     booking.SalonId.ToString(),
                                    "OnsiteAddonConflictNotification",
                                     new
                                    {
                                        BookingId = booking.BookingId,
                                        PrimaryArtistId = primaryArtistId,
                                        PrimaryArtistName = primaryArtistName,
                                        AddonNames = addonNames,
                                        CanMultiArtistSplit = false,
                                        SuggestedAlternativeTime = alternativeTime,
                                        Message = response.RecommendationMessage
                                     }
            );
            return new ApiSuccessResult<OnsiteAddonSimulationResponseDTO>(response, "Salon hết thợ rảnh. Đã gửi thông báo tới Lễ tân để  hẹn giờ khác");
        }

        public async Task<ApiResult<List<BookingProcedureResponseDTO>>> ConfirmOnsiteAddonAsync(ConfirmOnsiteAddonRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId, trackChanges: true);
            if(booking == null)
            {
                return new ApiErrorResult<List<BookingProcedureResponseDTO>>("Không tìm thấy đơn đặt lịch.");
            }
            if(request.AddonItems == null || !request.AddonItems.Any())
            {
                return new ApiErrorResult<List<BookingProcedureResponseDTO>>("Không có dịch vụ phát sinh nào được chọn.");
            }
            var assignedArtistId = request.AssignedArtistId ?? booking.NailArtistId;
            var existingProcedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId);
            int nextStepOrder = existingProcedures.Any() ? existingProcedures.Max(x => x.StepOrder) + 1 : 1;

            int totalAddedDuration = 0;
            decimal totalAddedPrice = 0;
            var createdProcedures = new List<BookingProcedure>();

            foreach(var item in request.AddonItems)
            {
                string procedureName = null;
                int durationMinutes = 30;
                decimal addonPrice = 0;

                if (item.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                    if(service != null)
                    {
                        procedureName = service.Name;
                        addonPrice = service.Price;
                        durationMinutes = service.Duration;
                    }
                }
                else if (item.NailVariantId.HasValue)
                {
                    var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(item.NailVariantId.Value);
                    if(variant != null)
                    {
                        procedureName = variant.Name;
                        durationMinutes = variant.Duration ?? 60;
                        addonPrice = variant.Price;
                    }
                }
                var bookingItem = new BookingItem
                {
                    BookingItemId = Guid.NewGuid(),
                    BookingId = booking.BookingId,
                    ServiceId = item.ServiceId,
                    NailVariantId = item.NailVariantId,
                    Price = addonPrice,
                    Duration = durationMinutes,
                    Quantity = 1
                };

                await _unitOfWork.BookingItemRepository.CreateAsync(bookingItem);
                var newProcedure = new BookingProcedure
                {
                    BookingProcedureId = Guid.NewGuid(),
                    BookingItemId = bookingItem.BookingItemId,
                    ProcedureName = procedureName,
                    StepOrder = nextStepOrder++,
                    Duration = durationMinutes,
                    ActiveDuration = durationMinutes,
                    PassiveDuration = 0,
                    CanOverlap = false,
                    IsRequired = true,
                    IsMainStep = true,
                    Status = BookingProcedureStatus.Pending,
                    AssignedArtistId = assignedArtistId
                };
                await _unitOfWork.BookingProcedureRepository.CreateAsync(newProcedure);

                totalAddedDuration += durationMinutes;
                totalAddedPrice += addonPrice;
                createdProcedures.Add(newProcedure);
            }

            booking.TotalDuration += totalAddedDuration;
            booking.Price = (booking.Price ?? 0) + totalAddedPrice;
            booking.TotalPrice = (booking.TotalPrice ?? 0) + totalAddedPrice;

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            if (assignedArtistId.HasValue && assignedArtistId != booking.NailArtistId)
            {
                var secondaryArtist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(assignedArtistId.Value);
                if (secondaryArtist != null)
                {
                    await _notificationService.SendNotificationToUserAsync(
                        secondaryArtist.AccountId.ToString(),
                        "MultiArtistHandoffAssigned",
                        new
                        {
                            BookingId = booking.BookingId,
                            Message = $"Bạn được Lễ tân phân công tiếp nhận {request.AddonItems.Count} dịch vụ phát sinh (+{totalAddedDuration}p) cho khách hàng."
                        }
                    );
                }
            }
            var response = _mapper.Map<List<BookingProcedureResponseDTO>>(createdProcedures);
            return new ApiSuccessResult<List<BookingProcedureResponseDTO>>(response, $"Thêm {request.AddonItems.Count} dịch vụ phát sinh thành công!");
        }
    }
}
