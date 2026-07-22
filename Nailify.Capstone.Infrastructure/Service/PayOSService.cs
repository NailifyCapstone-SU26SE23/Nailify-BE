using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class PayOSService
    {
        private readonly HttpClient _httpClient;
        private readonly IPayOSSettings _paymentSettings;
        private readonly IPaymentUrls _paymentUrls;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOSHelper _payOSHelper;
        private const string PayOSBaseUrl = "https://api-merchant.payos.vn";

        public PayOSService(
            IHttpClientFactory httpClientFactory,
            IPayOSSettings paymentSettings,
            IPaymentUrls paymentUrls,
            IUnitOfWork unitOfWork,
            PayOSHelper payOSHelper)
        {
            _httpClient = httpClientFactory.CreateClient();
            _paymentSettings = paymentSettings;
            _paymentUrls = paymentUrls;
            _unitOfWork = unitOfWork;
            _payOSHelper = payOSHelper;
        }

        public async Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreatePaymentLinkAsync(Guid bookingId)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    return (false, "Khong tim thay lich hen.", null);
                }

                var amountDue = booking.Status == BookingStatus.ServiceCompleted
                    ? booking.AmountDue ?? booking.TotalPrice ?? 0m
                    : booking.TotalPrice * 0.2m ?? 0m;
                if (amountDue <= 0)
                {
                    return (false, $"So tien khong hop le: {amountDue}.", null);
                }

                var existing = await _unitOfWork.TransactionRepository
                    .FindByCondition(t => t.BookingId == bookingId && t.Status == TransactionStatus.Pending)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();
                if (existing != null && existing.ExpiresAt > DateTime.UtcNow)
                {
                    return (true, "Link thanh toan da ton tai.", ToResponse(existing));
                }

                var orderCode = await _payOSHelper.GenerateUniqueOrderCodeAsync();
                var amount = (int)Math.Round(amountDue, MidpointRounding.AwayFromZero);
                var description = $"Thanh toan don {orderCode}";
                var itemName = $"Ma don {orderCode}";
                var signature = CreatePaymentRequestSignature(amount, description, orderCode);

                var paymentRequest = new
                {
                    orderCode,
                    amount,
                    description,
                    items = new[]
                    {
                        new
                        {
                            name = itemName,
                            quantity = 1,
                            price = amount
                        }
                    },
                    cancelUrl = _paymentUrls.CancelUrl,
                    returnUrl = _paymentUrls.ReturnUrl,
                    signature
                };

                using var content = new StringContent(JsonSerializer.Serialize(paymentRequest, JsonOptions), Encoding.UTF8, "application/json");
                ApplyAuthenticationHeaders();

                var response = await _httpClient.PostAsync($"{PayOSBaseUrl}/v2/payment-requests", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return (false, $"Loi tu PayOS: {responseContent}", null);
                }

                var paymentResult = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (paymentResult.TryGetProperty("code", out var codeElement) &&
                    codeElement.GetString() != "00")
                {
                    var desc = paymentResult.TryGetProperty("desc", out var descElement)
                        ? descElement.GetString()
                        : responseContent;
                    return (false, $"Loi tu PayOS - Code: {codeElement.GetString()}, Message: {desc}", null);
                }

                if (!paymentResult.TryGetProperty("data", out var data))
                {
                    return (false, $"PayOS response khong co data: {responseContent}", null);
                }

                var transaction = new Transaction
                {
                    BookingId = bookingId,
                    OrderCode = orderCode.ToString(CultureInfo.InvariantCulture),
                    Amount = amountDue,
                    PaymentLinkId = GetString(data, "paymentLinkId"),
                    CheckoutUrl = GetString(data, "checkoutUrl") ?? string.Empty,
                    QrCode = GetString(data, "qrCode") ?? string.Empty,
                    Status = ParseStatus(GetString(data, "status")),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    WebhookPayload = string.Empty
                };

                await _unitOfWork.TransactionRepository.CreateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                return (true, "Tao link thanh toan thanh cong!", ToResponse(transaction));
            }
            catch (Exception ex)
            {
                return (false, $"Loi khi tao link thanh toan: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> HandlePaymentWebhookAsync(PaymentWebhookDto webhookDto)
        {
            try
            {
                if (!VerifyWebhookSignature(webhookDto))
                {
                    return (false, "Webhook signature khong hop le.");
                }

                var transaction = await _unitOfWork.TransactionRepository.GetByOrderCodeAsync(
                    ResolveOrderCode(webhookDto).ToString(CultureInfo.InvariantCulture),
                    trackChanges: true);
                if (transaction == null)
                {
                    return (false, "Khong tim thay giao dich tuong ung.");
                }

                transaction.WebhookPayload = JsonSerializer.Serialize(webhookDto, JsonOptions);
                transaction.Reference = webhookDto.Data?.Reference;
                transaction.PaymentLinkId = webhookDto.Data?.PaymentLinkId ?? transaction.PaymentLinkId;
                transaction.Status = webhookDto.Code == "00" && webhookDto.Success
                    ? TransactionStatus.Paid
                    : TransactionStatus.Cancelled;
                transaction.PaidAt = transaction.Status == TransactionStatus.Paid ? DateTime.UtcNow : transaction.PaidAt;
                if (transaction.Status == TransactionStatus.Paid)
                {
                    transaction.Booking.AmountPaid = transaction.Amount;
                    transaction.Booking.AmountDue = transaction.Booking.TotalPrice - transaction.Booking.AmountPaid;
                }

                if (transaction.Booking.Status == BookingStatus.ServiceCompleted)
                {
                    transaction.Booking.CheckOut(Guid.Empty);
                }

                _unitOfWork.TransactionRepository.Update(transaction);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Xu ly webhook thanh cong.");
            }
            catch (Exception ex)
            {
                return (false, $"Loi khi xu ly webhook: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, string? Status)> GetPaymentStatusAsync(long orderCode)
        {
            try
            {
                ApplyAuthenticationHeaders();
                var response = await _httpClient.GetAsync($"{PayOSBaseUrl}/v2/payment-requests/{orderCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return (false, $"Loi tu PayOS: {responseContent}", null);
                }

                var paymentResult = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var data = paymentResult.GetProperty("data");
                var status = data.GetProperty("status").GetString();

                await SyncLocalTransactionStatusAsync(orderCode, status);

                return (true, "Lay trang thai thanh toan thanh cong!", status);
            }
            catch (Exception ex)
            {
                return (false, $"Loi khi lay trang thai thanh toan: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> CancelPaymentLinkAsync(long orderCode)
        {
            try
            {
                var transaction = await _unitOfWork.TransactionRepository.GetByOrderCodeAsync(orderCode.ToString(CultureInfo.InvariantCulture), trackChanges: true);
                if (transaction == null)
                {
                    return (false, "Khong tim thay giao dich tuong ung.");
                }

                if (transaction.Status == TransactionStatus.Paid)
                {
                    return (false, "Khong the huy giao dich da thanh toan.");
                }

                ApplyAuthenticationHeaders();
                var cancelRequest = new { cancellationReason = "Huy boi nguoi dung" };
                using var content = new StringContent(JsonSerializer.Serialize(cancelRequest), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{PayOSBaseUrl}/v2/payment-requests/{orderCode}/cancel", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return (false, $"Loi tu PayOS: {responseContent}");
                }

                transaction.Status = TransactionStatus.Cancelled;
                _unitOfWork.TransactionRepository.Update(transaction);
                await _unitOfWork.SaveChangesAsync();

                return (true, "Huy link thanh toan thanh cong!");
            }
            catch (Exception ex)
            {
                return (false, $"Loi khi huy link thanh toan: {ex.Message}");
            }
        }

        private string CreatePaymentRequestSignature(int amount, string description, long orderCode)
        {
            var signatureData = $"amount={amount}&cancelUrl={_paymentUrls.CancelUrl}&description={description}&orderCode={orderCode}&returnUrl={_paymentUrls.ReturnUrl}";
            return CreateHmacSha256(signatureData);
        }

        private bool VerifyWebhookSignature(PaymentWebhookDto webhookDto)
        {
            var data = webhookDto.Data;
            if (data == null || string.IsNullOrWhiteSpace(webhookDto.Signature))
            {
                return false;
            }

            var amount = data.Amount.ToString(CultureInfo.InvariantCulture);
            var dataStr = $"amount={amount}&code={data.Code}&desc={data.Desc}&orderCode={data.OrderCode}";
            var calculatedSignature = CreateHmacSha256(dataStr);
            return string.Equals(calculatedSignature, webhookDto.Signature, StringComparison.OrdinalIgnoreCase);
        }

        private string CreateHmacSha256(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_paymentSettings.ChecksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private void ApplyAuthenticationHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _paymentSettings.ClientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _paymentSettings.ApiKey);
        }

        private static long ResolveOrderCode(PaymentWebhookDto webhookDto)
        {
            return webhookDto.Data?.OrderCode > 0 ? webhookDto.Data.OrderCode : webhookDto.OrderCode;
        }

        private static TransactionStatus ParseStatus(string? status)
        {
            return status?.ToUpperInvariant() switch
            {
                "PAID" => TransactionStatus.Paid,
                "CANCELLED" or "CANCELED" => TransactionStatus.Cancelled,
                "EXPIRED" => TransactionStatus.Overdue,
                _ => TransactionStatus.Pending
            };
        }

        private async Task SyncLocalTransactionStatusAsync(long orderCode, string? payOSStatus)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetByOrderCodeAsync(
                orderCode.ToString(CultureInfo.InvariantCulture),
                trackChanges: true);

            if (transaction == null)
            {
                return;
            }

            var newStatus = ParseStatus(payOSStatus);
            if (transaction.Status == newStatus && (newStatus != TransactionStatus.Paid || transaction.PaidAt.HasValue))
            {
                return;
            }

            transaction.Status = newStatus;
            if (newStatus == TransactionStatus.Paid && !transaction.PaidAt.HasValue)
            {
                transaction.PaidAt = DateTime.UtcNow;
            }
            if (newStatus == TransactionStatus.Paid)
            {
                transaction.Booking.AmountPaid = transaction.Amount;
                transaction.Booking.AmountDue = transaction.Booking.TotalPrice - transaction.Booking.AmountPaid;
            }

            _unitOfWork.TransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        private PaymentResponseDto ToResponse(Transaction transaction)
        {
            return new PaymentResponseDto
            {
                PaymentUrl = transaction.CheckoutUrl,
                ReturnUrl = _paymentUrls.ReturnUrl,
                CancelUrl = _paymentUrls.CancelUrl,
                QrCode = transaction.QrCode,
                OrderCode = long.Parse(transaction.OrderCode, CultureInfo.InvariantCulture),
                Status = transaction.Status.ToString(),
                TransactionId = transaction.TransactionId,
                BookingId = transaction.BookingId,
                Amount = transaction.Amount
            };
        }

        private static string? GetString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var value) ? value.GetString() : null;
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
