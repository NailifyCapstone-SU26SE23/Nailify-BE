using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IRefundService
    {
        Task<PayoutResult> RefundToWalletByBookingAsync(Guid bookingId, string? reason = null, bool forceFullRefund = false);
    }
}
