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
                var prevProcedure = allProcedures.FirstOrDefault(p => p.StepOrder == procedure.StepOrder - 1);

                if (prevProcedure != null && prevProcedure.Status != BookingProcedureStatus.Completed)
                {
                    return new ApiErrorResult<BookingProcedureResponseDTO>($"Không thể bắt đầu bước này. Bước trước đó '{prevProcedure.ProcedureName}' chưa hoàn thành.");
                }
            }

            procedure.Status = BookingProcedureStatus.InProgress;
            procedure.CompletedById = artistId;
            procedure.CompletedAt = null;

            _unitOfWork.BookingProcedureRepository.Update(procedure);
            await _unitOfWork.SaveChangesAsync();

            var updatedProcs = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingItemIdAsync(procedure.BookingItemId);
            var targetProc = updatedProcs.First(x => x.BookingProcedureId == bookingProcedureId);
            var response = _mapper.Map<BookingProcedureResponseDTO>(targetProc);
            return new ApiSuccessResult<BookingProcedureResponseDTO>(response, "Nhận công đoạn thành công. Hãy bắt đầu phục vụ.");
        }

        public async Task<ApiResult<bool>> DuplicateProceduresForBookingItemAsync(Guid bookingItemId, int nailVariantId)
        {
            var activeNailProcedures = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(nailVariantId);
            if (!activeNailProcedures.Any())
            {
                return new ApiSuccessResult<bool>(false, "Không tìm thấy cấu hình quy trình mẫu cho biến thể nail này.");
            }
            foreach (var y in activeNailProcedures)
            {
                var x = new BookingProcedure
                {
                    BookingItemId = bookingItemId,
                    ProcedureId = y.ProcedureId,
                    ProcedureName = y.Procedure.Name,
                    Description = y.Procedure.Description,
                    StepOrder = y.StepOrder,
                    Status = BookingProcedureStatus.Pending,
                    IsRequired = y.Procedure.IsRequired
                };
                await _unitOfWork.BookingProcedureRepository.CreateAsync(x);
                await _unitOfWork.SaveChangesAsync();
            }
            return new ApiSuccessResult<bool>(true, "Sao chép quy trình thành công.");
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
                existbooking.CompletedById = artistId;
            }
            else if (status == BookingProcedureStatus.InProgress)
            {
                existbooking.CompletedAt = null;
                existbooking.CompletedById = artistId;
            }
            else
            {
                existbooking.CompletedAt = null;
                existbooking.CompletedById = null;
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
