using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class BookingHistoryService : IBookingHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<BookingHistoryResponseDTO>>> GetPagedBookingHistoriesAsync(int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var pagedHistories = await _unitOfWork.BookingHistoryRepository.GetPagedBookingHistoriesAsync(pageNumber, pageSize, startDate, endDate);
            var response = MapPagedHistories(pagedHistories, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingHistoryResponseDTO>>(response, "Lay danh sach lich su dat lich thanh cong.");
        }

        public async Task<ApiResult<BookingHistoryResponseDTO>> GetBookingHistoryByIdAsync(Guid bookingHistoryId)
        {
            var history = await _unitOfWork.BookingHistoryRepository.GetBookingHistoryDetailAsync(bookingHistoryId);
            if (history == null)
            {
                return new ApiErrorResult<BookingHistoryResponseDTO>("Khong tim thay lich su dat lich.");
            }

            var response = _mapper.Map<BookingHistoryResponseDTO>(history);
            return new ApiSuccessResult<BookingHistoryResponseDTO>(response, "Lay thong tin lich su dat lich thanh cong.");
        }

        public async Task<ApiResult<PagedList<BookingHistoryResponseDTO>>> GetPagedBookingHistoriesByBookingIdAsync(Guid bookingId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var pagedHistories = await _unitOfWork.BookingHistoryRepository.GetPagedBookingHistoriesByBookingIdAsync(bookingId, pageNumber, pageSize, startDate, endDate);
            var response = MapPagedHistories(pagedHistories, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingHistoryResponseDTO>>(response, "Lay lich su dat lich theo don dat lich thanh cong.");
        }

        public async Task<ApiResult<PagedList<BookingHistoryResponseDTO>>> GetPagedBookingHistoriesBySalonIdAsync(Guid salonId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var pagedHistories = await _unitOfWork.BookingHistoryRepository.GetPagedBookingHistoriesBySalonIdAsync(salonId, pageNumber, pageSize, startDate, endDate);
            var response = MapPagedHistories(pagedHistories, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingHistoryResponseDTO>>(response, "Lay lich su dat lich theo salon thanh cong.");
        }

        public async Task<ApiResult<PagedList<BookingHistoryResponseDTO>>> GetPagedBookingHistoriesByArtistIdAsync(Guid artistId, int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var pagedHistories = await _unitOfWork.BookingHistoryRepository.GetPagedBookingHistoriesByArtistIdAsync(artistId, pageNumber, pageSize, startDate, endDate);
            var response = MapPagedHistories(pagedHistories, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<BookingHistoryResponseDTO>>(response, "Lay lich su dat lich theo tho lam mong thanh cong.");
        }

        private PagedList<BookingHistoryResponseDTO> MapPagedHistories(PagedList<BookingHistory> pagedHistories, int pageNumber, int pageSize)
        {
            var mappedItems = _mapper.Map<List<BookingHistoryResponseDTO>>(pagedHistories.Items);
            return new PagedList<BookingHistoryResponseDTO>(mappedItems, pagedHistories.MetaData.TotalItems, pageNumber, pageSize);
        }
    }
}
