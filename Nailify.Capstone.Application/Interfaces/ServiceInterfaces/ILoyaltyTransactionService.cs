using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTransactionRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ILoyaltyTransactionService
    {
        Task<ApiResult<PagedList<LoyaltyTransactionDto>>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Guid? userId = null);
        Task<ApiResult<LoyaltyTransactionDto>> GetByIdAsync(int id);
        Task<ApiResult<LoyaltyTransactionDto>> UpdateAsync(int id, LoyaltyTransactionUpdateRequest request);

        /// <summary>
        ///  Khi khách đặt xong (có dùng điểm) thì sẽ trừ điểm của khách và tạo 1 transaction mới nhưng khách hủy nên vào sẽ refund lại điểm cho khách và tạo 1 transaction mới
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="bookingId"></param>
        /// <param name="pointsToRefund"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task<ApiResult<LoyaltyTransactionDto>> RefundPointsToWalletAsync(Guid customerId, Guid? bookingId, int pointsToRefund, string reason);
        Task<ApiResult<bool>> RevertEarnedPointsAsync(Guid customerId, Guid bookingId, string reason);
        Task<ApiResult<WalletSummaryDTO>> GetWalletSummaryAsync(Guid customerId);
    }
}
