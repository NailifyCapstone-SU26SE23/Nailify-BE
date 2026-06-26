using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRatingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingRatingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.Services
{
    public class BookingRatingService : IBookingRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingRatingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetAllAsync(BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetPagedAsync(parameters), parameters, "Lay danh sach danh gia thanh cong.");

        public async Task<ApiResult<BookingRatingResponseDTO>> GetByIdAsync(Guid id)
        {
            var rating = await _unitOfWork.BookingRatingRepository.GetDetailByIdAsync(id);
            return rating == null
                ? new ApiErrorResult<BookingRatingResponseDTO>("Khong tim thay danh gia.")
                : new ApiSuccessResult<BookingRatingResponseDTO>(_mapper.Map<BookingRatingResponseDTO>(rating), "Lay danh gia thanh cong.");
        }

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByBookingIdAsync(Guid bookingId, BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetByBookingIdAsync(bookingId, parameters), parameters, "Lay danh gia theo booking thanh cong.");

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetBySalonIdAsync(Guid salonId, BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetBySalonIdAsync(salonId, parameters), parameters, "Lay danh gia theo salon thanh cong.");

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByNailArtistIdAsync(Guid nailArtistId, BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetByNailArtistIdAsync(nailArtistId, parameters), parameters, "Lay danh gia theo tho nail thanh cong.");

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByCustomerIdAsync(Guid customerId, BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetByCustomerIdAsync(customerId, parameters), parameters, "Lay danh gia theo khach hang thanh cong.");

        public async Task<ApiResult<BookingRatingResponseDTO>> CreateAsync(Guid customerId, BookingRatingCreateRequest request, string? imageUrl)
        {
            var validationError = ValidateScores(request.OverallScore, request.ServiceQuality, request.Punctuality, request.Cleanliness);
            if (validationError != null) return new ApiErrorResult<BookingRatingResponseDTO>(validationError);

            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId);
            if (booking == null) return new ApiErrorResult<BookingRatingResponseDTO>("Khong tim thay booking.");
            if (booking.CustomerId != customerId) return new ApiErrorResult<BookingRatingResponseDTO>("Ban chi duoc danh gia booking cua minh.");
            if (booking.Status != BookingStatus.Completed) return new ApiErrorResult<BookingRatingResponseDTO>("Chi booking da hoan thanh moi duoc danh gia.");

            var existingRating = await _unitOfWork.BookingRatingRepository.GetByBookingIdAsync(request.BookingId, includeDeleted: true);
            if (existingRating?.DeletedAt != null) return new ApiErrorResult<BookingRatingResponseDTO>("Booking nay da xoa danh gia, khong the danh gia lai.");
            if (existingRating != null) return new ApiErrorResult<BookingRatingResponseDTO>("Booking nay da duoc danh gia.");

            var rating = new BookingRating
            {
                BookingRatingId = Guid.NewGuid(),
                BookingId = request.BookingId,
                CustomerId = customerId,
                OverallScore = request.OverallScore,
                Comment = request.Comment,
                ImageUrl = imageUrl,
                ServiceQuality = request.ServiceQuality,
                Punctuality = request.Punctuality,
                Cleanliness = request.Cleanliness,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BookingRatingRepository.CreateAsync(rating);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.BookingRatingRepository.GetDetailByIdAsync(rating.BookingRatingId);
            return new ApiSuccessResult<BookingRatingResponseDTO>(_mapper.Map<BookingRatingResponseDTO>(created), "Tao danh gia thanh cong.");
        }

        public async Task<ApiResult<BookingRatingResponseDTO>> UpdateAsync(Guid customerId, Guid id, BookingRatingUpdateRequest request, string? imageUrl)
        {
            var validationError = ValidateScores(request.OverallScore, request.ServiceQuality, request.Punctuality, request.Cleanliness);
            if (validationError != null) return new ApiErrorResult<BookingRatingResponseDTO>(validationError);

            var rating = await _unitOfWork.BookingRatingRepository.GetDetailByIdAsync(id, true);
            if (rating == null) return new ApiErrorResult<BookingRatingResponseDTO>("Không tìm thấy đánh giá.");
            if (rating.CustomerId != customerId) return new ApiErrorResult<BookingRatingResponseDTO>("Bạn chỉ được cập nhật đánh giá của mình.");
            if (rating.DeletedAt != null) return new ApiErrorResult<BookingRatingResponseDTO>("Đánh giá đã bị xóa, không thể cập nhật.");
            if (rating.IsUpdated) return new ApiErrorResult<BookingRatingResponseDTO>("Mỗi đánh giá chỉ được cập nhật một lần.");

            var hasChanges = false;

            if (request.OverallScore.HasValue)
            {
                rating.OverallScore = request.OverallScore.Value;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                rating.Comment = request.Comment;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                rating.ImageUrl = imageUrl;
                hasChanges = true;
            }

            if (request.ServiceQuality.HasValue)
            {
                rating.ServiceQuality = request.ServiceQuality;
                hasChanges = true;
            }

            if (request.Punctuality.HasValue)
            {
                rating.Punctuality = request.Punctuality;
                hasChanges = true;
            }

            if (request.Cleanliness.HasValue)
            {
                rating.Cleanliness = request.Cleanliness;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return new ApiErrorResult<BookingRatingResponseDTO>("Khong co thong tin nao de cap nhat.");
            }

            rating.IsUpdated = true;
            rating.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.BookingRatingRepository.Update(rating);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.BookingRatingRepository.GetDetailByIdAsync(id);
            return new ApiSuccessResult<BookingRatingResponseDTO>(_mapper.Map<BookingRatingResponseDTO>(updated), "Cập nhật đánh giá thành công.");
        }

        public async Task<ApiResult<bool>> DeleteAsync(Guid customerId, Guid id)
        {
            var rating = await _unitOfWork.BookingRatingRepository.GetDetailByIdAsync(id, true);
            if (rating == null) return new ApiErrorResult<bool>("Không tìm thấy đánh giá.");

            rating.Status = "InActive";
            rating.DeletedAt = DateTime.UtcNow;
            rating.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.BookingRatingRepository.Update(rating);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa đánh giá thành công.");
        }

        private ApiResult<PagedList<BookingRatingResponseDTO>> SuccessPaged(PagedList<BookingRating> ratings, BookingRatingRequestParameters parameters, string message)
        {
            var response = new PagedList<BookingRatingResponseDTO>(
                _mapper.Map<List<BookingRatingResponseDTO>>(ratings.Items),
                ratings.MetaData.TotalItems,
                parameters.PageNumber,
                parameters.PageSize);
            return new ApiSuccessResult<PagedList<BookingRatingResponseDTO>>(response, message);
        }

        private static string? ValidateScores(params int?[] scores)
        {
            return scores.Any(score => score.HasValue && (score < 1 || score > 5))
                ? "Diem danh gia phai nam trong khoang 1 den 5."
                : null;
        }
    }
}
