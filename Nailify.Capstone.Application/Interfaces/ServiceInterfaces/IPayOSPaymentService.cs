using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IPayOSPaymentService
    {
        Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreateBookingPaymentLinkAsync(Guid bookingId);
        Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreateBookingRequestPaymentLinkAsync(Guid customerId, CreateBookingRequestDTO request);
        Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreateWalletDepositPaymentLinkAsync(Guid walletId, decimal amount);
        Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreateDynamicPaymentLinkAsync(PayOSPaymentContextRequest request);
        Task<(bool Success, string Message)> HandlePaymentWebhookAsync(PaymentWebhookDto webhookDto);
        Task<(bool Success, string Message, string? Status)> GetPaymentStatusAsync(long orderCode);
        Task<(bool Success, string Message)> CancelPaymentLinkAsync(long orderCode);
    }
}
