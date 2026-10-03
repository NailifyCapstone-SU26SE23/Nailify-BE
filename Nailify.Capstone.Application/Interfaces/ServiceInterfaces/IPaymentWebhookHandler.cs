
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IPaymentWebhookHandler
    {
        /// <summary>
        /// 1. Kiểm tra xem Handler này có nhận xử lý giao dịch này hay không?
        /// (Dựa vào PaymentType = "WalletDeposit" hay "BookingPayment")
        /// </summary>
        bool CanHandle(Transaction transaction);
        /// <summary>
        /// 2. Thực thi nghiệp vụ xử lý khi thanh toán thành công
        /// </summary>
        Task HandlePaidTransactionAsync(Transaction transaction, PaymentWebhookDto webhookDto);
    }
}
