using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.TransactionResponseDTOs;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.Configuration.PayOS;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using static Nailify.Capstone.Infrastructure.Configuration.PayOS.PayOutDto;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class RefundService
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _payoutClientId;
        private readonly string _payoutApiKey;
        private readonly string _payoutCheckSumKey;
        private readonly string _baseUrl = "https://api-merchant.payos.vn";

        public RefundService(
            IPayOSSettings payOSSettings,
            IHttpClientFactory httpClientFactory,
            IUnitOfWork unitOfWork)
        {
            _httpClient = httpClientFactory.CreateClient();
            _unitOfWork = unitOfWork;
            _payoutClientId = payOSSettings.PayoutClientId;
            _payoutApiKey = payOSSettings.PayoutApiKey;
            _payoutCheckSumKey = payOSSettings.PayoutChecksumKey;

            if (string.IsNullOrEmpty(_payoutClientId) || string.IsNullOrEmpty(_payoutApiKey))
            {
                throw new InvalidOperationException("PayOS payout credentials are not configured");
            }
        }

        public async Task<PayoutResult> CreateSinglePayoutByBookingAsync(Guid bookingId, BankAccountInfo bankInfo, string? reason = null)
        {
            try
            {
                var transaction = await _unitOfWork.TransactionRepository
                    .FindByCondition(t => t.BookingId == bookingId && t.Status == TransactionStatus.Paid, trackChanges: true)
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.Customer)
                        .ThenInclude(c => c.User)
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.Salon)
                    .OrderByDescending(t => t.PaidAt ?? t.CreatedAt)
                    .FirstOrDefaultAsync();

                if (transaction == null)
                {
                    return new PayoutResult
                    {
                        Success = false,
                        Message = "Paid transaction not found for this booking"
                    };
                }

                return await CreateSinglePayoutAsync(transaction, bankInfo, reason);
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

        private async Task<PayoutResult> CreateSinglePayoutAsync(Transaction paidTransaction, BankAccountInfo bankInfo, string? reason)
        {
            if (paidTransaction.Booking.IsRefunded)
            {
                return new PayoutResult
                {
                    Success = false,
                    Message = "Booking has already been refunded"
                };
            }

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

            var refundPolicy = CalculateRefundPolicy(paidTransaction.Booking, paidTransaction.Amount);
            var referenceId = $"transaction_{paidTransaction.TransactionId}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            var payoutRequest = new
            {
                referenceId,
                amount = (int)Math.Round(refundPolicy.Amount, MidpointRounding.AwayFromZero),
                description = $"Refund transaction {paidTransaction.TransactionId}",
                toBin = GetBankBin(bankInfo.BankCode),
                toAccountNumber = bankInfo.AccountNumber,
                category = new[] { "refund" }
            };

            var response = await SendPayOSRequestAsync(
                "/v1/payouts",
                payoutRequest,
                idempotencyKey: referenceId);

            var responseContent = await response.Content.ReadAsStringAsync();
            var payoutResponse = JsonSerializer.Deserialize<PayOSPayoutResponse>(
                responseContent,
                JsonOptions);

            if (payoutResponse?.Code == "00" && payoutResponse.Data != null)
            {
                var refundTransaction = new Transaction
                {
                    BookingId = paidTransaction.BookingId,
                    OrderCode = $"RF-{paidTransaction.OrderCode}",
                    Amount = refundPolicy.Amount,
                    Reference = payoutResponse.Data.Id,
                    PaymentLinkId = paidTransaction.PaymentLinkId,
                    CheckoutUrl = string.Empty,
                    QrCode = string.Empty,
                    Status = TransactionStatus.Refunded,
                    Policy = refundPolicy.PolicyText,
                    CreatedAt = DateTime.UtcNow,
                    PaidAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow,
                    WebhookPayload = responseContent
                };

                paidTransaction.Booking.IsRefunded = true;
                refundTransaction.Booking = paidTransaction.Booking;

                await _unitOfWork.TransactionRepository.CreateAsync(refundTransaction);
                _unitOfWork.BookingRepository.Update(paidTransaction.Booking);
                await _unitOfWork.SaveChangesAsync();

                return new PayoutResult
                {
                    Success = true,
                    TransactionId = payoutResponse.Data.Id,
                    Message = payoutResponse.Desc ?? "Payout initiated successfully",
                    Transaction = ToTransactionResponse(refundTransaction)
                };
            }

            return new PayoutResult
            {
                Success = false,
                Message = payoutResponse?.Desc ?? "PayOS payout failed",
                ErrorCode = payoutResponse?.Code ?? string.Empty
            };
        }

        public async Task<decimal> GetAccountBalanceAsync()
        {
            var response = await SendPayOSRequestAsync("/v1/payouts-account/balance", null, HttpMethod.Get);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get account balance: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var balanceResponse = JsonSerializer.Deserialize<PayOSBalanceResponse>(content, JsonOptions)
                ?? throw new InvalidOperationException("Invalid balance response");

            return decimal.Parse(balanceResponse.Data.Balance);
        }

        private static RefundPolicy CalculateRefundPolicy(Booking booking, decimal originalAmount)
        {
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
                    "Refund requested less than 24 hours before booking time: 80% refund.");
            }

            return new RefundPolicy(
                originalAmount,
                "Refund requested at least 24 hours before booking time: full refund.");
        }

        private string GetBankBin(string bankCode)
        {
            var bankBins = new Dictionary<string, string>
            {
                { "VCB", "970436" }, { "BIDV", "970418" }, { "VIB", "970441" },
                { "MB", "970422" }, { "TCB", "970407" }, { "ACB", "970416" },
                { "VPB", "970432" }, { "TPB", "970423" }, { "HDB", "970437" },
                { "MSB", "970426" }, { "SCB", "970429" }, { "OCB", "970448" },
                { "SHB", "970443" }, { "EIB", "970431" }, { "VAB", "970425" },
                { "NAB", "970428" }, { "BAB", "970409" }, { "PGB", "970430" },
                { "GPB", "970408" }, { "AGB", "970405" }, { "LVB", "970434" },
                { "KLB", "970452" }, { "VBSP", "970427" }
            };

            return bankBins.GetValueOrDefault(bankCode.ToUpperInvariant(), "970436");
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
                CustomerId = transaction.Booking.CustomerId,
                CustomerName = $"{transaction.Booking.Customer.User.FirstName} {transaction.Booking.Customer.User.LastName}".Trim(),
                SalonId = transaction.Booking.SalonId,
                SalonName = transaction.Booking.Salon.Name
            };
        }

        private async Task<HttpResponseMessage> SendPayOSRequestAsync(
            string endpoint,
            object? data = null,
            HttpMethod? method = null,
            string? idempotencyKey = null)
        {
            method ??= data == null ? HttpMethod.Get : HttpMethod.Post;
            var url = $"{_baseUrl}{endpoint}";

            using var request = new HttpRequestMessage(method, url);
            request.Headers.Add("x-client-id", _payoutClientId);
            request.Headers.Add("x-api-key", _payoutApiKey);

            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                request.Headers.Add("x-idempotency-key", idempotencyKey);
            }

            var signature = GeneratePayoutSignature(data);
            request.Headers.Add("x-signature", signature);

            if (data != null)
            {
                var json = JsonSerializer.Serialize(data, JsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return await _httpClient.SendAsync(request);
        }

        private string GeneratePayoutSignature(object? data)
        {
            if (data == null) return string.Empty;

            var json = JsonSerializer.Serialize(data, JsonOptions);
            var dataDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                ?? new Dictionary<string, JsonElement>();

            var queryString = string.Join("&", dataDict.OrderBy(kv => kv.Key).Select(kv =>
            {
                var key = Uri.EscapeDataString(kv.Key);
                var value = kv.Value.ValueKind switch
                {
                    JsonValueKind.Array => Uri.EscapeDataString(kv.Value.ToString()),
                    JsonValueKind.Object => Uri.EscapeDataString(kv.Value.ToString()),
                    JsonValueKind.String => Uri.EscapeDataString(kv.Value.GetString() ?? string.Empty),
                    _ => Uri.EscapeDataString(kv.Value.ToString())
                };
                return $"{key}={value}";
            }));

            using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(_payoutCheckSumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private sealed record RefundPolicy(decimal Amount, string PolicyText);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
