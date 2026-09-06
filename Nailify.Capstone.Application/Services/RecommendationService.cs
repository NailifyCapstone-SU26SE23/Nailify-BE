using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Nailify.Capstone.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private static readonly HttpClient OpenRouterHttpClient = new HttpClient();
        private static readonly JsonSerializerOptions LlmJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly ILogger<RecommendationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INemotronConfiguration _nemotronConfiguration;

        public RecommendationService(IUnitOfWork unitOfWork, IMapper mapper,
            INemotronConfiguration nemotronConfiguration, ILogger<RecommendationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _nemotronConfiguration = nemotronConfiguration;
            _logger = logger;
        }

        public async Task<ApiResult<RecommendedNailCompositionDto>> GetRecommendedCompositionAsync(Guid userId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiErrorResult<RecommendedNailCompositionDto>("Không tìm thấy khách hàng");
            }

            var request = new RecommendationCompositionRequest
            {
                SkinTone = customer.SkinTone,
                SkinShade = customer.SkinShade,
                HandShape = customer.HandShape,
                Occupation = customer.Occupation,
                NailCondition = customer.NailCondition,
                PreferredColors = DeserializeList(customer.PreferredColorsJson),
                PreferredStyles = DeserializeList(customer.PreferredStylesJson),
                PreferredOccasions = DeserializeList(customer.PreferredOccasionsJson),
                PreferredNailShapeId = customer.PreferredNailShapeId,
                PreferredComplexity = customer.PreferredComplexity
            };

            return await GetRecommendedCompositionAsync(request);
        }

        public async Task<ApiResult<RecommendedNailCompositionDto>> GetRecommendedCompositionAsync(RecommendationCompositionRequest request)
        {
            if (request == null)
            {
                return new ApiErrorResult<RecommendedNailCompositionDto>("Dữ liệu gợi ý không hợp lệ");
            }

            var shapes = await _unitOfWork.NailShapeRepository.GetAllNailShapesAsync();
            var surfaces = await _unitOfWork.NailSurfaceRepository.GetAllNailSurfacesAsync();

            if (!shapes.Any())
            {
                return new ApiErrorResult<RecommendedNailCompositionDto>("Không có dữ liệu cho dáng móng");
            }

            if (!surfaces.Any())
            {
                return new ApiErrorResult<RecommendedNailCompositionDto>("Không có dữ liệu cho bề mặt móng");
            }

            var llmResult = await TryGetOpenRouterCompositionAsync(request, shapes, surfaces);
            var selectedShape = ResolveRecommendedShape(llmResult, request, shapes);
            var selectedSurface = ResolveRecommendedSurface(llmResult, request, surfaces);
            var colors = ResolveRecommendedColors(llmResult, request);

            var dto = new RecommendedNailCompositionDto
            {
                NailShapeId = selectedShape.NailShapeId,
                NailSurfaceId = selectedSurface.NailSurfaceId,
                NailShape = _mapper.Map<NailShapeDto>(selectedShape),
                NailSurface = _mapper.Map<NailSurfaceDto>(selectedSurface),
                Colors = colors
            };

            return new ApiSuccessResult<RecommendedNailCompositionDto>(dto, "Lấy gợi ý cấu hình móng thành công");
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
            var preferredCategoryIds = preferredStyles
                .Concat(preferredOccasions)
                .Concat(ParseOptionValues(customer.SkinTone ?? string.Empty))
                .Concat(ParseOptionValues(customer.SkinShade ?? string.Empty))
                .Concat(ParseOptionValues(customer.HandShape ?? string.Empty))
                .SelectMany(ParseIntValues)
                .ToHashSet();

            var favorites = await _unitOfWork.FavoriteNailRepository.GetFavoritesWithDetailsAsync(userId);
            var booking = await _unitOfWork.BookingRepository.GetCompletedBookingsWithDetailsAsync(userId);
            var variants = await _unitOfWork.NailVariantRepository.GetAllNailVariantsAsync();
            // Gom het dac trung cua mau nail
            var shapeList = await _unitOfWork.NailShapeRepository.GetAllNailShapesAsync();
            var shapes = shapeList.ToDictionary(x => x.NailShapeId, x => x.Name);

            var featureList = new List<string>();
            // Lấy hết các category của các variant đang xét và gom nhóm theo ID
            var categories = variants
                .Where(x => x.NailDesign != null)
                .SelectMany(x => x.NailDesign!.NailCategories)
                .Where(nc => nc.Category != null)
                .Select(nc => nc.Category)
                .GroupBy(c => c.CategoryId)
                .Select(g => g.First())
                .ToList();
            var categoryNames = categories.ToDictionary(c => c.CategoryId, c => c.Name);

            var allColors = variants.SelectMany(x => ParseColorJson(x.ColorJson)).Distinct().ToList();
            var allShapes = variants.Select(x => $"Shape:{x.NailShapeId}").Distinct().ToList();
            var allSurfaces = variants.Select(x => $"Surface:{x.NailSurfaceId}").Distinct().ToList();
            // Gom het dac trung cua mau nail
            // Rap danh sach cac chieu dac trung mau

            // Gộp tất cả các nhãn thể loại từ các variant (Style, Occasion, Skin Tone, v.v.)
            // Tạo tiền tố
            foreach (var cat in categories)
            {
                featureList.Add($"Category:{cat.CategoryId}");
            }

            featureList.AddRange(allColors.Select(c => $"Color:{c}"));
            featureList.AddRange(allShapes);
            featureList.AddRange(allSurfaces);
            featureList.Add("Complexity:simple");
            featureList.Add("Complexity:moderate");
            featureList.Add("Complexity:complex");

            // ["Style:Minimal", "Style:Ombre", "Color:Pink", "Color:Red", "Shape:1", "Complexity:simple"]
            var featureIndex = featureList.Select((f, idx) => new { Feature = f, Index = idx }).ToDictionary(x => x.Feature, x => x.Index);

            // Khoi tao mang de luu so thich cua nguoi dung doi voi tung dac trung
            // Khởi tạo vector sở thích khách hàng
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
                foreach (var categoryId in ParseIntValues(style))
                {
                    if (featureIndex.TryGetValue($"Category:{categoryId}", out int categoryIdx))
                    {
                        userVector[categoryIdx] += 2.0;
                    }
                }
                if (featureIndex.TryGetValue($"Style:{style}", out int styleIdx))
                {
                    userVector[styleIdx] += 2.0;
                }
            }
            foreach (var occasion in preferredOccasions)
            {
                foreach (var categoryId in ParseIntValues(occasion))
                {
                    if (featureIndex.TryGetValue($"Category:{categoryId}", out int categoryIdx))
                    {
                        userVector[categoryIdx] += 2.0;
                    }
                }
                if (featureIndex.TryGetValue($"Occasion:{occasion}", out int occasionIdx))
                {
                    userVector[occasionIdx] += 2.0;
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
            if (!string.IsNullOrEmpty(customer.SkinTone))
            {
                foreach (var categoryId in ParseIntValues(customer.SkinTone))
                {
                    if (featureIndex.TryGetValue($"Category:{categoryId}", out int categoryIdx))
                    {
                        userVector[categoryIdx] += 2.0;
                    }
                }
                if (featureIndex.TryGetValue($"SkinTone:{customer.SkinTone}", out int skinToneIdx))
                {
                    userVector[skinToneIdx] += 2.0;
                }
            }
            if (!string.IsNullOrEmpty(customer.SkinShade))
            {
                foreach (var categoryId in ParseIntValues(customer.SkinShade))
                {
                    if (featureIndex.TryGetValue($"Category:{categoryId}", out int categoryIdx))
                    {
                        userVector[categoryIdx] += 2.0;
                    }
                }
                if (featureIndex.TryGetValue($"SkinShade:{customer.SkinShade}", out int skinShadeIdx))
                {
                    userVector[skinShadeIdx] += 2.0;
                }
            }
            if (!string.IsNullOrEmpty(customer.HandShape))
            {
                foreach (var categoryId in ParseIntValues(customer.HandShape))
                {
                    if (featureIndex.TryGetValue($"Category:{categoryId}", out int categoryIdx))
                    {
                        userVector[categoryIdx] += 2.0;
                    }
                }
                if (featureIndex.TryGetValue($"HandShape:{customer.HandShape}", out int handShapeIdx))
                {
                    userVector[handShapeIdx] += 2.0;
                }
            }

            // Favorites
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
                        if (nc.Category == null)
                        {
                            continue;
                        }
                        if (featureIndex.TryGetValue($"Category:{nc.CategoryId}", out int idxCat))
                        {
                            userVector[idxCat] += 0.5;
                        }
                    }
                }
            }
            // Dem so lan
            // Mau nail 1 : 12 lan
            // Lịch sử đặt lịch
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
                                    if (nc.Category == null)
                                    {
                                        continue;
                                    }
                                    if (featureIndex.TryGetValue($"Category:{nc.CategoryId}", out int idx))
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

                foreach (var col in vColors)
                {
                    if (featureIndex.TryGetValue($"Color:{col}", out int idx))
                    {
                        variantVector[idx] = 1.0;
                    }
                }
                if (v.NailDesign != null)
                {
                    foreach (var nc in v.NailDesign.NailCategories)
                    {
                        if (nc.Category == null)
                        {
                            continue;
                        }
                        if (featureIndex.TryGetValue($"Category:{nc.CategoryId}", out int idx))
                        {
                            variantVector[idx] = 1.0;
                        }
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
                // Tính Cosine Similarity thô
                double dotProduct = 0;
                // La do lon hinhh hoc cua mau mong do
                double variantNorm = 0;
                for (int i = 0; i < featureList.Count; i++)
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

                // Rules Engine 1: Hand Shape to Nail Shape Harmony
                string? shapeNameStr = v.NailShape?.Name;
                if (!string.IsNullOrEmpty(customer.HandShape) && !string.IsNullOrEmpty(shapeNameStr))
                {
                    if (customer.HandShape.Contains("Mu bàn tay rộng") || customer.HandShape.Contains("mập") || customer.HandShape.Contains("ngắn") || customer.HandShape.Contains("đều tròn"))
                    {
                        if (shapeNameStr.Contains("Vuông") || shapeNameStr.Contains("Square") || shapeNameStr.Contains("Thang") || shapeNameStr.Contains("Coffin"))
                        {
                            similarity -= 0.15; // Phạt
                            reasons.Add("Kiểu tay hơi đầy đặn nên hạn chế dáng vuông/thang để tránh tay trông ngắn hơn.");
                        }
                        else if (shapeNameStr.Contains("Hạnh nhân") || shapeNameStr.Contains("Almond") || shapeNameStr.Contains("Tròn") || shapeNameStr.Contains("Round") || shapeNameStr.Contains("Bầu dục") || shapeNameStr.Contains("Oval"))
                        {
                            similarity += 0.05; // Thưởng
                            reasons.Add("Dáng móng này giúp ngón tay của bạn trông thon dài và mềm mại hơn.");
                        }
                    }
                    else if (customer.HandShape.Contains("thon dài") || customer.HandShape.Contains("nhỏ") || customer.HandShape.Contains("thanh mảnh"))
                    {
                        if (shapeNameStr.Contains("Vuông") || shapeNameStr.Contains("Square") || shapeNameStr.Contains("Nhọn") || shapeNameStr.Contains("Stiletto"))
                        {
                            similarity += 0.05; // Thưởng
                            reasons.Add("Cực kỳ phù hợp và tôn dáng ngón tay thon dài thanh mảnh sẵn có.");
                        }
                    }
                }

                // Rules Engine 2: Color Harmony by Skin Tone
                if (!string.IsNullOrEmpty(customer.SkinTone))
                {
                    bool hasWarmColor = vColors.Any(IsWarmColor);
                    bool hasCoolColor = vColors.Any(IsCoolColor);

                    if (customer.SkinTone.Contains("Warm") || customer.SkinTone.Contains("Ấm"))
                    {
                        if (hasWarmColor)
                        {
                            similarity += 0.10;
                            reasons.Add("Gam màu ấm rất đồng điệu và làm sáng tông da ấm của bạn.");
                        }
                    }
                    else if (customer.SkinTone.Contains("Cool") || customer.SkinTone.Contains("Lạnh"))
                    {
                        if (hasCoolColor)
                        {
                            similarity += 0.10;
                            reasons.Add("Gam màu lạnh tương phản dịu mát, giúp tôn tông da lạnh sáng hồng.");
                        }
                    }
                }

                // Giới hạn tương đồng trong khoảng [0.0, 1.0]
                similarity = Math.Clamp(similarity, 0.0, 1.0);

                // Thêm lý do khớp từ vector đặc trưng
                for (int i = 0; i < featureList.Count; i++)
                {
                    if (userVector[i] > 0 && variantVector[i] > 0)
                    {
                        var featureKey = featureList[i];
                        if (featureKey.StartsWith("Color:"))
                        {
                            reasons.Add($"Tông màu {featureKey.Replace("Color:", "")} khớp với màu bạn thích.");
                        }
                        else if (featureKey.StartsWith("Style:"))
                        {
                            reasons.Add($"Mang phong cách {featureKey.Replace("Style:", "")} yêu thích của bạn.");
                        }
                        else if (featureKey.StartsWith("Occasion:"))
                        {
                            reasons.Add($"Thiết kế rất phù hợp cho dịp {featureKey.Replace("Occasion:", "")}.");
                        }
                        else if (featureKey.StartsWith("Shape:") && v.NailShapeId.HasValue && shapes.TryGetValue(v.NailShapeId.Value, out var shapeName))
                        {
                            reasons.Add($"Mẫu móng dáng {shapeName} theo sở thích.");
                        }
                        else if (featureKey.StartsWith("Category:") && int.TryParse(featureKey.Replace("Category:", ""), out var categoryId) && categoryNames.TryGetValue(categoryId, out var categoryName))
                        {
                            reasons.Add($"Phù hợp với danh mục {categoryName} bạn đã chọn.");
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
                dto.Name = v.NailDesign != null ? $"{v.NailDesign.Name} - {v.Name}" : v.Name;
                dto.Score = Math.Round(finalScore, 1);
                dto.Reasons = reasons.Distinct().Take(3).ToList();

                var matchedChars = new List<MatchedCharacteristicDTO>();

                // Colors
                foreach (var col in vColors)
                {
                    bool isMatch = preferredColors.Any(c => string.Equals(c, col, StringComparison.OrdinalIgnoreCase));
                    matchedChars.Add(new MatchedCharacteristicDTO
                    {
                        Category = "Color",
                        Value = col,
                        Label = $"Màu {col}",
                        IsMatchingPreference = isMatch,
                        Description = isMatch ? "Trùng khớp với màu sắc ưa thích của bạn." : "Màu sắc của mẫu móng."
                    });
                }

                // Nail Categories
                if (v.NailDesign != null)
                {
                    foreach (var nc in v.NailDesign.NailCategories)
                    {
                        if (nc.Category == null)
                        {
                            continue;
                        }
                        var typeName = nc.Category.CategoryType?.Name ?? "Category";
                        string prefix = typeName switch
                        {
                            "Style" => "Style",
                            "Occasion" => "Occasion",
                            "SkinUndertone" => "SkinTone",
                            "SkinTone" => "SkinTone",
                            "SkinShade" => "SkinShade",
                            "HandShape" => "HandShape",
                            _ => typeName
                        };

                        bool isMatch = false;
                        string desc = string.Empty;

                        if (prefix == "Style")
                        {
                            isMatch = preferredCategoryIds.Contains(nc.CategoryId)
                                || preferredStyles.Any(s => string.Equals(s, nc.Category.Name, StringComparison.OrdinalIgnoreCase));
                            desc = isMatch ? "Trùng khớp với phong cách thiết kế bạn yêu thích." : "Phong cách của mẫu móng.";
                        }
                        else if (prefix == "Occasion")
                        {
                            isMatch = preferredCategoryIds.Contains(nc.CategoryId)
                                || preferredOccasions.Any(o => string.Equals(o, nc.Category.Name, StringComparison.OrdinalIgnoreCase));
                            desc = isMatch ? "Thiết kế phù hợp với dịp bạn lựa chọn." : "Dịp phù hợp cho mẫu móng.";
                        }
                        else if (prefix == "SkinTone")
                        {
                            isMatch = preferredCategoryIds.Contains(nc.CategoryId)
                                || !string.IsNullOrEmpty(customer.SkinTone) && string.Equals(customer.SkinTone, nc.Category.Name, StringComparison.OrdinalIgnoreCase);
                            desc = isMatch ? "Tông da phù hợp lý tưởng." : "Tông da khuyên dùng.";
                        }
                        else if (prefix == "SkinShade")
                        {
                            isMatch = !string.IsNullOrEmpty(customer.SkinShade) && string.Equals(customer.SkinShade, nc.Category.Name, StringComparison.OrdinalIgnoreCase);
                            desc = isMatch ? "Độ sáng da phù hợp lý tưởng." : "Độ sáng da khuyên dùng.";
                        }
                        else if (prefix == "HandShape")
                        {
                            isMatch = preferredCategoryIds.Contains(nc.CategoryId)
                                || !string.IsNullOrEmpty(customer.HandShape) && customer.HandShape.Contains(nc.Category.Name, StringComparison.OrdinalIgnoreCase);
                            desc = isMatch ? "Dáng tay phù hợp lý tưởng." : "Dáng tay khuyên dùng.";
                        }
                        else
                        {
                            isMatch = preferredCategoryIds.Contains(nc.CategoryId);
                            desc = isMatch ? "Trùng khớp với danh mục bạn đã chọn." : "Danh mục của mẫu móng.";
                        }

                        matchedChars.Add(new MatchedCharacteristicDTO
                        {
                            Category = prefix,
                            Value = nc.Category.Name,
                            Label = nc.Category.Name,
                            IsMatchingPreference = isMatch,
                            Description = desc
                        });
                    }
                }

                // Nail Shape
                if (v.NailShapeId.HasValue && shapes.TryGetValue(v.NailShapeId.Value, out var matchedShapeName))
                {
                    bool isMatch = preferredNailShapeId == v.NailShapeId;
                    string desc = isMatch ? "Dáng móng ưa thích của bạn." : "Dáng móng của mẫu thiết kế.";

                    if (!isMatch && !string.IsNullOrEmpty(customer.HandShape))
                    {
                        if (customer.HandShape.Contains("Mu bàn tay rộng") || customer.HandShape.Contains("mập") || customer.HandShape.Contains("ngắn") || customer.HandShape.Contains("đều tròn"))
                        {
                            if (matchedShapeName.Contains("Hạnh nhân") || matchedShapeName.Contains("Almond") || matchedShapeName.Contains("Tròn") || matchedShapeName.Contains("Round") || matchedShapeName.Contains("Bầu dịch") || matchedShapeName.Contains("Oval"))
                            {
                                isMatch = true;
                                desc = "Dáng móng này giúp ngón tay của bạn trông thon dài và mềm mại hơn.";
                            }
                        }
                        else if (customer.HandShape.Contains("thon dài") || customer.HandShape.Contains("nhỏ") || customer.HandShape.Contains("thanh mảnh"))
                        {
                            if (matchedShapeName.Contains("Vuông") || matchedShapeName.Contains("Square") || matchedShapeName.Contains("Nhọn") || matchedShapeName.Contains("Stiletto"))
                            {
                                isMatch = true;
                                desc = "Cực kỳ phù hợp và tôn dáng ngón tay thon dài thanh mảnh sẵn có.";
                            }
                        }
                    }

                    matchedChars.Add(new MatchedCharacteristicDTO
                    {
                        Category = "Shape",
                        Value = matchedShapeName,
                        Label = $"Dáng móng: {matchedShapeName}",
                        IsMatchingPreference = isMatch,
                        Description = desc
                    });
                }

                bool compMatch = !string.IsNullOrEmpty(preferredComplexity) && string.Equals(preferredComplexity, vComplexity, StringComparison.OrdinalIgnoreCase);
                matchedChars.Add(new MatchedCharacteristicDTO
                {
                    Category = "Complexity",
                    Value = vComplexity,
                    Label = $"Độ phức tạp: {vComplexity}",
                    IsMatchingPreference = compMatch,
                    Description = compMatch ? "Mức độ phức tạp phù hợp với thói quen làm móng của bạn." : "Mức độ phức tạp thiết kế."
                });

                dto.MatchedCharacteristics = matchedChars;
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

                var colorValues = options.Where(x => x.QuizQuestion.Category == QuizCategory.Color).SelectMany(o => ParseOptionValues(o.OptionValue)).ToList();
                var styleValues = options.Where(x => x.QuizQuestion.Category == QuizCategory.Style).SelectMany(o => ParseOptionValues(o.OptionValue)).ToList();
                var occasionValues = options.Where(x => x.QuizQuestion.Category == QuizCategory.Occasion).SelectMany(o => ParseOptionValues(o.OptionValue)).ToList();

                var shapeOpt = options.FirstOrDefault(x => x.QuizQuestion.Category == QuizCategory.Shape);

                int? shapeId = null;
                if (shapeOpt != null)
                {
                    var shapeVal = ParseOptionValues(shapeOpt.OptionValue).FirstOrDefault();
                    if (shapeVal != null && int.TryParse(shapeVal, out int parsedId))
                    {
                        shapeId = parsedId;
                    }
                }

                var complexityOpt = options.FirstOrDefault(x => x.QuizQuestion.Category == QuizCategory.Complexity);
                var complexityVal = complexityOpt != null ? ParseOptionValues(complexityOpt.OptionValue).FirstOrDefault() : null;
                var skinToneOpt = options.FirstOrDefault(x => x.QuizQuestion.Category == QuizCategory.SkinTone);
                var skinToneVal = skinToneOpt != null ? ParseOptionValues(skinToneOpt.OptionValue).FirstOrDefault() : null;

                var handShapeOpt = options.FirstOrDefault(x => x.QuizQuestion.Category == QuizCategory.HandShape);
                var handShapeVal = handShapeOpt != null ? string.Join(", ", ParseOptionValues(handShapeOpt.OptionValue)) : null;
                var skinShadeOpt = options.FirstOrDefault(x => x.QuizQuestion.Category == QuizCategory.SkinShade);
                var skinShadeVal = skinShadeOpt != null ? ParseOptionValues(skinShadeOpt.OptionValue).FirstOrDefault() : null;

                customer.PreferredColorsJson = JsonSerializer.Serialize(colorValues);
                customer.PreferredStylesJson = JsonSerializer.Serialize(styleValues);
                customer.PreferredOccasionsJson = JsonSerializer.Serialize(occasionValues);
                customer.PreferredNailShapeId = shapeId;
                customer.PreferredComplexity = complexityVal ?? string.Empty;
                customer.SkinTone = skinToneVal ?? customer.SkinTone;
                customer.HandShape = handShapeVal ?? customer.HandShape;
                customer.SkinShade = skinShadeVal ?? customer.SkinShade;

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

        private bool IsWarmColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return false;
            if (color.StartsWith("#"))
            {
                var hex = color.Trim().Replace("#", "");
                if (hex.Length == 6)
                {
                    if (int.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out int r) &&
                        int.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out int g) &&
                        int.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out int b))
                    {
                        return r > b + 15;
                    }
                }
                return false;
            }

            var warmKeywords = new[] { "Vàng", "Cam", "Đỏ", "Nâu", "Champagne", "Vàng hồng", "Rose Gold", "Gold", "Warm", "Yellow", "Orange", "Red", "Brown", "Nude" };
            return warmKeywords.Any(key => color.Contains(key, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsCoolColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return false;
            if (color.StartsWith("#"))
            {
                var hex = color.Trim().Replace("#", "");
                if (hex.Length == 6)
                {
                    if (int.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out int r) &&
                        int.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out int g) &&
                        int.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out int b))
                    {
                        return b > r + 15;
                    }
                }
                return false;
            }

            var coolKeywords = new[] { "Xanh", "Bạc", "Tím", "Mint", "Silver", "Đen", "Blue", "Green", "Purple", "Lavender", "Black" };
            return coolKeywords.Any(key => color.Contains(key, StringComparison.OrdinalIgnoreCase));
        }
        private List<string> ParseOptionValues(string optionValueJson)
        {
            if (string.IsNullOrEmpty(optionValueJson)) return new List<string>();
            try
            {
                if (optionValueJson.TrimStart().StartsWith("["))
                {
                    return JsonSerializer.Deserialize<List<string>>(optionValueJson) ?? new List<string>();
                }
                return new List<string> { optionValueJson };
            }
            catch
            {
                return new List<string> { optionValueJson };
            }
        }

        private IEnumerable<int> ParseIntValues(string value)
        {
            foreach (var item in ParseOptionValues(value))
            {
                if (int.TryParse(item, out var parsed))
                {
                    yield return parsed;
                }
            }
        }

        private NailShape SelectRecommendedShape(RecommendationCompositionRequest request, List<NailShape> shapes)
        {
            if (request.PreferredNailShapeId.HasValue)
            {
                var preferredShape = shapes.FirstOrDefault(shape => shape.NailShapeId == request.PreferredNailShapeId.Value);
                if (preferredShape != null)
                {
                    return preferredShape;
                }
            }

            var handShape = request.HandShape ?? string.Empty;
            var shapeKeywords = handShape.Contains("elong", StringComparison.OrdinalIgnoreCase)
                || handShape.Contains("thon", StringComparison.OrdinalIgnoreCase)
                || handShape.Contains("dai", StringComparison.OrdinalIgnoreCase)
                    ? new[] { "Square", "Vuong", "Stiletto", "Nhon" }
                    : new[] { "Almond", "Hanh nhan", "Oval", "Bau", "Round", "Tron" };

            return shapes.FirstOrDefault(shape => shapeKeywords.Any(keyword => shape.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                ?? shapes.OrderBy(shape => shape.NailShapeId).First();
        }

        private async Task<LlmCompositionResult?> TryGetOpenRouterCompositionAsync(
    RecommendationCompositionRequest request,
    List<NailShape> shapes,
    List<NailSurface> surfaces)
        {
            var provider = _nemotronConfiguration.LlmProvider;
            var apiKey = _nemotronConfiguration.OpenRouterApiKey;
            var model = _nemotronConfiguration.OpenRouterModel;
            var baseUrl = _nemotronConfiguration.OpenRouterBaseUrl;

            _logger.LogInformation(
                "[LLM] Attempting LLM recommendation. Provider: {Provider}, Model: {Model}",
                provider,
                model);

            try
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, baseUrl);

                httpRequest.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                httpRequest.Content = new StringContent(
                    JsonSerializer.Serialize(
                        BuildOpenRouterRequest(model, request, shapes, surfaces),
                        LlmJsonOptions),
                    Encoding.UTF8,
                    "application/json");

                _logger.LogInformation(
                    "[LLM] Sending request to {BaseUrl}",
                    baseUrl);

                using var response =
                    await OpenRouterHttpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "[LLM] Request failed. StatusCode: {StatusCode}. FALLBACK will be used.",
                        response.StatusCode);

                    return null;
                }

                var body = await response.Content.ReadAsStringAsync();

                var content = ExtractOpenRouterMessageContent(body);

                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning(
                        "[LLM] Response contained no usable content. FALLBACK will be used.");

                    return null;
                }

                var result = ParseLlmCompositionResult(content);

                if (result == null)
                {
                    _logger.LogWarning(
                        "[LLM] Failed to parse LLM response. FALLBACK will be used.");

                    return null;
                }

                _logger.LogInformation(
                    "[LLM] SUCCESS. Nemotron/LLM recommendation received. ShapeId: {ShapeId}, SurfaceId: {SurfaceId}",
                    result.NailShapeId,
                    result.NailSurfaceId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[LLM] Exception while calling LLM. FALLBACK will be used.");

                return null;
            }
        }

        private object BuildOpenRouterRequest(
            string model,
            RecommendationCompositionRequest request,
            List<NailShape> shapes,
            List<NailSurface> surfaces)
        {
            var prompt = BuildCompositionPrompt(request, shapes, surfaces);
            return new
            {
                model,
                temperature = 0.7,
                top_p = 0.9,
                max_tokens = 800,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a nail recommendation engine. Return only valid JSON. Use only IDs from the provided catalog."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };
        }

        private string BuildCompositionPrompt(
            RecommendationCompositionRequest request,
            List<NailShape> shapes,
            List<NailSurface> surfaces)
        {
            var catalog = new
            {
                shapes = shapes.Select(shape => new { id = shape.NailShapeId, shape.Name }),
                surfaces = surfaces.Select(surface => new { id = surface.NailSurfaceId, surface.Name, surface.FinishType, surface.Price }),
            };

            var profile = new
            {
                request.SkinTone,
                request.SkinShade,
                request.HandShape,
                request.Occupation,
                request.NailCondition,
                request.PreferredColors,
                request.PreferredStyles,
                request.PreferredOccasions,
                request.PreferredNailShapeId,
                request.PreferredComplexity
            };

            var builder = new StringBuilder();
            builder.AppendLine("Customer profile:");
            builder.AppendLine(JsonSerializer.Serialize(profile));
            builder.AppendLine();
            builder.AppendLine("Available catalog:");
            builder.AppendLine(JsonSerializer.Serialize(catalog));
            builder.AppendLine();
            builder.AppendLine("Choose one nail shape, one nail surface, and two or three colors.");
            builder.AppendLine("Respect preferredNailShapeId when it exists in the catalog.");
            builder.AppendLine("Colors must be #RRGGBB hex values.");
            builder.AppendLine();
            builder.AppendLine("Return only this JSON shape:");
            builder.AppendLine("{");
            builder.AppendLine("  \"nail_shape_id\": 1,");
            builder.AppendLine("  \"surface_id\": 1,");
            builder.AppendLine("  \"colors\": [\"#F5F5DC\"],");
            builder.AppendLine("}");
            return builder.ToString();
        }

        private string? ExtractOpenRouterMessageContent(string body)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (!root.TryGetProperty("choices", out var choices) || choices.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var first = choices.EnumerateArray().FirstOrDefault();
            if (first.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            if (!first.TryGetProperty("message", out var message))
            {
                return null;
            }

            if (!message.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            return content.GetString();
        }

        private LlmCompositionResult? ParseLlmCompositionResult(string content)
        {
            var json = ExtractJsonObject(content);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var result = new LlmCompositionResult();

                result.NailShapeId = GetNullableInt(root, "nail_shape_id") ?? GetNullableInt(root, "nailShapeId");
                result.NailSurfaceId = GetNullableInt(root, "surface_id") ?? GetNullableInt(root, "nailSurfaceId");

                if (root.TryGetProperty("colors", out var colors))
                {
                    result.Colors = ReadStringArray(colors);
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        private string? ExtractJsonObject(string content)
        {
            var trimmed = content.Trim();
            var start = trimmed.IndexOf('{');
            var end = trimmed.LastIndexOf('}');

            if (start < 0 || end <= start)
            {
                return null;
            }

            return trimmed.Substring(start, end - start + 1);
        }

        private int? GetNullableInt(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var value))
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var intValue))
            {
                return intValue;
            }

            if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out intValue))
            {
                return intValue;
            }

            return null;
        }

        private List<int> ReadIntArray(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Array)
            {
                return new List<int>();
            }

            return element.EnumerateArray()
                .Select(item => item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out var value) ? value : (int?)null)
                .Where(value => value.HasValue)
                .Select(value => value!.Value)
                .ToList();
        }

        private List<string> ReadStringArray(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Array)
            {
                return new List<string>();
            }

            return element.EnumerateArray()
                .Where(item => item.ValueKind == JsonValueKind.String)
                .Select(item => item.GetString())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!)
                .ToList();
        }

        private NailSurface SelectRecommendedSurface(RecommendationCompositionRequest request, List<NailSurface> surfaces)
        {
            var preferredStyles = request.PreferredStyles ?? new List<string>();

            if (preferredStyles.Any(style => style.Contains("matte", StringComparison.OrdinalIgnoreCase)))
            {
                var matteSurface = surfaces.FirstOrDefault(surface =>
                    surface.Name.Contains("matte", StringComparison.OrdinalIgnoreCase)
                    || surface.FinishType.Contains("matte", StringComparison.OrdinalIgnoreCase));
                if (matteSurface != null) return matteSurface;
            }

            if (preferredStyles.Any(style => style.Contains("glitter", StringComparison.OrdinalIgnoreCase)))
            {
                var glitterSurface = surfaces.FirstOrDefault(surface =>
                    surface.Name.Contains("glitter", StringComparison.OrdinalIgnoreCase)
                    || surface.FinishType.Contains("glitter", StringComparison.OrdinalIgnoreCase));
                if (glitterSurface != null) return glitterSurface;
            }

            var glossySurface = surfaces.FirstOrDefault(surface =>
                surface.Name.Contains("gloss", StringComparison.OrdinalIgnoreCase)
                || surface.FinishType.Contains("gloss", StringComparison.OrdinalIgnoreCase));

            return glossySurface ?? surfaces.OrderBy(surface => surface.NailSurfaceId).First();
        }

        private NailShape ResolveRecommendedShape(
    LlmCompositionResult? llmResult,
    RecommendationCompositionRequest request,
    List<NailShape> shapes)
        {
            if (request.PreferredNailShapeId.HasValue)
            {
                var preferredShape = shapes.FirstOrDefault(
                    shape => shape.NailShapeId == request.PreferredNailShapeId.Value);

                if (preferredShape != null)
                {
                    _logger.LogInformation(
                        "[RECOMMENDATION] Using user's preferred nail shape: {ShapeId}",
                        preferredShape.NailShapeId);

                    return preferredShape;
                }
            }

            if (llmResult?.NailShapeId.HasValue == true)
            {
                var llmShape = shapes.FirstOrDefault(
                    shape => shape.NailShapeId == llmResult.NailShapeId.Value);

                if (llmShape != null)
                {
                    _logger.LogInformation(
                        "[RECOMMENDATION] Using LLM recommended shape: {ShapeId}",
                        llmShape.NailShapeId);

                    return llmShape;
                }
            }

            _logger.LogInformation(
                "[RECOMMENDATION] Using RULE-BASED FALLBACK for nail shape.");

            return SelectRecommendedShape(request, shapes);
        }

        private NailSurface ResolveRecommendedSurface(
    LlmCompositionResult? llmResult,
    RecommendationCompositionRequest request,
    List<NailSurface> surfaces)
        {
            if (llmResult?.NailSurfaceId.HasValue == true)
            {
                var llmSurface = surfaces.FirstOrDefault(
                    surface => surface.NailSurfaceId == llmResult.NailSurfaceId.Value);

                if (llmSurface != null)
                {
                    _logger.LogInformation(
                        "[RECOMMENDATION] Using LLM recommended surface: {SurfaceId}",
                        llmSurface.NailSurfaceId);

                    return llmSurface;
                }
            }

            _logger.LogInformation(
                "[RECOMMENDATION] Using RULE-BASED FALLBACK for nail surface.");

            return SelectRecommendedSurface(request, surfaces);
        }

        private List<string> ResolveRecommendedColors(LlmCompositionResult? llmResult, RecommendationCompositionRequest request)
        {
            var llmColors = (llmResult?.Colors ?? new List<string>())
                .Where(IsHexColor)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return BuildRandomizedColors(request, llmColors);
        }

        private List<string> BuildRandomizedColors(RecommendationCompositionRequest request, List<string> suggestedColors)
        {
            var preferredColors = (request.PreferredColors ?? new List<string>())
                .Where(IsHexColor)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var palette = new List<string>();
            if (!string.IsNullOrWhiteSpace(request.SkinTone)
                && request.SkinTone.Contains("warm", StringComparison.OrdinalIgnoreCase))
            {
                palette.AddRange(new[] { "#F5F5DC", "#D4A5A5", "#C9B1A0", "#E8C39E", "#B76E79", "#C8A951", "#F2D6B3" });
            }
            else
            {
                palette.AddRange(new[] { "#FFFFFF", "#F2D7E6", "#C7D8F4", "#D9E7E2", "#D8C7F4", "#B7C9E2", "#E7E7EA" });
            }

            palette.AddRange(suggestedColors.Where(IsHexColor));
            palette.AddRange(preferredColors);

            var colors = new List<string>();
            if (preferredColors.Any())
            {
                colors.Add(preferredColors[Random.Shared.Next(preferredColors.Count)]);
            }

            foreach (var color in palette
                .Where(color => !colors.Contains(color, StringComparer.OrdinalIgnoreCase))
                .OrderBy(_ => Random.Shared.Next()))
            {
                colors.Add(color);
                if (colors.Count == 3)
                {
                    break;
                }
            }

            return colors
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();
        }

        private bool IsHexColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) return false;

            var value = color.Trim();
            if (!value.StartsWith("#") || value.Length != 7) return false;

            return value.Skip(1).All(Uri.IsHexDigit);
        }

        private sealed class LlmCompositionResult
        {
            public int? NailShapeId { get; set; }
            public int? NailSurfaceId { get; set; }
            public List<string> Colors { get; set; } = new List<string>();
        }

    }
}
