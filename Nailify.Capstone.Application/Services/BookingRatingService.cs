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
        private readonly ISentimentAnalysisService _sentimentAnalysisService;
        private readonly INotificationService _notificationService;

        public BookingRatingService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            ISentimentAnalysisService sentimentAnalysisService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sentimentAnalysisService = sentimentAnalysisService;
            _notificationService = notificationService;
        }

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetAllAsync(BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetPagedAsync(parameters), parameters, "Lấy danh sách đánh giá thành công.");

        public async Task<ApiResult<BookingRatingResponseDTO>> GetByIdAsync(Guid id)
        {
            var rating = await _unitOfWork.BookingRatingRepository.GetDetailByIdAsync(id);
            return rating == null
                ? new ApiErrorResult<BookingRatingResponseDTO>("Không tìm thấy đánh giá.")
                : new ApiSuccessResult<BookingRatingResponseDTO>(_mapper.Map<BookingRatingResponseDTO>(rating), "Lấy thông tin đánh giá thành công.");
        }

        public async Task<ApiResult<BookingRatingResponseDTO>> GetByBookingIdAsync(Guid bookingId)
        {
            var rating = await _unitOfWork.BookingRatingRepository.GetByBookingIdAsync(bookingId);
            return rating == null
                ? new ApiErrorResult<BookingRatingResponseDTO>("Không tìm thấy đánh giá.")
                : new ApiSuccessResult<BookingRatingResponseDTO>(_mapper.Map<BookingRatingResponseDTO>(rating), "Lấy thông tin đánh giá thành công.");
        }

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetBySalonIdAsync(Guid salonId, BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetBySalonIdAsync(salonId, parameters), parameters, "Lấy thông tin đánh giá thành công.");

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByNailArtistIdAsync(Guid nailArtistId, BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetByNailArtistIdAsync(nailArtistId, parameters), parameters, "Lấy thông tin đánh giá thành công.");

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByNailVariantIdAsync(int nailVariantId, BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetByNailVariantIdAsync(nailVariantId, parameters), parameters, "Lấy thông tin đánh giá thành công.");

        public async Task<ApiResult<PagedList<BookingRatingResponseDTO>>> GetByCustomerIdAsync(Guid customerId, BookingRatingRequestParameters parameters)
            => SuccessPaged(await _unitOfWork.BookingRatingRepository.GetByCustomerIdAsync(customerId, parameters), parameters, "Lấy thông tin đánh giá thành công.");

        public async Task<ApiResult<BookingRatingResponseDTO>> CreateAsync(Guid customerId, BookingRatingCreateRequest request, string? imageUrl)
        {
            var validationError = ValidateScores(request.OverallScore, request.ServiceQuality, request.Punctuality, request.Cleanliness);
            if (validationError != null) return new ApiErrorResult<BookingRatingResponseDTO>(validationError);

            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId);
            if (booking == null) return new ApiErrorResult<BookingRatingResponseDTO>("Không tìm thấy lịch hẹn.");
            if (booking.CustomerId != customerId) return new ApiErrorResult<BookingRatingResponseDTO>("Bạn chỉ được đánh giá cho lịch hẹn của mình.");
            if (booking.Status != BookingStatus.Completed) return new ApiErrorResult<BookingRatingResponseDTO>("Chỉ lịch hẹn hoàn thành mới được đánh giá.");

            var existingRating = await _unitOfWork.BookingRatingRepository.GetByBookingIdAsync(request.BookingId, includeDeleted: true);
            if (existingRating?.DeletedAt != null) return new ApiErrorResult<BookingRatingResponseDTO>("Lịch hẹn này đã xóa đánh giá.");
            if (existingRating != null) return new ApiErrorResult<BookingRatingResponseDTO>("Lịch hẹn này đã được đánh giá.");

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
            booking.IsRated = true;
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.BookingRatingRepository.GetDetailByIdAsync(rating.BookingRatingId);

            var dto = _mapper.Map<BookingRatingResponseDTO>(created);

            // Gọi Python AI Microservice phân tích cảm xúc nhận xét
            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                bool isNegative = await _sentimentAnalysisService.IsNegativeReviewAsync(request.Comment);
                dto.IsNegativeReview = isNegative;
                dto.SentimentScore = isNegative ? "NEGATIVE (Tiêu cực)" : "POSITIVE (Tích cực)";

                if (isNegative || request.OverallScore <= 2)
                {
                    // Bắn cảnh báo SignalR cho Salon Manager về đánh giá tiêu cực
                    await _notificationService.SendNotificationToSalonStaffAsync(
                        booking.SalonId.ToString(),
                        "NegativeReviewAlert",
                        new
                        {
                            BookingId = booking.BookingId,
                            CustomerId = customerId,
                            Stars = request.OverallScore,
                            Comment = request.Comment,
                            Message = $"CẢNH BÁO: Đánh giá tiêu cực vừa được gửi từ khách hàng cho đơn #{booking.BookingId}!"
                        }
                    );
                }
            }

            return new ApiSuccessResult<BookingRatingResponseDTO>(dto, "Tạo đánh giá thành công.");
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
                return new ApiErrorResult<BookingRatingResponseDTO>("Không có thông tin nào để cập nhật.");
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
                ? "Điểm đánh giá phải nằm trong thang điểm từ 1 đến 5."
                : null;
        }
    }
}
