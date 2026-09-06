namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ISentimentAnalysisService
    {
        /// <summary>
        /// Phân tích cảm xúc nhận xét của khách hàng qua Python AI Microservice.
        /// Trả về true nếu nhận xét là Tiêu cực (Negative).
        /// </summary>
        Task<bool> IsNegativeReviewAsync(string comment);
    }
}
