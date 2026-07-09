using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RecommendationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<List<RecommendedNailVariantResponseDTO>>> GetRecommendationsAsync(Guid userId, int limit = 10)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiErrorResult<List<RecommendedNailVariantResponseDTO>>("Không tìm thấy thông tin khách hàng.");
            }
            // Đọc dữ liệu từ DB
            var preferredColors = DeserializeList(customer.PreferredColorsJson);
            var preferredStyles = DeserializeList(customer.PreferredStylesJson);
            var preferredOccasions = DeserializeList(customer.PreferredOccasionsJson);
            int? preferredNailShapeId = customer.PreferredNailShapeId;
            var preferredComplexity = customer.PreferredComplexity;

            var favorites = await _unitOfWork.FavoriteNailRepository.GetFavoritesWithDetailsAsync(userId);
            var booking = await _unitOfWork.BookingRepository.GetCompletedBookingsWithDetailsAsync(userId);
            var variants = await _unitOfWork.NailVariantRepository.GetAllNailVariantsAsync();
            // Gom het dac trung cua mau nail
            var shapeList = await _unitOfWork.NailShapeRepository.GetAllNailShapesAsync();
            var shapes = shapeList.ToDictionary(x => x.NailShapeId, x => x.Name);

            var featureList = new List<string>();
            // Lấy hết các category theo nail desgin 
            var allCategories = variants.SelectMany(x => x.NailDesign.NailCategories.Select(x => x.Category.Name)).Distinct().ToList();

            var allColors = variants.SelectMany(x => ParseColorJson(x.ColorJson)).Distinct().ToList();
            var allShapes = variants.Select(x => $"Shape: {x.NailShapeId}").Distinct().ToList();
            var allSurfaces = variants.Select(x => $"Surface:{x.NailSurfaceId}").Distinct().ToList();
            // Gom het dac trung cua mau nail

            // Master Feature List
            // Rap danh sach cac chieu dac trung mau
            featureList.AddRange(allCategories.Select(c => $"Style:{c}"));
            featureList.AddRange(allColors.Select(c => $"Color:{c}"));
            featureList.AddRange(allShapes);
            featureList.AddRange(allSurfaces);
            featureList.Add("Complexity:simple");
            featureList.Add("Complexity:moderate");
            featureList.Add("Complexity:complex");

            // ["Style:Minimal", "Style:Ombre", "Color:Pink", "Color:Red", "Shape:1", "Complexity:simple"]
            var featureIndex = featureList.Select((f, idx) => new { Feature = f, Index = idx }).ToDictionary(x => x.Feature, x => x.Index);

            // Khoi tao mang de luu so thich cua nguoi dung doi voi tung dac trung
            double[] userVector = new double[featureList.Count];
            foreach (var col in preferredColors)
            {
                if (featureIndex.TryGetValue($"Color:{col}", out int idx))
                {
                    userVector[idx] += 2.0;
                }
            }
            foreach (var style in preferredStyles)
            {
                if (featureIndex.TryGetValue($"Style:{style}", out int idx))
                {
                    userVector[idx] += 2.0;
                }
            }
            foreach (var occasion in preferredOccasions)
            {
                if (featureIndex.TryGetValue($"Style:{occasion}", out int idx))
                {
                    userVector[idx] += 2.0;
                }
            }
            if (preferredNailShapeId.HasValue && featureIndex.TryGetValue($"Shape:{preferredNailShapeId}", out int x))
            {
                userVector[x] += 2.0;
            }
            if (!string.IsNullOrEmpty(preferredComplexity) && featureIndex.TryGetValue($"Complexity:{preferredComplexity.ToLower()}", out int compIdx))
            {
                userVector[compIdx] += 1.0;
            }
            foreach (var fav in favorites)
            {
                // Biến thể móng
                if (fav.NailVariant != null)
                {
                    foreach (var col in ParseColorJson(fav.NailVariant.ColorJson))
                    {
                        if (featureIndex.TryGetValue($"Color:{col}", out int idx))
                        {
                            userVector[idx] += 0.5;
                        }
                    }
                    if (featureIndex.TryGetValue($"Shape:{fav.NailVariant.NailShapeId}", out int idxShape))
                    {
                        userVector[idxShape] += 0.5;
                    }
                }
                var design = fav.NailVariant?.NailDesign ?? fav.NailDesign;
                if (design != null)
                {
                    foreach (var nc in design.NailCategories)
                    {
                        if (featureIndex.TryGetValue($"Style:{nc.Category.Name}", out int idxCat))
                        {
                            userVector[idxCat] += 0.5;
                        }
                    }
                }
            }
            // Dem so lan
            // Mau nail 1 : 12 lan
            var variantBookingCounts = new Dictionary<int, int>();
            foreach (var b in booking)
            {
                foreach (var item in b.BookingItems)
                {
                    if (item.NailVariantId.HasValue)
                    {
                        var nailVarinatId = item.NailVariantId.Value;
                        if (variantBookingCounts.ContainsKey(nailVarinatId))
                        {
                            variantBookingCounts[nailVarinatId]++;
                        }
                        else
                        {
                            variantBookingCounts[nailVarinatId] = 1;
                        }
                        if (item.NailVariant != null)
                        {
                            foreach (var col in ParseColorJson(item.NailVariant.ColorJson))
                            {
                                if (featureIndex.TryGetValue($"Color:{col}", out int idx))
                                {
                                    userVector[idx] += 0.3;
                                }
                            }
                            if (featureIndex.TryGetValue($"Shape:{item.NailVariant.NailShapeId}", out int idxShape))
                            {
                                userVector[idxShape] += 0.3;
                            }
                            if (item.NailVariant.NailDesign != null)
                            {
                                foreach (var nc in item.NailVariant.NailDesign.NailCategories)
                                {
                                    if (featureIndex.TryGetValue($"Style:{nc.Category.Name}", out int idx))
                                    {
                                        userVector[idx] += 0.3;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var globalBookingCounts = await _unitOfWork.BookingItemRepository.GetGlobalBookingCountsAsync();
            // Tính toán Độ dài (Norm) của Vector sở thích khách hàng
            // Tinh do dai cua khach bang tong binh phuong cac dac trung cua khach
            double userNorm = Math.Sqrt(userVector.Sum(val => val * val));
            var recommendedList = new List<RecommendedNailVariantResponseDTO>();

            foreach (var v in variants)
            {
                // vector cua mot mau mong dang xet
                double[] variantVector = new double[featureList.Count];
                var vColors = ParseColorJson(v.ColorJson);
                var vCats = v.NailDesign.NailCategories.Select(nc => nc.Category.Name).ToList();
                foreach (var col in vColors)
                {
                    if (featureIndex.TryGetValue($"Color:{col}", out int idx))
                    {
                        variantVector[idx] = 1.0;
                    }
                }
                foreach (var cat in vCats)
                {
                    if (featureIndex.TryGetValue($"Style:{cat}", out int idx))
                    {
                        variantVector[idx] = 1.0;
                    }
                }
                if (featureIndex.TryGetValue($"Shape:{v.NailShapeId}", out int shapeIdIdx))
                {
                    variantVector[shapeIdIdx] = 1.0;
                }
                if (featureIndex.TryGetValue($"Surface:{v.NailSurfaceId}", out int surfIdIdx))
                {
                    variantVector[surfIdIdx] = 1.0;
                }
                string vComplexity = GetComplexity(v.Duration ?? 60);
                if (featureIndex.TryGetValue($"Complexity:{vComplexity}", out int compIdIdx))
                {
                    variantVector[compIdIdx] = 1.0;
                }
                // Tong diem so thich trung khop giua khach va mau (ti le % hop)
                double dotProduct = 0;
                // La do lon hinhh hoc cua mau mong do
                double variantNorm = 0;
                for(int i = 0; i < featureList.Count; i++)
                {
                    // Triet tieu mau mong ko hop (0.0)
                    dotProduct += userVector[i] * variantVector[i];
                    variantNorm += variantVector[i] * variantVector[i];
                }

                variantNorm = Math.Sqrt(variantNorm);
                double similarity = 0.0;
                if (userNorm > 0 && variantNorm > 0)
                {
                    similarity = dotProduct / (userNorm * variantNorm);
                }
                var reasons = new List<string>();
                for(int i = 0; i < featureList.Count; i++)
                {
                    if (userVector[i] > 0 && variantVector[i] > 0)
                    {
                        var featureKey = featureList[i];
                        if (featureKey.StartsWith("Color:"))
                        {
                            reasons.Add($"Phù hợp với tông màu {featureKey.Replace("Color:", "")} bạn yêu thích.");
                        }
                        else if (featureKey.StartsWith("Style:"))
                        {
                            reasons.Add($"Khớp với phong cách/dịp {featureKey.Replace("Style:", "")} bạn muốn tìm.");
                        }
                        else if (featureKey.StartsWith("Shape:") && v.NailShapeId.HasValue && shapes.TryGetValue(v.NailShapeId.Value, out var shapeName))
                        {
                            reasons.Add($"Đúng kiểu dáng form móng {shapeName} ưa thích.");
                        }
                    }
                }
                double finalScore = similarity * 100;
                if (finalScore == 0)
                {
                    globalBookingCounts.TryGetValue(v.NailVariantId, out int bookCount);
                    finalScore = Math.Min(10, bookCount * 0.5);
                    reasons.Add("Gợi ý phổ biến đang được nhiều khách hàng yêu thích.");
                }

                var dto = _mapper.Map<RecommendedNailVariantResponseDTO>(v);
                dto.Name = $"{v.NailDesign.Name} - {v.Name}";
                dto.Score = Math.Round(finalScore, 1);
                dto.Reasons = reasons.Distinct().Take(3).ToList();
                recommendedList.Add(dto);
            }
            var results = recommendedList.OrderByDescending(r => r.Score).Take(limit).ToList();
            return new ApiSuccessResult<List<RecommendedNailVariantResponseDTO>>(results, "Lấy danh sách gợi ý thành công.");
        }

        public async Task<ApiResult<PagedList<RecommendedNailVariantResponseDTO>>> GetRecommendationsFeedAsync(Guid userId, int pageNumber, int pageSize)
        {
            var allResult = await GetRecommendationsAsync(userId, 100);
            if (!allResult.IsSucceeded)
            {
                return new ApiErrorResult<PagedList<RecommendedNailVariantResponseDTO>>(allResult.Message);
            }
            var all = allResult.Data;
            var pagedItems = all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var paged = new PagedList<RecommendedNailVariantResponseDTO>(pagedItems, all.Count, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<RecommendedNailVariantResponseDTO>>(paged, "Lấy Smart Feed thành công.");
        }

        public async Task<ApiResult<List<RecommendedNailVariantResponseDTO>>> SubmitQuizAnswersAsync(Guid userId, SubmitQuizAnswersRequestDto request)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiErrorResult<List<RecommendedNailVariantResponseDTO>>("Không tìm thấy thông tin khách hàng.");
            }
            var options = await _unitOfWork.QuizOptionRepository.GetOptionsWithQuestionsAsync(request.SelectedOptionIds);
            if (!options.Any())
            {
                return new ApiErrorResult<List<RecommendedNailVariantResponseDTO>>("Danh sách tùy chọn lựa chọn không hợp lệ.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingAnswers = await _unitOfWork.CustomerQuizAnswerRepository.GetAnswersByCustomerIdAsync(userId);
                foreach (var ans in existingAnswers)
                {
                    _unitOfWork.CustomerQuizAnswerRepository.Delete(ans);
                }
                await _unitOfWork.SaveChangesAsync();

                foreach (var x in options)
                {
                    var newAnswer = new CustomerQuizAnswer
                    {
                        CustomerId = userId,
                        QuizQuestionId = x.QuizQuestionId,
                        QuizOptionId = x.QuizOptionId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.CustomerQuizAnswerRepository.CreateAsync(newAnswer);
                }
                await _unitOfWork.SaveChangesAsync();

                var colorValues = options.Where(x => x.QuizQuestion.Category == QuizCategory.Color).Select(o => o.OptionValue).ToList();
                var styleValues = options.Where(x => x.QuizQuestion.Category == QuizCategory.Style).Select(o => o.OptionValue).ToList();
                var occasionValues = options.Where(x => x.QuizQuestion.Category == QuizCategory.Occasion).Select(o => o.OptionValue).ToList();

                var shapeOpt = options.FirstOrDefault(x => x.QuizQuestion.Category == QuizCategory.Shape);

                int? shapeId = null;
                if (shapeOpt != null && int.TryParse(shapeOpt.OptionValue, out int parsedId))
                {
                    shapeId = parsedId;
                }

                var complexityOpt = options.FirstOrDefault(x => x.QuizQuestion.Category == QuizCategory.Complexity)?.OptionValue;
                var skinToneOpt = options.FirstOrDefault(x => x.QuizQuestion.Category == QuizCategory.SkinTone)?.OptionValue;
                customer.PreferredColorsJson = JsonSerializer.Serialize(colorValues);
                customer.PreferredStylesJson = JsonSerializer.Serialize(styleValues);
                customer.PreferredOccasionsJson = JsonSerializer.Serialize(occasionValues);
                customer.PreferredNailShapeId = shapeId;
                customer.PreferredComplexity = complexityOpt ?? string.Empty;
                customer.SkinTone = skinToneOpt ?? customer.SkinTone;
                _unitOfWork.CustomerRepository.Update(customer);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Get fresh recommendations immediately
                var recommendationsResult = await GetRecommendationsAsync(userId);
                return recommendationsResult;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ApiErrorResult<List<RecommendedNailVariantResponseDTO>>($"Lỗi hệ thống khi lưu đáp án: {ex.Message}");
            }
        }
        // Chuyển đổi các cột dữ liệu JSON lưu trong bảng Customer từ string về List<string>
        private List<string> DeserializeList(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<string>();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
        private List<string> ParseColorJson(string colorJson)
        {
            if (string.IsNullOrEmpty(colorJson)) return new List<string>();
            var colors = new List<string>();
            try
            {
                using var doc = JsonDocument.Parse(colorJson);
                var root = doc.RootElement;

                // 1. If it's a simple array of strings: ["#FF0000", "#0000FF"]
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in root.EnumerateArray())
                    {
                        if (element.ValueKind == JsonValueKind.String)
                        {
                            var color = element.GetString();
                            if (!string.IsNullOrEmpty(color)) colors.Add(color);
                        }
                    }
                }
                // 2. If it's a complex color JSON object (solid, gradient, perFinger)
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    // Extract main color if present
                    if (root.TryGetProperty("color", out var mainColorProp) && mainColorProp.ValueKind == JsonValueKind.String)
                    {
                        var color = mainColorProp.GetString();
                        if (!string.IsNullOrEmpty(color)) colors.Add(color);
                    }
                    // Extract colors from main gradient if present
                    if (root.TryGetProperty("gradient", out var gradProp) && gradProp.ValueKind == JsonValueKind.Object)
                    {
                        ExtractColorsFromGradient(gradProp, colors);
                    }
                    // Extract colors from fingers if present
                    if (root.TryGetProperty("fingers", out var fingersProp) && fingersProp.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var finger in fingersProp.EnumerateArray())
                        {
                            if (finger.TryGetProperty("color", out var fingerColorProp) && fingerColorProp.ValueKind == JsonValueKind.String)
                            {
                                var color = fingerColorProp.GetString();
                                if (!string.IsNullOrEmpty(color)) colors.Add(color);
                            }
                            if (finger.TryGetProperty("gradient", out var fingerGradProp) && fingerGradProp.ValueKind == JsonValueKind.Object)
                            {
                                ExtractColorsFromGradient(fingerGradProp, colors);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fallback for simple comma-separated string
                colors = colorJson.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(c => c.Trim())
                                  .ToList();
            }
            return colors.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }
        private void ExtractColorsFromGradient(JsonElement gradElement, List<string> colors)
        {
            if (gradElement.TryGetProperty("stops", out var stopsProp) && stopsProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var stop in stopsProp.EnumerateArray())
                {
                    if (stop.ValueKind == JsonValueKind.String)
                    {
                        var color = stop.GetString();
                        if (!string.IsNullOrEmpty(color)) colors.Add(color);
                    }
                }
            }
        }
        private string GetComplexity(int duration)
        {
            if (duration <= 60) return "simple";
            if (duration <= 90) return "moderate";
            return "complex";
        }
    }
}
