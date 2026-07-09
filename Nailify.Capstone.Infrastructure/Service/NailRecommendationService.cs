using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class NailRecommendationService : INailRecommendationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NailRecommendationService> _logger;
        private readonly string _modelDirectory;
        private readonly string _modelPath;
        private static ITransformer? _cachedModel;
        private static MLContext? _cachedContext;

        public NailRecommendationService(IServiceScopeFactory scopeFactory, ILogger<NailRecommendationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _modelDirectory = Path.Combine(AppContext.BaseDirectory, "MLModels");
            _modelPath = Path.Combine(_modelDirectory, "NailRecommendationModel.zip");
        }

        public async Task<ApiResult<ModelTrainingResultDto>> TrainModelAsync()
        {
            try
            {
                var data = await BuildRatingDataAsync();
                if (data.Count < 10)
                {
                    return new ApiErrorResult<ModelTrainingResultDto>("Chưa đủ dữ liệu để train (cần ít nhất 10 tương tác).");
                }
                var mlContext = new MLContext(seed: 42);
                var dataView = mlContext.Data.LoadFromEnumerable(data);
                var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
                var pipeline = mlContext.Transforms.Conversion
                    .MapValueToKey("userEncoded", "UserId")
                    .Append(mlContext.Transforms.Conversion.MapValueToKey("variantEncoded", "NailVariantId"))
                    .Append(mlContext.Recommendation().Trainers.MatrixFactorization(
                        new MatrixFactorizationTrainer.Options
                        {
                            MatrixColumnIndexColumnName = "userEncoded",
                            MatrixRowIndexColumnName = "variantEncoded",
                            LabelColumnName = "Label",
                            NumberOfIterations = 20,
                            ApproximationRank = 100
                        }));
                _logger.LogInformation("Huấn luyện mô hình với {Count} mẫu...", data.Count);
                var model = pipeline.Fit(split.TrainSet);
                // Evaluate
                var predictions = model.Transform(split.TestSet);
                var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
                // Save model
                if (!Directory.Exists(_modelDirectory))
                {
                    Directory.CreateDirectory(_modelDirectory);
                }
                mlContext.Model.Save(model, dataView.Schema, _modelPath);
                _cachedModel = model;   // cache in memory
                _cachedContext = mlContext;
                var resultDto = new ModelTrainingResultDto
                {
                    TrainedAt = DateTime.UtcNow,
                    TotalSamples = data.Count,
                    UniqueUsers = data.Select(x => x.UserId).Distinct().Count(),
                    UniqueVariants = data.Select(x => x.NailVariantId).Distinct().Count(),
                    RmseScore = Math.Round(metrics.RootMeanSquaredError, 4),
                    ModelPath = _modelPath
                };
                return new ApiSuccessResult<ModelTrainingResultDto>(resultDto, "Train model thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình huấn luyện mô hình.");
                return new ApiErrorResult<ModelTrainingResultDto>($"Lỗi train model: {ex.Message}");
            }
        }

        public async Task<ApiResult<List<NailVariantRecommendationDto>>> GetRecommendationsAsync(Guid userId, int topN = 10)
        {
            try
            {
                var userInteractionCount = await CountUserInteractionsAsync(userId);
                if (userInteractionCount < 3 || !File.Exists(_modelPath))
                {
                    return await GetPopularAsync(topN); // fallback
                }
                var (mlContext, model) = LoadOrGetCachedModel();
                var engine = mlContext.Model.CreatePredictionEngine<NailRatingInput, NailRatingPrediction>(model);
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var allVariants = await unitOfWork.NailVariantRepository.GetAllActiveVariantAsync();
                var bookedVariantIds = await GetBookedVariantIdsAsync(userId);
                var userEncoded = Math.Abs(userId.GetHashCode()) % 1_000_000f;
                var recommendations = allVariants
                    .Where(v => !bookedVariantIds.Contains(v.NailVariantId))
                    .Select(v => new
                    {
                        Variant = v,
                        Score = engine.Predict(new NailRatingInput
                        {
                            UserId = userEncoded,
                            NailVariantId = v.NailVariantId
                        }).Score
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(topN)
                    .Select(x => new NailVariantRecommendationDto
                    {
                        NailVariantId = x.Variant.NailVariantId,
                        Name = x.Variant.Name,
                        NailDesignId = x.Variant.NailDesignId,
                        NailDesignName = x.Variant.NailDesign?.Name ?? string.Empty,
                        ImageUrl = x.Variant.ImageUrl,
                        Price = x.Variant.Price,
                        PredictedScore = MathF.Round(x.Score, 2),
                        IsFallback = false
                    })
                    .ToList();
                return new ApiSuccessResult<List<NailVariantRecommendationDto>>(recommendations, $"Gợi ý {recommendations.Count} mẫu nail cho bạn.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách gợi ý cho user {UserId}", userId);
                return await GetPopularAsync(topN);
            }
        }

        public async Task<ApiResult<List<NailVariantRecommendationDto>>> GetPopularAsync(int topN = 10)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var bookingCounts = await unitOfWork.BookingItemRepository
                                                    .GetBookingCountsByVariantAsync();
                var favoriteCounts = await unitOfWork.FavoriteNailRepository
                                                     .GetFavoriteCountsByVariantAstbc();
                var activeVariants = await unitOfWork.NailVariantRepository.GetAllActiveVariantAsync();
                var popularList = activeVariants.Select(v =>
                {
                    bookingCounts.TryGetValue(v.NailVariantId, out int bCount);
                    favoriteCounts.TryGetValue(v.NailVariantId, out int fCount);
                    float popularityScore = (bCount * 2f) + fCount;
                    return new NailVariantRecommendationDto
                    {
                        NailVariantId = v.NailVariantId,
                        Name = v.Name,
                        NailDesignId = v.NailDesignId,
                        NailDesignName = v.NailDesign?.Name ?? string.Empty,
                        ImageUrl = v.ImageUrl,
                        Price = v.Price,
                        PredictedScore = popularityScore,
                        IsFallback = true
                    };
                })
                .OrderByDescending(x => x.PredictedScore)
                .Take(topN)
                .ToList();
                return new ApiSuccessResult<List<NailVariantRecommendationDto>>(popularList, "Lấy danh sách phổ biến thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular recommendations.");
                return new ApiErrorResult<List<NailVariantRecommendationDto>>($"Lỗi lấy danh sách phổ biến: {ex.Message}");
            }
        }
        private async Task<List<NailRatingInput>> BuildRatingDataAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var ratings = new Dictionary<(Guid userId, int variantId), float>();

            // Nguồn : BookingItem từ completed bookings
            var completedBookingItems = await unitOfWork.BookingItemRepository.GetCompletedBookingItemsWithVariantAsync();
            foreach(var item in completedBookingItems.Where(x => x.NailVariantId.HasValue))
            {
                var key = (item.Booking.CustomerId, item.NailVariantId!.Value);
                ratings[key] = Math.Max(ratings.GetValueOrDefault(key, 0f), 3.0f);
            }

            // Nguồn: FavoriteNails
            var favorites = await unitOfWork.FavoriteNailRepository.GetAllWithVariantAsync();
            foreach(var favorite in favorites.Where(x => x.NailVariantId.HasValue))
            {
                var key = (favorite.UserId, favorite.NailVariantId!.Value);
                ratings[key] = Math.Max(ratings.GetValueOrDefault(key, 0f), 2.0f);
            }

            // Nguồn: BookingRatings (join qua Booking -> BookingItems)
            var bookingRatings = await unitOfWork.BookingRatingRepository.GetAllWithBookingItemsAsync();
            foreach(var rating in bookingRatings)
            {
                foreach (var item in rating.Booking.BookingItems.Where(x => x.NailVariantId.HasValue))
                {
                    var key = (rating.CustomerId, item.NailVariantId!.Value);
                    var ratingVal = (float)rating.OverallScore;
                    ratings[key] = Math.Max(ratings.GetValueOrDefault(key, 0f), ratingVal);
                }
            }
            // Convert sang ML.NET input format
            return ratings.Select(kv => new NailRatingInput
            {
                UserId = Math.Abs(kv.Key.userId.GetHashCode()) % 1_000_000f,
                NailVariantId = kv.Key.variantId,
                Label = kv.Value
            }).ToList();
        }
        private (MLContext, ITransformer) LoadOrGetCachedModel()
        {
            if (_cachedModel != null && _cachedContext != null)
            {
                return (_cachedContext, _cachedModel);
            }
            var mlContext = new MLContext(seed: 42);
            if (File.Exists(_modelPath))
            {
                try
                {
                    var model = mlContext.Model.Load(_modelPath, out _);
                    _cachedModel = model;
                    _cachedContext = mlContext;
                    return (mlContext, model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi load model từ file {Path}", _modelPath);
                    throw;
                }
            }
            else
            {
                throw new FileNotFoundException("Không tìm thấy file model và không có model cache.", _modelPath);
            }
        }
        private async Task<int> CountUserInteractionsAsync(Guid userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var bookingCount = await unitOfWork.BookingRepository.CountBookingsByCustomerIdAsync(userId);
            var favoriteCount = await unitOfWork.FavoriteNailRepository.CountFavoritesWithVariantByUserIdAsync(userId);
            return bookingCount + favoriteCount;
        }
        private async Task<HashSet<int>> GetBookedVariantIdsAsync(Guid userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var ids = await unitOfWork.BookingItemRepository
                                      .GetBookedVariantIdsByCustomerIdAsync(userId);
            return ids;
        }
    }
}
