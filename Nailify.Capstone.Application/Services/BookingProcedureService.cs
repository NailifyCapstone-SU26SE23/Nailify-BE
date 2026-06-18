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
