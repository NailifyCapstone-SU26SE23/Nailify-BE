using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using static Nailify.Capstone.Infrastructure.Configuration.PayOS.PayOutDto;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class PayOSService : IPayOSPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IPayOSSettings _paymentSettings;
        private readonly IPaymentUrls _paymentUrls;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOSHelper _payOSHelper;
        private readonly ILogger<PayOSService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDistributedCache _cache;
        private const string PayOSBaseUrl = "https://api-merchant.payos.vn";

        public PayOSService(
            IHttpClientFactory httpClientFactory,
            IPayOSSettings paymentSettings,
            IPaymentUrls paymentUrls,
            IUnitOfWork unitOfWork,
            PayOSHelper payOSHelper,
            IServiceScopeFactory scopeFactory,
            IDistributedCache cache,
            ILogger<PayOSService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _paymentSettings = paymentSettings;
            _paymentUrls = paymentUrls;
            _unitOfWork = unitOfWork;
            _payOSHelper = payOSHelper;
            _scopeFactory = scopeFactory;
            _cache = cache;
            _logger = logger;
        }

        public class PendingBookingPaymentData
        {
            public Guid CustomerId { get; set; }
            public CreateBookingRequestDTO Request { get; set; } = null!;
        }

        public async Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreatePaymentLinkForBookingRequestAsync(Guid customerId, CreateBookingRequestDTO request)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var slotHoldService = scope.ServiceProvider.GetRequiredService<ISlotHoldService>();
                var bookingCreationService = scope.ServiceProvider.GetRequiredService<IBookingCreationService>();

                if (request.NailArtistId.HasValue && !string.IsNullOrEmpty(request.HoldToken))
                {
                    var isValidHold = await slotHoldService.ValidateHoldTokenAsync(request.HoldToken, customerId, request.NailArtistId.Value, request.BookingDate, request.StartTime);
                    if (!isValidHold)
                    {
                        return (false, "Mã giữ chỗ không hợp lệ hoặc đã hết hạn.", null);
                    }
                }

                var priceResult = await bookingCreationService.CalculateBookingPriceAsync(customerId, request.BookingItems, request.SelectedPromotionIds);
                if (!priceResult.IsSucceeded)
                {
                    return (false, priceResult.Message ?? "Lỗi tính giá.", null);
                }

                var salon = await _unitOfWork.SalonRepository.GetByIdAsync(request.SalonId);
                if (salon == null)
                {
                    return (false, "Khong tim thay salon.", null);
                }

                var depositRate = salon.DepositConfig;
                var amountDue = priceResult.Data?.TotalPrice ?? 0m;
                var finalAmountDue = amountDue * depositRate;
                if (finalAmountDue <= 0)
                {
                    // If the booking requires 0 payment (e.g. 100% discount or warranty), we shouldn't create a payment link.
                    // But for consistency with your flow, we handle it if needed. Let's assume there's an amount to pay.
                    if (amountDue > 0) finalAmountDue = amountDue; // Fallback if 20% calculation somehow goes wrong
                }
                decimal walletPaidAmount = 0m;
                WalletTransaction ? walletTx = null;
                if (request.UseWalletBalance && finalAmountDue > 0)
                {
                    var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdForUpdateAsync(customerId);
                    if(wallet != null)
                    {
                        var availableBalance = wallet.Balance - wallet.FrozenBalance;
                        if(availableBalance > 0)
                        {
                            walletPaidAmount = Math.Min(availableBalance, finalAmountDue);
                            var balanceBefore = wallet.Balance;
                            wallet.Balance -= walletPaidAmount;
                            wallet.UpdatedAt = DateTime.UtcNow;
                            _unitOfWork.CustomerWalletRepository.Update(wallet);
                            walletTx = new WalletTransaction
                            {
                                WalletId = wallet.WalletId,
                                Amount = -walletPaidAmount,
                                BalanceBefore = balanceBefore,
                                BalanceAfter = wallet.Balance,
                                Type = WalletTransactionType.BookingPayment,
                                Status = WalletTransactionStatus.Completed,
                                ReferenceId = null,
                                ReferenceType = WalletReferenceType.Booking,
                                Description = $"Thanh toán cọc đơn đặt lịch tại {salon.Name}",
                                CreatedAt = DateTime.UtcNow
                            };
                            await _unitOfWork.WalletTransactionRepository.CreateAsync(walletTx);
                            request.UseWalletBalance = false;
                        }
                    }
                }

                decimal remainingAmountToPayOnline = finalAmountDue - walletPaidAmount;

                if(remainingAmountToPayOnline <= 0)
                {
                    var createBookingResult = await bookingCreationService.CreateBookingAsync(customerId, request);
                    if (!createBookingResult.IsSucceeded || createBookingResult.Data == null)
                    {
                        return (false, createBookingResult.Message ?? "Lỗi tạo đơn đặt lịch.", null);
                    }

                    var createdBookingId = createBookingResult.Data.BookingId;
                    var booking = await _unitOfWork.BookingRepository.GetByIdAsync(createdBookingId);
                    if (booking != null)
                    {
                        booking.AmountPaid = walletPaidAmount;
                        booking.AmountDue = Math.Max(0m, amountDue - walletPaidAmount);
                        booking.Status = BookingStatus.Pending;
                        _unitOfWork.BookingRepository.Update(booking);
                        if (walletTx != null)
                        {
                            walletTx.ReferenceId = createdBookingId.ToString();
                            _unitOfWork.WalletTransactionRepository.Update(walletTx);
                        }
                    }
                    var code = await _payOSHelper.GenerateUniqueOrderCodeAsync();
                    var transactions = new Transaction
                    {
                        BookingId = createdBookingId,
                        OrderCode = code.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        Amount = finalAmountDue,
                        PaymentLinkId = "WALLET_PAYMENT",
                        CheckoutUrl = string.Empty,
                        QrCode = string.Empty,
                        Status = TransactionStatus.Paid,
                        Policy = FormatDepositPolicy(depositRate),
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow,
                        WebhookPayload = "Paid via Customer Wallet"
                    };
                    await _unitOfWork.TransactionRepository.CreateAsync(transactions);
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Thanh toán cọc bằng Ví thành công! Đơn đặt lịch đã được xác nhận.", new PaymentResponseDto
                    {
                        OrderCode = code,
                        Amount = (int)finalAmountDue,
                        QrCode = string.Empty,
                        Status = "PAID",
                        BookingId = createdBookingId
                    });
                }


                var orderCode = await _payOSHelper.GenerateUniqueOrderCodeAsync();
                var amount = (int)Math.Round(remainingAmountToPayOnline, MidpointRounding.AwayFromZero);
                if (amount <= 0)
                {
                     return (false, "Số tiền thanh toán không hợp lệ.", null);
                }

                var description = $"Cọc đơn {orderCode}";
                var itemName = $"Cọc đơn {orderCode}";
                var signature = CreatePaymentRequestSignature(amount, description, orderCode);
                
                var paymentRequest = new
                {
                    orderCode,
                    amount,
                    description,
                    items = new[]
                    {
                        new { name = itemName, quantity = 1, price = amount }
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
                if (paymentResult.TryGetProperty("code", out var codeElement) && codeElement.GetString() != "00")
                {
                    var desc = paymentResult.TryGetProperty("desc", out var descElement) ? descElement.GetString() : responseContent;
                    return (false, $"Lỗi từ PayOS - Code: {codeElement.GetString()}, Message: {desc}", null);
                }

                if (!paymentResult.TryGetProperty("data", out var data))
                {
                    return (false, $"PayOS response không có data: {responseContent}", null);
                }

                var pendingData = new PendingBookingPaymentData
                {
                    CustomerId = customerId,
                    Request = request
                };
                
                var cacheKey = $"payos:booking_req:{orderCode}";
                var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20) };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(pendingData), cacheOptions);

                var transaction = new Transaction
                {
                    BookingId = null,
                    OrderCode = orderCode.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Amount = finalAmountDue,
                    PaymentLinkId = GetString(data, "paymentLinkId"),
                    CheckoutUrl = GetString(data, "checkoutUrl") ?? string.Empty,
                    QrCode = GetString(data, "qrCode") ?? string.Empty,
                    Status = TransactionStatus.Pending,
                    Policy = FormatDepositPolicy(depositRate),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    WebhookPayload = string.Empty
                };

                await _unitOfWork.TransactionRepository.CreateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                StartStatusPolling(orderCode, transaction.ExpiresAt);

                return (true, "Tạo link thanh toán thành công!", ToResponse(transaction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo link thanh toán cho request.");
                return (false, $"Lỗi khi tạo link thanh toán: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreateWalletDepositPaymentLinkAsync(Guid walletId, decimal amount)
        {
            try
            {
                var wallet = await _unitOfWork.CustomerWalletRepository.GetByIdAsync(walletId);
                if (wallet == null)
                {
                    return (false, "Không tìm thấy ví của khách hàng.", null);
                }

                var orderCode = await _payOSHelper.GenerateUniqueOrderCodeAsync();
                var amountInt = (int)Math.Round(amount, MidpointRounding.AwayFromZero);
                if (amountInt <= 0)
                {
                    return (false, "Số tiền nạp không hợp lệ.", null);
                }

                var description = $"Nap vi {orderCode}";
                var itemName = $"Nap tien vi {orderCode}";
                var signature = CreatePaymentRequestSignature(amountInt, description, orderCode);

                var paymentRequest = new
                {
                    orderCode,
                    amount = amountInt,
                    description,
                    items = new[]
                    {
                        new { name = itemName, quantity = 1, price = amountInt }
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
                    return (false, $"Lỗi từ PayOS: {responseContent}", null);
                }

                var paymentResult = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (paymentResult.TryGetProperty("code", out var codeElement) && codeElement.GetString() != "00")
                {
                    var desc = paymentResult.TryGetProperty("desc", out var descElement) ? descElement.GetString() : responseContent;
                    return (false, $"Lỗi từ PayOS - Code: {codeElement.GetString()}, Message: {desc}", null);
                }

                if (!paymentResult.TryGetProperty("data", out var data))
                {
                    return (false, $"PayOS response không có data: {responseContent}", null);
                }

                var transaction = new Transaction
                {
                    BookingId = null,
                    WalletId = walletId,
                    PaymentType = PaymentType.WalletDeposit,
                    OrderCode = orderCode.ToString(CultureInfo.InvariantCulture),
                    Amount = amount,
                    PaymentLinkId = GetString(data, "paymentLinkId"),
                    CheckoutUrl = GetString(data, "checkoutUrl") ?? string.Empty,
                    QrCode = GetString(data, "qrCode") ?? string.Empty,
                    Status = TransactionStatus.Pending,
                    Policy = "Nạp tiền vào ví cá nhân",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    WebhookPayload = string.Empty
                };

                await _unitOfWork.TransactionRepository.CreateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                StartStatusPolling(orderCode, transaction.ExpiresAt);

                return (true, "Tạo link nạp tiền ví thành công!", ToResponse(transaction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo link nạp tiền ví cho wallet {WalletId}.", walletId);
                return (false, $"Lỗi khi tạo link nạp tiền: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, PayoutResponseDto? Payout)> CreateWalletWithdrawalPayoutAsync(
            Guid withdrawalRequestId,
            string bankCode,
            string accountNumber,
            string accountHolderName,
            decimal amount)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_paymentSettings.PayoutClientId) ||
                    string.IsNullOrWhiteSpace(_paymentSettings.PayoutApiKey))
                {
                    return (false, "PayOS payout credentials are not configured.", null);
                }

                var amountInt = (int)Math.Round(amount, MidpointRounding.AwayFromZero);
                if (amountInt <= 0)
                {
                    return (false, "Số tiền rút không hợp lệ.", null);
                }

                var referenceId = $"withdrawal_{withdrawalRequestId}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                var payoutRequest = new
                {
                    referenceId,
                    amount = amountInt,
                    description = "Wallet withdrawal",
                    toBin = GetBankBin(bankCode),
                    toAccountNumber = accountNumber,
                    category = new[] { "withdrawal" }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{PayOSBaseUrl}/v1/payouts");
                request.Headers.Add("x-client-id", _paymentSettings.PayoutClientId);
                request.Headers.Add("x-api-key", _paymentSettings.PayoutApiKey);
                request.Headers.Add("x-idempotency-key", referenceId);
                request.Headers.Add("x-signature", GeneratePayoutSignature(payoutRequest));
                request.Content = new StringContent(JsonSerializer.Serialize(payoutRequest, JsonOptions), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return (false, $"Lỗi từ PayOS payout: {responseContent}", null);
                }

                var payoutResponse = JsonSerializer.Deserialize<PayOSPayoutResponse>(responseContent, JsonOptions);
                if (payoutResponse?.Code == "00" && payoutResponse.Data != null)
                {
                    return (true, payoutResponse.Desc ?? "Tạo payout rút tiền thành công.", new PayoutResponseDto
                    {
                        PayoutId = payoutResponse.Data.Id,
                        ReferenceId = payoutResponse.Data.ReferenceId,
                        ApprovalState = payoutResponse.Data.ApprovalState
                    });
                }

                return (false, payoutResponse?.Desc ?? "PayOS payout failed.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo PayOS payout cho withdrawal {WithdrawalRequestId}.", withdrawalRequestId);
                return (false, $"Lỗi khi tạo payout rút tiền: {ex.Message}", null);
            }
        }

        public Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreateBookingPaymentLinkAsync(Guid bookingId)
            => CreatePaymentLinkAsync(bookingId);

        public Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreateBookingRequestPaymentLinkAsync(Guid customerId, CreateBookingRequestDTO request)
            => CreatePaymentLinkForBookingRequestAsync(customerId, request);

        public Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreateDynamicPaymentLinkAsync(PayOSPaymentContextRequest request)
        {
            throw new NotImplementedException("Dynamic payment context link is not enabled.");
        }

        public async Task<(bool Success, string Message, PaymentResponseDto? Payment)> CreatePaymentLinkAsync(Guid bookingId)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    return (false, "Không tìm thấy lịch hẹn.", null);
                }

                var depositRate = 1m;
                if (booking.Status != BookingStatus.ServiceCompleted)
                {
                    var salon = await _unitOfWork.SalonRepository.GetByIdAsync(booking.SalonId);
                    if (salon == null)
                    {
                        return (false, "Không tìm thấy salon.", null);
                    }

                    depositRate = salon.DepositConfig;
                }

                var amountDue = booking.Status == BookingStatus.ServiceCompleted
                    ? booking.AmountDue ?? booking.TotalPrice ?? 0m
                    : booking.TotalPrice * depositRate ?? 0m;
                if (amountDue <= 0)
                {
                    return (false, $"Số tiền không hợp lệ: {amountDue}.", null);
                }

                var existing = await _unitOfWork.TransactionRepository
                    .FindByCondition(t => t.BookingId == bookingId && t.Status == TransactionStatus.Pending)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();
                if (existing != null && existing.ExpiresAt > DateTime.UtcNow)
                {
                    return (true, "Link thanh toán đã tồn tại.", ToResponse(existing));
                }

                var orderCode = await _payOSHelper.GenerateUniqueOrderCodeAsync();
                var amount = (int)Math.Round(amountDue, MidpointRounding.AwayFromZero);
                var description = $"Thanh toán đơn {orderCode}";
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
                    Policy = booking.Status == BookingStatus.ServiceCompleted ? string.Empty : FormatDepositPolicy(depositRate),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    WebhookPayload = string.Empty
                };

                await _unitOfWork.TransactionRepository.CreateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                StartStatusPolling(orderCode, transaction.ExpiresAt);

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
                    _logger.LogWarning(
                        "PayOS webhook rejected because signature is invalid. OrderCode: {OrderCode}, Payload: {@Payload}",
                        ResolveOrderCode(webhookDto),
                        webhookDto);
                    return (false, "Webhook signature khong hop le.");
                }

                var transaction = await _unitOfWork.TransactionRepository.GetByOrderCodeAsync(
                    ResolveOrderCode(webhookDto).ToString(CultureInfo.InvariantCulture),
                    trackChanges: true);
                if (transaction == null)
                {
                    _logger.LogWarning(
                        "PayOS webhook rejected because no local transaction was found. OrderCode: {OrderCode}, Payload: {@Payload}",
                        ResolveOrderCode(webhookDto),
                        webhookDto);
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
                    if (transaction.PaymentType == PaymentType.WalletDeposit || transaction.WalletId.HasValue)
                    {
                        await CreditWalletForPaidTransactionAsync(transaction);
                    }
                    else
                    {
                        await EnsureBookingForPaidTransactionAsync(transaction);
                        await ApplyPaidAmountToBookingAsync(transaction);
                    }
                }

                _unitOfWork.TransactionRepository.Update(transaction);
                await _unitOfWork.SaveChangesAsync();
                return (true, "Xu ly webhook thanh cong.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing PayOS webhook. Payload: {@Payload}", webhookDto);
                return (false, $"Loi khi xu ly webhook: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, string? Status)> GetPaymentStatusAsync(long orderCode)
        {
            try
            {
                var transaction = await _unitOfWork.TransactionRepository.GetByOrderCodeAsync(orderCode.ToString(CultureInfo.InvariantCulture), trackChanges: false);
                if (transaction != null && (transaction.PaymentLinkId == "WALLET_PAYMENT" || transaction.Status == TransactionStatus.Paid))
                {
                    return (true, "Lấy trạng thái thanh toán thành công!", transaction.Status.ToString().ToUpper());
                }

                ApplyAuthenticationHeaders();
                var response = await _httpClient.GetAsync($"{PayOSBaseUrl}/v2/payment-requests/{orderCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    if (transaction != null)
                    {
                        return (true, "Lấy trạng thái từ hệ thống thành công!", transaction.Status.ToString().ToUpper());
                    }
                    return (false, $"Lỗi từ PayOS: {responseContent}", null);
                }

                var paymentResult = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var data = paymentResult.GetProperty("data");
                var status = data.GetProperty("status").GetString();

                await SyncLocalTransactionStatusAsync(orderCode, status);

                return (true, "Lấy trạng thái thanh toán thành công!", status);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi lấy trạng thái thanh toán: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> CancelPaymentLinkAsync(long orderCode)
        {
            try
            {
                var transaction = await _unitOfWork.TransactionRepository.GetByOrderCodeAsync(orderCode.ToString(CultureInfo.InvariantCulture), trackChanges: true);
                if (transaction == null)
                {
                    return (false, "Không tìm thấy giao dịch tương ứng.");
                }

                if (transaction.Status == TransactionStatus.Paid)
                {
                    return (false, "Không thể hủy giao dịch đã thanh toán.");
                }

                ApplyAuthenticationHeaders();
                var cancelRequest = new { cancellationReason = "Hủy bởi người dùng" };
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

                return (true, "Hủy link thanh toán thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi hủy link thanh toán: {ex.Message}");
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

            var dataStr = CreateSignatureDataString(data);
            var calculatedSignature = CreateHmacSha256(dataStr);
            return string.Equals(calculatedSignature, webhookDto.Signature, StringComparison.OrdinalIgnoreCase);
        }

        private static string CreateSignatureDataString(PaymentWebhookData data)
        {
            var values = data.GetType()
                .GetProperties()
                .OrderBy(property => GetJsonPropertyName(property.Name), StringComparer.Ordinal)
                .Select(property =>
                {
                    var key = GetJsonPropertyName(property.Name);
                    var value = property.GetValue(data);
                    return $"{key}={FormatSignatureValue(value)}";
                });

            return string.Join("&", values);
        }

        private static string GetJsonPropertyName(string propertyName)
        {
            return char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
        }

        private static string FormatSignatureValue(object? value)
        {
            return value switch
            {
                null => string.Empty,
                decimal decimalValue => decimalValue.ToString(CultureInfo.InvariantCulture),
                long longValue => longValue.ToString(CultureInfo.InvariantCulture),
                int intValue => intValue.ToString(CultureInfo.InvariantCulture),
                bool boolValue => boolValue.ToString().ToLowerInvariant(),
                _ => value.ToString() ?? string.Empty
            };
        }

        private string CreateHmacSha256(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_paymentSettings.ChecksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private string GeneratePayoutSignature(object? data)
        {
            if (data == null)
            {
                return string.Empty;
            }

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

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_paymentSettings.PayoutChecksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static string GetBankBin(string bankCode)
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

        private static string FormatDepositPolicy(decimal depositRate)
        {
            var percent = depositRate * 100m;
            var formattedPercent = decimal.Truncate(percent) == percent
                ? percent.ToString("0", CultureInfo.InvariantCulture)
                : percent.ToString("0.##", CultureInfo.InvariantCulture);

            return $"Cọc {formattedPercent}%";
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
            if (transaction.Status == newStatus && newStatus != TransactionStatus.Paid)
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
                if (transaction.PaymentType == PaymentType.WalletDeposit || transaction.WalletId.HasValue)
                {
                    await CreditWalletForPaidTransactionAsync(transaction);
                }
                else
                {
                    await EnsureBookingForPaidTransactionAsync(transaction);
                    await ApplyPaidAmountToBookingAsync(transaction);
                }
            }

            _unitOfWork.TransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task CreditWalletForPaidTransactionAsync(Transaction transaction)
        {
            if (!transaction.WalletId.HasValue)
            {
                _logger.LogError("WalletDeposit transaction {OrderCode} missing WalletId.", transaction.OrderCode);
                return;
            }

            var existingTx = await _unitOfWork.WalletTransactionRepository
                .FindByCondition(t => t.WalletId == transaction.WalletId.Value && t.ReferenceId == transaction.OrderCode)
                .FirstOrDefaultAsync();

            if (existingTx != null)
            {
                return;
            }

            var wallet = await _unitOfWork.CustomerWalletRepository.GetByWalletIdForUpdateAsync(transaction.WalletId.Value);
            if (wallet == null)
            {
                _logger.LogError("Wallet {WalletId} not found for deposit order {OrderCode}.", transaction.WalletId, transaction.OrderCode);
                return;
            }

            var balanceBefore = wallet.Balance;
            wallet.Balance += transaction.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            var walletTx = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                Amount = transaction.Amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = wallet.Balance,
                Type = WalletTransactionType.Deposit,
                Status = WalletTransactionStatus.Completed,
                ReferenceId = transaction.OrderCode,
                ReferenceType = WalletReferenceType.PayOs,
                Description = $"Nạp tiền vào ví qua PayOS (Mã GD: {transaction.OrderCode})",
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.CustomerWalletRepository.Update(wallet);
            await _unitOfWork.WalletTransactionRepository.CreateAsync(walletTx);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully credited {Amount} VND to Wallet {WalletId}. New Balance: {Balance}",
                transaction.Amount, wallet.WalletId, wallet.Balance);
        }

        private async Task EnsureBookingForPaidTransactionAsync(Transaction transaction)
        {
            if (transaction.BookingId.HasValue)
            {
                transaction.Booking ??= await _unitOfWork.BookingRepository.GetByIdAsync(transaction.BookingId.Value);
                return;
            }

            var cacheKey = $"payos:booking_req:{transaction.OrderCode}";
            var cachedJson = await _cache.GetStringAsync(cacheKey);
            if (string.IsNullOrWhiteSpace(cachedJson))
            {
                return;
            }

            var pendingData = JsonSerializer.Deserialize<PendingBookingPaymentData>(cachedJson);
            if (pendingData == null)
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

            var createResult = await bookingService.CreateBookingAsync(pendingData.CustomerId, pendingData.Request);
            if (!createResult.IsSucceeded || createResult.Data == null)
            {
                _logger.LogError(
                    "Failed to create booking after successful payment. OrderCode: {OrderCode}, Error: {Error}",
                    transaction.OrderCode,
                    createResult.Message);
                return;
            }

            transaction.BookingId = createResult.Data.BookingId;
            transaction.Booking = await _unitOfWork.BookingRepository.GetByIdAsync(createResult.Data.BookingId);
            await _cache.RemoveAsync(cacheKey);
        }

        private async Task ApplyPaidAmountToBookingAsync(Transaction transaction)
        {
            if (transaction.Booking == null || !transaction.BookingId.HasValue)
            {
                return;
            }

            var paidAmountBeforeCurrentTransaction = await _unitOfWork.TransactionRepository
                .FindByCondition(t =>
                    t.BookingId == transaction.BookingId &&
                    t.TransactionId != transaction.TransactionId &&
                    t.Status == TransactionStatus.Paid)
                .SumAsync(t => t.Amount);

            var amountPaid = paidAmountBeforeCurrentTransaction + transaction.Amount;
            var totalPrice = transaction.Booking.TotalPrice ?? 0m;

            transaction.Booking.AmountPaid = amountPaid;
            transaction.Booking.AmountDue = Math.Max(0m, totalPrice - amountPaid);
            if (transaction.Booking.Status == BookingStatus.ServiceCompleted)
            {
                transaction.Booking.CheckOut(Guid.Empty);
            }
        }

        private void StartStatusPolling(long orderCode, DateTime expiresAt)
        {
            _ = Task.Run(async () =>
            {
                var maxDuration = expiresAt - DateTime.UtcNow;
                if (maxDuration < TimeSpan.FromMinutes(1))
                {
                    maxDuration = TimeSpan.FromMinutes(1);
                }

                var deadline = DateTime.UtcNow.Add(maxDuration);
                var delay = TimeSpan.FromSeconds(10);

                var attempt = 0;
                while (DateTime.UtcNow < deadline)
                {
                    attempt++;
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var scopedPaymentService = scope.ServiceProvider.GetRequiredService<PayOSService>();
                        var (_, _, status) = await scopedPaymentService.GetPaymentStatusAsync(orderCode);
                        if (IsTerminalPayOSStatus(status))
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Auto status poll failed for order code {OrderCode} on attempt {Attempt}.",
                            orderCode,
                            attempt);
                    }

                    if (DateTime.UtcNow < deadline)
                    {
                        await Task.Delay(delay);
                    }
                }

                using var overdueScope = _scopeFactory.CreateScope();
                var overduePaymentService = overdueScope.ServiceProvider.GetRequiredService<PayOSService>();
                await overduePaymentService.MarkTransactionOverdueAsync(orderCode);
            });
        }

        private async Task MarkTransactionOverdueAsync(long orderCode)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetByOrderCodeAsync(
                orderCode.ToString(CultureInfo.InvariantCulture),
                trackChanges: true);

            if (transaction == null || transaction.Status == TransactionStatus.Paid || transaction.Status == TransactionStatus.Cancelled || transaction.Status == TransactionStatus.Overdue)
            {
                return;
            }

            transaction.Status = TransactionStatus.Overdue;
            _unitOfWork.TransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        private static bool IsTerminalPayOSStatus(string? status)
        {
            return status?.ToUpperInvariant() is "PAID" or "CANCELLED" or "CANCELED" or "EXPIRED";
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
