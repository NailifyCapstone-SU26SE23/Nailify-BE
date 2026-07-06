using AutoMapper;
using Nailify.Capstone.Application.Common;
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
    public class BookingProcedureService : IBookingProcedureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingProcedureService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<BookingProcedureResponseDTO>> ClaimProcedureStepAsync(Guid bookingProcedureId, Guid accountId)
        {
            var procedure = await _unitOfWork.BookingProcedureRepository.GetByIdAsync(bookingProcedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Không tìm thấy bước quy trình yêu cầu.");
            }

            // Tự động tìm thông tin Thợ từ AccountId người dùng đang đăng nhập
            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistByAccountIdAsync(accountId);
            if (artist == null)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Tài khoản đăng nhập không liên kết với thợ nail nào.");
            }

            var artistId = artist.NailArtistId;
            if (artist.ConcurrentCapacity == 1)
            {
                // Kiểm tra xem thợ này có đang làm dở công đoạn nào khác (InProgress) hay không
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

            _unitOfWork.BookingProcedureRepository.Update(procedure);
            await _unitOfWork.SaveChangesAsync();

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
                    Status = BookingProcedureStatus.Pending
                };
                await _unitOfWork.BookingProcedureRepository.CreateAsync(bookingProcedure);
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
            var existbooking = await _unitOfWork.BookingProcedureRepository.GetByIdAsync(bookingProcedureId);
            if (existbooking == null)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Không tìm thấy bước quy trình yêu cầu.");
            }
            var existartist = await _unitOfWork.NailArtistRepository.ExistsAsync(a => a.NailArtistId == artistId);
            if (!existartist)
            {
                return new ApiErrorResult<BookingProcedureResponseDTO>("Không tìm thấy thông tin thợ nail.");
            }

            existbooking.Status = status;
            if (status == BookingProcedureStatus.Completed)
            {
                existbooking.CompletedAt = DateTime.UtcNow;
                existbooking.ActualEndTime = DateTime.UtcNow;
                existbooking.CompletedById = artistId;
            }
            else if (status == BookingProcedureStatus.InProgress)
            {
                existbooking.CompletedAt = null;
                existbooking.ActualStartTime = DateTime.UtcNow; // Ghi nhận thời gian thực tế bắt đầu
                existbooking.AssignedArtistId = artistId;
                existbooking.CompletedById = null;
            }
            else
            {
                existbooking.CompletedAt = null;
                existbooking.CompletedById = null;
                existbooking.ActualStartTime = null;
                existbooking.ActualEndTime = null;
            }
            _unitOfWork.BookingProcedureRepository.Update(existbooking);
            await _unitOfWork.SaveChangesAsync();
            var updatedProc = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingItemIdAsync(existbooking.BookingItemId);
            var mapper = updatedProc.First(x => x.BookingProcedureId == bookingProcedureId);
            var response = _mapper.Map<BookingProcedureResponseDTO>(mapper);
            return new ApiSuccessResult<BookingProcedureResponseDTO>(response, "Cập nhật trạng thái bước quy trình thành công.");
        }
    }
}
