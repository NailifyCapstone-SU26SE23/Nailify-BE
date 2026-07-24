using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ChairRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ChairResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class ChairService : IChairService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChairService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<ChairResponseDTO>> CreateChairAsync(ChairCreateRequest request)
        {
            var salon = await _unitOfWork.SalonRepository.GetByIdAsync(request.SalonId);
            if (salon == null)
            {
                return new ApiErrorResult<ChairResponseDTO>("Không tìm thấy chi nhánh Salon.");
            }

            var chair = _mapper.Map<Chair>(request);
            chair.ChairId = Guid.NewGuid();

            await _unitOfWork.ChairRepository.CreateAsync(chair);
            await _unitOfWork.SaveChangesAsync();

            // Fetch with Salon name for response
            var createdChair = await _unitOfWork.ChairRepository.GetChairWithSalonAsync(chair.ChairId);

            var response = _mapper.Map<ChairResponseDTO>(createdChair);
            return new ApiSuccessResult<ChairResponseDTO>(response, "Tạo ghế thành công.");
        }

        public async Task<ApiResult<bool>> DeleteChairAsync(Guid id)
        {
            var chair = await _unitOfWork.ChairRepository.GetByIdAsync(id);
            if (chair == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy ghế.");
            }

            // check if there are upcoming bookings assigned to this chair
            var hasUpcoming = await _unitOfWork.BookingRepository.ExistsAsync(b =>
                b.ChairId == id &&
                b.BookingDate.Date >= DateTime.UtcNow.AddHours(7).Date &&
                (b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckedIn || b.Status == BookingStatus.InProgress));

            if (hasUpcoming)
            {
                return new ApiErrorResult<bool>("Không thể xóa ghế vì đang được gán cho các lịch hẹn sắp tới.");
            }

            _unitOfWork.ChairRepository.Delete(chair);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa ghế thành công.");
        }

        public async Task<ApiResult<ChairResponseDTO>> GetChairByIdAsync(Guid id)
        {
            var chair = await _unitOfWork.ChairRepository.GetChairWithSalonAsync(id);

            if (chair == null)
            {
                return new ApiErrorResult<ChairResponseDTO>("Không tìm thấy ghế.");
            }

            var response = _mapper.Map<ChairResponseDTO>(chair);
            return new ApiSuccessResult<ChairResponseDTO>(response, "Lấy thông tin ghế thành công.");
        }

        public async Task<ApiResult<PagedList<ChairResponseDTO>>> GetChairsBySalonAsync(Guid salonId, PagingRequestParameters parameters)
        {
            var salon = await _unitOfWork.SalonRepository.GetByIdAsync(salonId);
            if (salon == null)
            {
                return new ApiErrorResult<PagedList<ChairResponseDTO>>("Không tìm thấy chi nhánh Salon.");
            }

            var pagedChairs = await _unitOfWork.ChairRepository.GetPagedChairsBySalonAsync(
                salonId,
                parameters.PageIndex,
                parameters.PageSize
            );

            var mappedItems = _mapper.Map<List<ChairResponseDTO>>(pagedChairs.Items);
            var response = new PagedList<ChairResponseDTO>(
                mappedItems,
                pagedChairs.MetaData.TotalItems,
                pagedChairs.MetaData.CurrentPage,
                pagedChairs.MetaData.PageSize
            );

            return new ApiSuccessResult<PagedList<ChairResponseDTO>>(response, "Lấy danh sách ghế thành công.");
        }

        public async Task<ApiResult<ChairResponseDTO>> UpdateChairAsync(Guid id, ChairUpdateRequest request)
        {
            var chair = await _unitOfWork.ChairRepository.GetChairWithSalonAsync(id);

            if (chair == null)
            {
                return new ApiErrorResult<ChairResponseDTO>("Không tìm thấy ghế.");
            }

            _mapper.Map(request, chair);
            _unitOfWork.ChairRepository.Update(chair);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ChairResponseDTO>(chair);
            return new ApiSuccessResult<ChairResponseDTO>(response, "Cập nhật thông tin ghế thành công.");
        }

        public async Task<ApiResult<List<ChairResponseDTO>>> GetAvailableChairsAsync(Guid salonId, DateTime bookingDate, TimeSpan startTime, int durationMinutes)
        {
            var salon = await _unitOfWork.SalonRepository.GetByIdAsync(salonId);
            if (salon == null)
            {
                return new ApiErrorResult<List<ChairResponseDTO>>("Không tìm thấy chi nhánh Salon.");
            }

            // 1. Get all Active chairs in this salon
            var activeChairs = await _unitOfWork.ChairRepository.GetActiveChairsBySalonAsync(salonId);

            // 2. Get active bookings for this salon on this day
            var bookings = await _unitOfWork.BookingRepository.GetActiveBookingsWithChairsBySalonAndDateAsync(salonId, bookingDate);

            // 3. Find overlapping bookings in memory
            var requestedStart = startTime;
            var requestedEnd = startTime.Add(TimeSpan.FromMinutes(durationMinutes));

            var occupiedChairIds = new HashSet<Guid>();
            foreach (var b in bookings)
            {
                var bStart = b.StartTime;
                var bEnd = b.StartTime.Add(TimeSpan.FromMinutes(b.TotalDuration));

                // Overlap check
                if (bStart < requestedEnd && requestedStart < bEnd)
                {
                    if (b.ChairId.HasValue)
                    {
                        occupiedChairIds.Add(b.ChairId.Value);
                    }
                }
            }

            // 4. Filter out occupied chairs
            var availableChairs = activeChairs.Where(c => !occupiedChairIds.Contains(c.ChairId)).ToList();

            var response = _mapper.Map<List<ChairResponseDTO>>(availableChairs);
            return new ApiSuccessResult<List<ChairResponseDTO>>(response, "Lấy danh sách ghế trống thành công.");
        }
    }
}
