using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SentimentAnalysisService> _logger;

        public SentimentAnalysisService(HttpClient httpClient, ILogger<SentimentAnalysisService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;

            string baseUrl = configuration["PythonAIService:SentimentBaseUrl"] ?? "http://localhost:8001";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<bool> IsNegativeReviewAsync(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return false;

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/analyze-sentiment", new { comment });
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SentimentResultDTO>();
                    return result?.IsNegative ?? false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi gọi Python Sentiment AI Service");
            }

            return false;
        }

        private class SentimentResultDTO
        {
            [JsonPropertyName("sentiment")]
            public string Sentiment { get; set; } = string.Empty;

            [JsonPropertyName("confidence")]
            public double Confidence { get; set; }

            [JsonPropertyName("is_negative")]
            public bool IsNegative { get; set; }
        }
    }
}
