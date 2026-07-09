using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
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
    public class DurationPredictionService : IDurationPredictionService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DurationPredictionService> _logger;
        private readonly string _modelDirectory;
        private readonly string _modelPath;
        private static ITransformer? _cachedModel;
        private static MLContext? _cachedContext;

        public DurationPredictionService(IServiceScopeFactory scopeFactory, ILogger<DurationPredictionService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _modelDirectory = Path.Combine(AppContext.BaseDirectory, "MLModels");
            _modelPath = Path.Combine(_modelDirectory, "NailDurationModel.zip");
        }

        public float PredictDuration(float stepsCount, float requiredComplexity, float artistSpeed, float baseDuration)
        {
            try
            {
                var (mlContext, model) = LoadOrGetCachedModel();
                var predictiionEngie = mlContext.Model.CreatePredictionEngine<DurationPredictionInput, DurationPredictionOutput>(model);

                var prediction = predictiionEngie.Predict(new DurationPredictionInput
                {
                    StepsCount = stepsCount,
                    RequiredComplexity = requiredComplexity,
                    ArtistSpeed = artistSpeed,
                    BaseDuration = baseDuration
                });
                return Math.Clamp(prediction.PredictedDuration, baseDuration * 0.7f, baseDuration * 2.0f);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Không thể dự đoán bằng ML.NET. Sử dụng thời gian cơ bản: {Base}", baseDuration);
                return baseDuration;
            }
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
                var model = mlContext.Model.Load(_modelPath, out _);
                _cachedModel = model;
                _cachedContext = mlContext;
                return (mlContext, model);
            }
            throw new FileNotFoundException("Không tìm thấy tệp mô hình ML dự báo thời gian.", _modelPath);
        }

        public async Task<ApiResult<ModelTrainingResultDto>> TrainModelAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var completedProcedures = await unitOfWork.BookingProcedureRepository.GetCompletedProceduresForTrainingAsync();
                if (completedProcedures.Count < 10)
                {
                    return new ApiErrorResult<ModelTrainingResultDto>("Chưa đủ dữ liệu hoàn thành để huấn luyện (yêu cầu tối thiểu 10 bản ghi).");
                }

                var trainingData = completedProcedures.Select(x =>
                {
                    var artist = x.AssignedArtist;
                    var variant = x.BookingItem.NailVariant;

                    float speedLevel = 3f;
                    if (artist != null && artist.NailArtistSkills != null)
                    {
                        var speedSkill = artist.NailArtistSkills.FirstOrDefault(x => x.SkillType.Name.ToUpper().Contains("SPEED") || x.SkillType.Name.Contains("Tốc độ"));
                        if (speedSkill != null)
                        {
                            speedLevel = speedSkill.Level;
                        }
                    }
                    //  Tính tổng điểm yêu câu kỹ năng của móng
                    float complexity = 0f;
                    if (variant != null && variant.NailRequiredSkills != null)
                    {
                        complexity = variant.NailRequiredSkills.Where(x => !x.SkillType.Name.ToUpper().Contains("SPEED") && !x.SkillType.Name.Contains("Tốc độ"))
                        .Sum(x => x.RequiredLevel);
                    }
                    return new DurationPredictionInput
                    {
                        StepsCount = x.BookingItem.Booking.BookingItems.SelectMany(y => y.BookingProcedures).Count(),
                        RequiredComplexity = complexity > 0 ? complexity : 10f, // Mặc định nếu chưa gán
                        ArtistSpeed = speedLevel,
                        BaseDuration = x.Duration,
                        ActualDuration = (float)(x.ActualEndTime!.Value - x.ActualStartTime!.Value).TotalMinutes
                    };
                }).ToList();

                var mlContext = new MLContext(seed: 42);
                var dataView = mlContext.Data.LoadFromEnumerable(trainingData);
                var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

                var pipeline = mlContext.Transforms.Concatenate("Features", nameof(DurationPredictionInput.StepsCount), nameof(DurationPredictionInput.RequiredComplexity), nameof(DurationPredictionInput.ArtistSpeed), nameof(DurationPredictionInput.BaseDuration))
                        .Append(mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(DurationPredictionInput.ActualDuration)));

                _logger.LogInformation("Huấn luyện mô hình hồi quy thời gian dựa trên kỹ năng với {Count} mẫu...", trainingData.Count);
                var model = pipeline.Fit(split.TrainSet);

                var predictions = model.Transform(split.TestSet);
                var metrics = mlContext.Regression.Evaluate(predictions, nameof(DurationPredictionInput.ActualDuration), "Score");

                if (!Directory.Exists(_modelDirectory))
                {
                    Directory.CreateDirectory(_modelDirectory);
                }
                mlContext.Model.Save(model, dataView.Schema, _modelPath);
                _cachedModel = model;
                _cachedContext = mlContext;
                var resultDto = new ModelTrainingResultDto
                {
                    TrainedAt = DateTime.UtcNow,
                    TotalSamples = trainingData.Count,
                    UniqueUsers = completedProcedures.Select(x => x.BookingItem.Booking.CustomerId).Distinct().Count(),
                    UniqueVariants = completedProcedures.Where(x => x.BookingItem.NailVariantId.HasValue).Select(x => x.BookingItem.NailVariantId!.Value).Distinct().Count(),
                    RmseScore = Math.Round(metrics.RootMeanSquaredError, 4),
                    ModelPath = _modelPath
                };
                return new ApiSuccessResult<ModelTrainingResultDto>(resultDto, "Huấn luyện mô hình dự báo thời gian dựa trên kỹ năng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi huấn luyện mô hình dự báo thời gian.");
                return new ApiErrorResult<ModelTrainingResultDto>($"Lỗi train model: {ex.Message}");
            }
        }
    }
}
