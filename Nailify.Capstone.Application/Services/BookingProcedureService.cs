using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Application.Exceptions;
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

        public BookingProcedureService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
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
                && (!x.ActualStartTime.HasValue || (DateTime.UtcNow - x.ActualStartTime.Value).TotalMinutes < x.ActiveDuration)
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
                            var activeTime = DateTime.UtcNow - x.ActualStartTime.Value;
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
            procedure.ActualStartTime = DateTime.UtcNow;
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
            if (item.NailVariantId.HasValue)
            {
                var activeNailProcedures = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(item.NailVariantId.Value);
                foreach (var x in activeNailProcedures.OrderBy(x => x.StepOrder))
                {
                    var bookingProcedure = new BookingProcedure
                    {
                        BookingItemId = item.BookingItemId,
                        ProcedureId = x.ProcedureId,
                        ProcedureName = x.Procedure.Name,
                        StepOrder = x.StepOrder,
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

            else if (item.ServiceId.HasValue)
            {
                var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                if (service != null)
                {
                    var bookingProcedure = new BookingProcedure
                    {
                        BookingItemId = item.BookingItemId,
                        ProcedureName = service.Name,
                        StepOrder = 1,
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

            else if (item.CustomerNailRequestId.HasValue)
            {
                var customNailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(item.CustomerNailRequestId.Value);
                var customNail = customNailRequest == null
                    ? null
                    : await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);

                var activeNailProcedures = customNail != null
                    ? await _unitOfWork.NailProcedureRepository.GetActiveProceduresByCustomerNailIdAsync(customNail.CustomerNailId)
                    : new List<NailProcedure>();

                if (activeNailProcedures != null && activeNailProcedures.Any())
                {
                    foreach (var x in activeNailProcedures.OrderBy(x => x.StepOrder))
                    {
                        var bookingProcedure = new BookingProcedure
                        {
                            BookingItemId = item.BookingItemId,
                            ProcedureId = x.ProcedureId,
                            ProcedureName = x.Procedure.Name,
                            StepOrder = x.StepOrder,
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
                else
                {
                    int duration = 60;
                    if (customNailRequest != null && customNailRequest.Duration.HasValue)
                    {
                        duration = customNailRequest.Duration.Value;
                    }
                    else if (customNail != null)
                    {
                        duration = customNail.Duration ?? 60;
                    }
                    var bookingProcedure = new BookingProcedure
                    {
                        BookingItemId = item.BookingItemId,
                        ProcedureName = customNail?.Name ?? "Mẫu móng custom",
                        StepOrder = 1,
                        Duration = duration,
                        ActiveDuration = duration,
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
                    && (!x.ActualStartTime.HasValue || (DateTime.UtcNow - x.ActualStartTime.Value).TotalMinutes < x.ActiveDuration)
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
                                var activeTime = DateTime.UtcNow - x.ActualStartTime.Value;
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
                existbooking.CompletedAt = DateTime.UtcNow;
                existbooking.ActualEndTime = DateTime.UtcNow;
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
                existbooking.ActualStartTime = DateTime.UtcNow;
                existbooking.AssignedArtistId = artistId;
                existbooking.CompletedById = null;
            }
            else if (status == BookingProcedureStatus.Pending)
            {
                existbooking.CompletedAt = null;
                existbooking.ActualStartTime = null;
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
    }
}
