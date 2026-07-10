using AutoMapper;
using MediatR;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Helpers;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
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
    public class BookingRescheduleService : IBookingRescheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBookingSchedulingService _bookingSchedulingService;
        public BookingRescheduleService(IUnitOfWork unitOfWork, IMapper mapper, IBookingSchedulingService bookingSchedulingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookingSchedulingService = bookingSchedulingService;
        }
        private async Task<ApiResult<bool>> ValidateRescheduleSlotAsync(Booking booking, DateTime newDate, TimeSpan newTime)
        {

            var localDate = (newDate.Kind == DateTimeKind.Utc ? newDate.AddHours(7) : newDate).Date;
            var isOffDay = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x =>
                x.SalonId == booking.SalonId
                && x.StartDate.Date <= localDate
                && x.EndDate.Date >= localDate);
            if (isOffDay)
            {
                return new ApiErrorResult<bool>("Salon nghỉ vào ngày này. Vui lòng chọn ngày khác.");
            }
            var dayOfWeek = (int)localDate.DayOfWeek;
            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(booking.SalonId);
            var operatingHours = salon?.OperatingHours?.Where(x => x.DayOfWeek == dayOfWeek).ToList() ?? new List<SalonOperatingHour>();
            var targetEndTime = newTime.Add(TimeSpan.FromMinutes(booking.TotalDuration));
            if (!operatingHours.IsWithinOperatingHours(newTime, targetEndTime))
            {
                return new ApiErrorResult<bool>("Giờ hẹn không thuộc khung giờ hoạt động của Salon.");
            }
            if (booking.NailArtistId.HasValue)
            {
                var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(booking.NailArtistId.Value);
                if (artist == null || artist.Status != "Active")
                {
                    return new ApiErrorResult<bool>("Thợ làm móng không tồn tại hoặc không hoạt động.");
                }
                var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(booking.NailArtistId.Value, newDate);
                if (schedule == null || newTime < schedule.ShiftStart || targetEndTime > schedule.ShiftEnd)
                {
                    return new ApiErrorResult<bool>("Thợ làm móng không có lịch làm việc trong khung giờ này.");
                }

                var artistBreaks = await _unitOfWork.NailArtistBreakRepository.GetApprovedBreaksByArtistAndDateAsync(booking.NailArtistId.Value, newDate);
                bool overlapsBreak = artistBreaks.Any(b => newTime < b.EndTime && targetEndTime > b.StartTime);
                if (overlapsBreak)
                {
                    return new ApiErrorResult<bool>("Thợ làm móng bận lịch nghỉ cá nhân.");
                }
                var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId);
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures.ToList(), newTime);
                var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                    booking.NailArtistId.Value,
                    newDate,
                    timeline,
                    artist.ConcurrentCapacity
                );
                if (isConflict)
                {
                    return new ApiErrorResult<bool>("Thợ đã kín lịch làm việc trong khung giờ này.");
                }
            }
            return new ApiSuccessResult<bool>(true);
        }
        public async Task<ApiResult<BookingResponseDTO>> CustomerAcceptSuggestedTimeAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin lịch hẹn.");
            }
            if (booking.CustomerId != customerId)
            {
                return new ApiErrorResult<BookingResponseDTO>("Bạn không có quyền thực hiện hành động này.");
            }
            if (booking.Status != BookingStatus.RescheduleSuggested || booking.ProposedBy != "Manager")
            {
                return new ApiErrorResult<BookingResponseDTO>("Không có đề xuất giờ hẹn nào từ phía Salon.");
            }
            if (!booking.ProposedBookingDate.HasValue || !booking.ProposedStartTime.HasValue)
            {
                return new ApiErrorResult<BookingResponseDTO>("Thông tin giờ đề xuất bị thiếu.");
            }
            var validation = await ValidateRescheduleSlotAsync(booking, booking.ProposedBookingDate.Value, booking.ProposedStartTime.Value);
            if (!validation.IsSucceeded)
            {
                return new ApiErrorResult<BookingResponseDTO>(validation.Message);
            }
            booking.AcceptReschedule(customerId);
            _unitOfWork.BookingRepository.Update(booking);
            // Cập nhật timeline cho BookingProcedures tương ứng
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId, trackChanges: true);
            if (procedures.Any())
            {
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures.ToList(), booking.StartTime);
                foreach (var segment in timeline)
                {
                    var procedure = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                    procedure.EstimatedStartTime = segment.StartTime;
                    procedure.EstimatedEndTime = segment.EndTime;
                    _unitOfWork.BookingProcedureRepository.Update(procedure);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Chấp nhận đề xuất giờ hẹn mới thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CustomerDeclineSuggestedTimeAsync(Guid bookingId, Guid customerId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin lịch hẹn.");
            }
            if (booking.CustomerId != customerId)
            {
                return new ApiErrorResult<BookingResponseDTO>("Bạn không có quyền thực hiện hành động này.");
            }
            if (booking.Status != BookingStatus.RescheduleSuggested || booking.ProposedBy != "Manager")
            {
                return new ApiErrorResult<BookingResponseDTO>("Không có đề xuất giờ hẹn nào từ phía Salon để từ chối.");
            }
            booking.DeclineReschedule(customerId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Từ chối đề xuất giờ hẹn mới thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CustomerRequestRescheduleAsync(Guid bookingId, CustomerRescheduleRequestDTO request, Guid customerId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin lịch hẹn.");
            }
            if (booking.CustomerId != customerId)
            {
                return new ApiErrorResult<BookingResponseDTO>("Bạn không có quyền yêu cầu đổi lịch hẹn này.");
            }
            if (booking.Status != BookingStatus.Approved)
            {
                return new ApiErrorResult<BookingResponseDTO>("Chỉ có thể đổi lịch đối với đơn đã được xác nhận (Approved).");
            }
            var validation = await ValidateRescheduleSlotAsync(booking, request.NewDate, request.NewTime);
            if (!validation.IsSucceeded)
            {
                return new ApiErrorResult<BookingResponseDTO>(validation.Message);
            }
            booking.RequestReschedule(request.NewDate, request.NewTime, request.Reason, customerId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Gửi yêu cầu đổi lịch hẹn thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ManagerApproveRescheduleAsync(Guid bookingId, Guid managerId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin lịch hẹn.");
            }
            if (booking.Status != BookingStatus.ReschedulePending || booking.ProposedBy != "Customer")
            {
                return new ApiErrorResult<BookingResponseDTO>("Không có yêu cầu đổi lịch từ khách hàng cần duyệt.");
            }
            if (!booking.ProposedBookingDate.HasValue || !booking.ProposedStartTime.HasValue)
            {
                return new ApiErrorResult<BookingResponseDTO>("Thông tin giờ đề xuất bị thiếu.");
            }
            var validation = await ValidateRescheduleSlotAsync(booking, booking.ProposedBookingDate.Value, booking.ProposedStartTime.Value);
            if (!validation.IsSucceeded)
            {
                return new ApiErrorResult<BookingResponseDTO>(validation.Message);
            }
            booking.AcceptReschedule(managerId);
            _unitOfWork.BookingRepository.Update(booking);
            // Cập nhật timeline cho BookingProcedures tương ứng
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId, trackChanges: true);
            if (procedures.Any())
            {
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures.ToList(), booking.StartTime);
                foreach (var segment in timeline)
                {
                    var procedure = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                    procedure.EstimatedStartTime = segment.StartTime;
                    procedure.EstimatedEndTime = segment.EndTime;
                    _unitOfWork.BookingProcedureRepository.Update(procedure);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Duyệt yêu cầu đổi lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ManagerRejectRescheduleAsync(Guid bookingId, Guid managerId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin lịch hẹn.");
            }
            if (booking.Status != BookingStatus.ReschedulePending || booking.ProposedBy != "Customer")
            {
                return new ApiErrorResult<BookingResponseDTO>("Không có yêu cầu đổi lịch từ khách hàng để từ chối.");
            }
            booking.DeclineReschedule(managerId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Từ chối yêu cầu đổi lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ManagerSuggestTimeAsync(Guid bookingId, ManagerSuggestTimeRequestDTO request, Guid managerId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin lịch hẹn.");
            }
            if (booking.Status != BookingStatus.ReschedulePending)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn hàng không nằm trong trạng thái chờ đổi lịch.");
            }
            var validation = await ValidateRescheduleSlotAsync(booking, request.SuggestedDate, request.SuggestedTime);
            if (!validation.IsSucceeded)
            {
                return new ApiErrorResult<BookingResponseDTO>(validation.Message);
            }
            booking.SuggestAlternativeTime(request.SuggestedDate, request.SuggestedTime, request.Reason, managerId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Đề xuất giờ hẹn thay thế thành công.");
        }
    }
}
