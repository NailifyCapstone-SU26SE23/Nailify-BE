using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.TransactionResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System.Text.Json;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class RefundService : IRefundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RefundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PayoutResult> RefundToWalletByBookingAsync(Guid bookingId, string? reason = null, bool forceFullRefund = false)
        {
            try
            {
                /*
                var transaction = await _unitOfWork.TransactionRepository
                    .FindByCondition(t => t.BookingId == bookingId && t.Status == TransactionStatus.Paid, trackChanges: true)
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.User)
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.Salon)
                    .OrderByDescending(t => t.PaidAt ?? t.CreatedAt)
                    .FirstOrDefaultAsync();
                */


                // ThanhDT
                var booking = await _unitOfWork.BookingRepository.FindByCondition(x => x.BookingId == bookingId, true)
                   .Include(x => x.Customer)
                        .ThenInclude(x => x.User)
                   .Include(x => x.Salon)
                   .FirstOrDefaultAsync();
                if (booking == null)
                {
                    return new PayoutResult
                    {
                        Success = false,
                        Message = "Booking not found"
                    };
                }
                return await RefundToWalletAsync(booking, reason, forceFullRefund);
            }
            catch (Exception ex)
            {
                return new PayoutResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        private async Task<PayoutResult> RefundToWalletAsync(Booking booking, string? reason, bool forceFullRefund)
        {
            if (booking.IsRefunded)
            {
                return new PayoutResult
                {
                    Success = false,
                    Message = "Booking has already been refunded"
                };
            }

            var existingRefund = await _unitOfWork.TransactionRepository.FindByCondition(x => x.BookingId == booking.BookingId && x.Status == TransactionStatus.Refunded).FirstOrDefaultAsync();
            if(existingRefund != null)
            {
                return new PayoutResult
                {
                    Success = false,
                    Message = "Transaction has already been refunded"
                };
            }

            // Lấy phần thanh toán bằng ví (nếu có)
            var walletPaymentTx = await _unitOfWork.WalletTransactionRepository.FindByCondition(x => x.ReferenceId == booking.BookingId.ToString() && x.Type == WalletTransactionType.BookingPayment).FirstOrDefaultAsync();

            // Lấy phần thanh toán qua cổng PayOS (nếu có)
            var gatewayPaymentTx = await _unitOfWork.TransactionRepository.FindByCondition(x => x.BookingId == booking.BookingId && x.Status == TransactionStatus.Paid, true).OrderByDescending(x => x.PaidAt ?? x.CreatedAt).FirstOrDefaultAsync();

            decimal walletPaidAmount = walletPaymentTx != null ? Math.Abs(walletPaymentTx.Amount) : 0m;
            decimal gatewayPaidAmount = gatewayPaymentTx != null ? gatewayPaymentTx.Amount : 0m;
            decimal totalPaid = walletPaidAmount + gatewayPaidAmount;

            if(totalPaid <= 0m)
            {
                return new PayoutResult
                {
                    Success = false,
                    Message = "Paid transaction not found for this booking"
                };
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Tính toán tổng số tiền được hoàn (vd: 100% hoặc 80% của totalPaid)
                var refundPolicy = CalculateRefundPolicy(booking, totalPaid, reason, forceFullRefund);

                // Chia tỷ lệ hoàn cho phần Ví và phần
                decimal ratio = totalPaid > 0 ? refundPolicy.Amount / totalPaid : 0;

                // Tiền hoàn cho phần Ví = Tiền trả bằng Ví * Tỷ lệ hoàn
                decimal refundWalletPortion = decimal.Round(walletPaidAmount * ratio, 0, MidpointRounding.AwayFromZero);

                // Tiền hoàn cho phần Cổng = Tổng số tiền hoàn - Tiền hoàn phần Ví (tránh sai số làm)
                decimal refundGatewayPortion = refundPolicy.Amount - refundWalletPortion;

                var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdForUpdateAsync(booking.CustomerId);

                if(wallet == null)
                {
                    wallet = new CustomerWallet
                    {
                        CustomerId = booking.CustomerId,
                        Balance = 0m,
                        FrozenBalance = 0m,
                        Status = WalletStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.CustomerWalletRepository.CreateAsync(wallet);
                    await _unitOfWork.SaveChangesAsync();
                }

                Transaction? refundTransaction = null;
                var balanceBefore = wallet.Balance;

                if (refundWalletPortion > 0)
                {
                    wallet.Balance += refundWalletPortion;
                    var wtWalletRefund = new WalletTransaction
                    {
                        WalletId = wallet.WalletId,
                        Amount = refundWalletPortion,
                        BalanceBefore = balanceBefore,
                        BalanceAfter = wallet.Balance,
                        Type = WalletTransactionType.BookingRefund,
                        Status = WalletTransactionStatus.Completed,
                        ReferenceId = booking.BookingId.ToString(),
                        ReferenceType = WalletReferenceType.Booking,
                        Description = $"Hoàn phần tiền thanh toán bằng ví của booking {booking.BookingId}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.WalletTransactionRepository.CreateAsync(wtWalletRefund);
                    balanceBefore = wallet.Balance;
                }
                if(refundGatewayPortion > 0)
                {
                    string orderCode = gatewayPaymentTx != null
                     ? $"RF-{gatewayPaymentTx.OrderCode}"
                     : $"RF-{new Random().Next(100000, 999999)}";

                    refundTransaction = new Transaction
                    {
                        BookingId = booking.BookingId,
                        WalletId = wallet.WalletId,
                        PaymentType = PaymentType.WalletDeposit,
                        OrderCode = orderCode,
                        Amount = refundGatewayPortion,
                        Reference = wallet.WalletId.ToString(),
                        PaymentLinkId = gatewayPaymentTx?.PaymentLinkId,
                        CheckoutUrl = string.Empty,
                        QrCode = string.Empty,
                        Status = TransactionStatus.Refunded,
                        Policy = refundPolicy.PolicyText,
                        CreatedAt = DateTime.UtcNow,
                        PaidAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow,
                        WebhookPayload = JsonSerializer.Serialize(new
                        {
                            Type = "WalletRefund",
                            WalletId = wallet.WalletId,
                            OriginalTransactionId = gatewayPaymentTx?.TransactionId,
                            Reason = refundPolicy.PolicyText
                        }, JsonOptions)
                    };
                    refundTransaction.Booking = booking;

                    wallet.Balance += refundGatewayPortion;
                    var wtGatewayRefund = new WalletTransaction
                    {
                        WalletId = wallet.WalletId,
                        Amount = refundGatewayPortion,
                        BalanceBefore = balanceBefore,
                        BalanceAfter = wallet.Balance,
                        Type = WalletTransactionType.BookingRefund,
                        Status = WalletTransactionStatus.Completed,
                        ReferenceId = booking.BookingId.ToString(),
                        ReferenceType = WalletReferenceType.Booking,
                        Description = $"Hoàn phần tiền thanh toán qua cổng của booking {booking.BookingId}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.WalletTransactionRepository.CreateAsync(wtGatewayRefund);
                    await _unitOfWork.TransactionRepository.CreateAsync(refundTransaction);
                }
                wallet.UpdatedAt = DateTime.UtcNow;
                booking.IsRefunded = true;

                _unitOfWork.CustomerWalletRepository.Update(wallet);
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new PayoutResult
                {
                    Success = true,
                    TransactionId = refundTransaction?.TransactionId.ToString() ?? "Wallet-Only-Refund",
                    Message = "Hoàn tiền vaoof ví khách hàng thành công.",
                    Transaction = refundTransaction != null ? ToTransactionResponse(refundTransaction) : null
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        /*
        private async Task<PayoutResult> RefundToWalletAsync(Transaction paidTransaction, string? reason, bool forceFullRefund)
        {
            if (paidTransaction.Booking == null)
            {
                return new PayoutResult
                {
                    Success = false,
                    Message = "Booking not found for this transaction"
                };
            }
            // ThanhDT
            if (paidTransaction.Booking.IsRefunded)
            {
                return new PayoutResult
                {
                    Success = false,
                    Message = "Booking has already been refunded"
                };
            }

            // ThanhDT
            var existingRefund = await _unitOfWork.TransactionRepository
                .FindByCondition(t => t.BookingId == paidTransaction.BookingId && t.Status == TransactionStatus.Refunded)
                .FirstOrDefaultAsync();

            if (existingRefund != null)
            {
                return new PayoutResult
                {
                    Success = false,
                    Message = "Transaction has already been refunded"
                };
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var refundPolicy = CalculateRefundPolicy(paidTransaction.Booking, paidTransaction.Amount, reason, forceFullRefund);
                var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdForUpdateAsync(paidTransaction.Booking.CustomerId);
                if (wallet == null)
                {
                    wallet = new CustomerWallet
                    {
                        CustomerId = paidTransaction.Booking.CustomerId,
                        Balance = 0m,
                        FrozenBalance = 0m,
                        Status = WalletStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.CustomerWalletRepository.CreateAsync(wallet);
                    await _unitOfWork.SaveChangesAsync();
                }

                var refundTransaction = new Transaction
                {
                    BookingId = paidTransaction.BookingId,
                    WalletId = wallet.WalletId,
                    PaymentType = PaymentType.WalletDeposit,
                    OrderCode = $"RF-{paidTransaction.OrderCode}",
                    Amount = refundPolicy.Amount,
                    Reference = wallet.WalletId.ToString(),
                    PaymentLinkId = paidTransaction.PaymentLinkId,
                    CheckoutUrl = string.Empty,
                    QrCode = string.Empty,
                    Status = TransactionStatus.Refunded,
                    Policy = refundPolicy.PolicyText,
                    CreatedAt = DateTime.UtcNow,
                    PaidAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow,
                    WebhookPayload = JsonSerializer.Serialize(new
                    {
                        Type = "WalletRefund",
                        WalletId = wallet.WalletId,
                        OriginalTransactionId = paidTransaction.TransactionId,
                        Reason = refundPolicy.PolicyText
                    }, JsonOptions)
                };

                var balanceBefore = wallet.Balance;
                wallet.Balance += refundPolicy.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                var walletTransaction = new WalletTransaction
                {
                    WalletId = wallet.WalletId,
                    Amount = refundPolicy.Amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.Balance,
                    Type = WalletTransactionType.BookingRefund,
                    Status = WalletTransactionStatus.Completed,
                    ReferenceId = paidTransaction.BookingId?.ToString(),
                    ReferenceType = WalletReferenceType.Booking,
                    Description = $"Hoàn tiền booking {paidTransaction.BookingId}",
                    CreatedAt = DateTime.UtcNow
                };

                paidTransaction.Booking.IsRefunded = true;
                refundTransaction.Booking = paidTransaction.Booking;

                _unitOfWork.CustomerWalletRepository.Update(wallet);
                await _unitOfWork.WalletTransactionRepository.CreateAsync(walletTransaction);
                await _unitOfWork.TransactionRepository.CreateAsync(refundTransaction);
                _unitOfWork.BookingRepository.Update(paidTransaction.Booking);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new PayoutResult
                {
                    Success = true,
                    TransactionId = refundTransaction.TransactionId.ToString(),
                    Message = "Hoàn tiền vào ví khách hàng thành công.",
                    Transaction = ToTransactionResponse(refundTransaction)
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        */
        private static RefundPolicy CalculateRefundPolicy(Booking booking, decimal originalAmount, string? reason, bool forceFullRefund)
        {
            if (forceFullRefund)
            {
                return new RefundPolicy(
                    originalAmount,
                    string.IsNullOrWhiteSpace(reason) ? "Hoàn tiền toàn bộ do Salon hủy lịch." : reason);
            }

            var localBookingDate = booking.BookingDate.Kind == DateTimeKind.Utc
                ? booking.BookingDate.AddHours(7).Date
                : booking.BookingDate.Date;
            var bookingDateTime = localBookingDate.Add(booking.StartTime);
            var currentLocalTime = DateTime.UtcNow.AddHours(7);
            var hoursUntilBooking = (bookingDateTime - currentLocalTime).TotalHours;
            if (hoursUntilBooking < 24)
            {
                return new RefundPolicy(
                    decimal.Round(originalAmount * 0.8m, 0, MidpointRounding.AwayFromZero),
                    "Hoàn 80% tiền cọc cho yêu cầu hoàn tiền dưới 24 giờ trước thời gian đặt lịch.");
            }

            return new RefundPolicy(
                originalAmount,
                "Hoàn toàn bộ tiền cọc cho yêu cầu hoàn tiền trên 24 giờ trước thời gian đặt lịch.");
        }

        private static TransactionResponseDto ToTransactionResponse(Transaction transaction)
        {
            return new TransactionResponseDto
            {
                TransactionId = transaction.TransactionId,
                BookingId = transaction.BookingId,
                OrderCode = transaction.OrderCode,
                Amount = transaction.Amount,
                Reference = transaction.Reference,
                PaymentLinkId = transaction.PaymentLinkId,
                Policy = transaction.Policy,
                CheckoutUrl = transaction.CheckoutUrl,
                QrCode = transaction.QrCode,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt,
                PaidAt = transaction.PaidAt,
                ExpiresAt = transaction.ExpiresAt,
                CustomerId = transaction.Booking!.CustomerId,
                CustomerName = $"{transaction.Booking.Customer.User.FirstName} {transaction.Booking.Customer.User.LastName}".Trim(),
                SalonId = transaction.Booking.SalonId,
                SalonName = transaction.Booking.Salon.Name
            };
        }

        private sealed record RefundPolicy(decimal Amount, string PolicyText);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
