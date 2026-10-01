using AutoMapper;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Helpers;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class BookingLifecycleService : IBookingLifecycleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWalkInQueueService _queueService;
        private readonly IBookingSchedulingService _bookingSchedulingService;
        private readonly ILoyaltyTierService _loyaltyTierService;
        private readonly ILogger<BookingService> _logger;
        private readonly IBookingProcedureService _bookingProcedureService;
        private readonly INotificationService _notificationService;
        private readonly IPromotionService _promotionService;
        private readonly IOrderCodeGenerator _orderCodeGenerator;
        private readonly IRefundService _refundService;
        public BookingLifecycleService(
                                         IUnitOfWork unitOfWork,
                                         IMapper mapper,
                                         IWalkInQueueService queueService,
                                         IBookingSchedulingService bookingSchedulingService,
                                         ILoyaltyTierService loyaltyTierService,
                                         ILogger<BookingService> logger,
                                         IBookingProcedureService bookingProcedureService,
                                         INotificationService notificationService,
                                         IPromotionService promotionService,
                                         IOrderCodeGenerator orderCodeGenerator,
                                         IRefundService refundService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _queueService = queueService;
            _bookingSchedulingService = bookingSchedulingService;
            _loyaltyTierService = loyaltyTierService;
            _logger = logger;
            _bookingProcedureService = bookingProcedureService;
            _notificationService = notificationService;
            _promotionService = promotionService;
            _orderCodeGenerator = orderCodeGenerator;
            _refundService = refundService;
        }
        private static bool IsItemFromOldBooking(BookingItemRequestDTO item, IEnumerable<BookingItem> oldBookingItems)
        {
            return oldBookingItems.Any(oldItem =>
                (item.NailVariantId.HasValue && oldItem.NailVariantId == item.NailVariantId) ||
                (item.CustomerNailId.HasValue && oldItem.CustomerNailRequest != null && oldItem.CustomerNailRequest.CustomerNailId == item.CustomerNailId) ||
                (item.CustomerNailRequestId.HasValue && oldItem.CustomerNailRequestId == item.CustomerNailRequestId) ||
                (item.CustomerNailId.HasValue && oldItem.CustomerNailRequestId.HasValue && oldItem.CustomerNailRequest != null && oldItem.CustomerNailRequest.CustomerNailId == item.CustomerNailId)
            );
        }
        public async Task<ApiResult<BookingResponseDTO>> VerifyQrCodeAsync(string qrToken, Guid actorId)
        {
            if (string.IsNullOrEmpty(qrToken))
            {
                return new ApiErrorResult<BookingResponseDTO>("Mã QR không hợp lệ.");
            }

            var parts = qrToken.Split('|');
            if (parts.Length != 3 || parts[0] != "NAILIFY")
            {
                return new ApiErrorResult<BookingResponseDTO>("Định dạng mã QR không đúng.");
            }

            if (!Guid.TryParse(parts[1], out Guid bookingId))
            {
                return new ApiErrorResult<BookingResponseDTO>("Mã đặt lịch trong QR không hợp lệ.");
            }

            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != BookingStatus.Approved && booking.Status != BookingStatus.CheckedIn)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Đơn đặt lịch không ở trạng thái sẵn sàng để check-in. Trạng thái hiện tại: '{booking.Status}'.");
            }

            var tokenDateStr = parts[2];
            var localBookingDate = (booking.BookingDate.Kind == DateTimeKind.Utc ? booking.BookingDate.AddHours(7) : booking.BookingDate).Date;
            if (localBookingDate.ToString("yyyyMMdd") != tokenDateStr)
            {
                return new ApiErrorResult<BookingResponseDTO>("Ngày đặt lịch không khớp với thông tin trên mã QR.");
            }

            if (booking.Status == BookingStatus.Approved)
            {
                /*
                  booking.Status = BookingStatus.CheckedIn;
                  booking.UpdatedAt = DateTime.UtcNow;

                  var history = new BookingHistory
                  {
                      BookingHistoryId = Guid.NewGuid(),
                      BookingId = booking.BookingId,
                      EventType = "CheckedIn",
                      Payload = "Xác thực mã QR thành công. Trạng thái đơn hàng chuyển sang CheckedIn.",
                      CreatedAt = DateTime.UtcNow
                  };
                */

                booking.CheckInFromQr(actorId);
                var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId, trackChanges: true);
                if (procedures.Any())
                {
                    var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);
                    foreach(var segment in timeline)
                    {
                        var procedure = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                        procedure.EstimatedStartTime = segment.StartTime;
                        procedure.EstimatedEndTime = segment.EndTime;
                        _unitOfWork.BookingProcedureRepository.Update(procedure);
                    }
                }

                _unitOfWork.BookingRepository.Update(booking);
                //await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
                await _unitOfWork.SaveChangesAsync();
                
                if (!booking.IsLateArrival)
                {
                    await _bookingSchedulingService.HandleOverlappingOnCheckInAsync(booking);
                }
            }

            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Xác thực mã QR thành công. Trạng thái đơn chuyển sang CheckedIn.");
        }
        public async Task<ApiResult<BookingResponseDTO>> CheckInBookingAsync(CheckInRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            if (booking.Status != BookingStatus.CheckedIn)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể cập nhật ảnh tình trạng tay khi đơn đã ở trạng thái Checked-in. Trạng thái hiện tại: '{booking.Status}'.");
            }

            booking.CheckInImageUrl = request.CheckInImageUrl;
            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Cập nhật ảnh tình trạng bàn tay thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> ManualCheckInBookingAsync(Guid bookingId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            /*
            booking.Status = BookingStatus.CheckedIn;
            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.BookingRepository.Update(booking);
            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "BookingCheckedIn",
                Payload = "Tiếp tân checkin lịch hẹn bằng tay.",
                ActorId = actorId == Guid.Empty ? null : actorId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Checkin lịch hẹn thành công.");
            */
            booking.CheckInWithoutImage(actorId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            
            if (!booking.IsLateArrival)
            {
                await _bookingSchedulingService.HandleOverlappingOnCheckInAsync(booking);
            }
            
            if (booking.IsLateArrival)
            {
                var originalArtistId = booking.NailArtistId;
                booking.NailArtistId = null;

                var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId, trackChanges: true);
                foreach (var proc in procedures)
                {
                    proc.AssignedArtistId = null;
                    _unitOfWork.BookingProcedureRepository.Update(proc);
                }

                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var addToQueueRequest = new AddToQueueRequestDTO
                {
                    SalonId = booking.SalonId,
                    CustomerId = booking.CustomerId,
                    OriginalBookingId = booking.BookingId,
                    GuestName = $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}",
                    GuestPhone = booking.Customer.User.Phone,
                    RequestNote = "Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.",
                    AssignedNailArtistId = originalArtistId
                };

                await _queueService.AddToQueueAsync(actorId, addToQueueRequest);
            }
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, booking.IsLateArrival
                ? "Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ."
                : "Checkin lịch hẹn thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> StartServiceAsync(Guid bookingId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.CheckedIn)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể bắt đầu làm khi khách đã 'CheckedIn'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            booking.StartService(actorId);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Bắt đầu làm móng thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> CompleteServiceAsync(CompleteServiceRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch");
            }
            if (booking.Status != BookingStatus.InProgress)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể hoàn thành dịch vụ khi đơn đang ở trạng thái 'InProgress'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(request.BookingId);

            if (procedures.Any())
            {
                var incompleteRequiredProcedures = procedures.Where(x =>
                    x.IsRequired &&
                    x.Status != BookingProcedureStatus.Completed &&
                    x.Status != BookingProcedureStatus.Skipped)
                    .ToList();
                if (incompleteRequiredProcedures.Any())
                {
                    var names = string.Join(", ", incompleteRequiredProcedures.Select(p => p.ProcedureName));
                    return new ApiErrorResult<BookingResponseDTO>(
                                    $"Không thể hoàn thành dịch vụ. Các bước bắt buộc sau chưa hoàn thành: {names}.");
                }
            }
            string finalUrls = string.Join(",", request.CompleteImagesUrl);
            booking.CompleteService(finalUrls, actorId);

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Hoàn thành dịch vụ làm móng thành công");
        }
        public async Task<ApiResult<BookingResponseDTO>> CheckOutBookingAsync(CheckOutRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            if (booking.Status != BookingStatus.ServiceCompleted)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể check-out thanh toán khi dịch vụ đã làm xong ('ServiceCompleted'). Trạng thái hiện tại; '{booking.Status}'.");
            }
            if (booking.WarrantyForBookingId.HasValue)
            {
                booking.CheckOutWarranty(actorId);
            }
            else
            {
                booking.CheckOut(actorId);
            }
            if (booking.AmountDue.HasValue && booking.AmountDue.Value > 0)
            {
                var amountDue = booking.AmountDue.Value;
                var now = DateTime.UtcNow;
                var transaction = new Transaction
                {
                    BookingId = booking.BookingId,
                    OrderCode = (await _orderCodeGenerator.GenerateUniqueOrderCodeAsync()).ToString(),
                    Amount = amountDue,
                    Reference = null,
                    PaymentLinkId = null,
                    CheckoutUrl = string.Empty,
                    QrCode = string.Empty,
                    Status = TransactionStatus.Paid,
                    CreatedAt = now,
                    PaidAt = now,
                    ExpiresAt = now,
                    WebhookPayload = string.Empty
                };

                await _unitOfWork.TransactionRepository.CreateAsync(transaction);

                var amountPaid = (booking.AmountPaid ?? 0m) + amountDue;
                booking.AmountPaid = amountPaid;
                booking.AmountDue = Math.Max(0m, (booking.TotalPrice ?? amountPaid) - amountPaid);
            }
            _unitOfWork.BookingRepository.Update(booking);
            if (booking.AmountPaid.HasValue && booking.TotalPrice.HasValue && booking.AmountPaid.Value > booking.TotalPrice.Value)
            {
                decimal overpaidAmount = booking.AmountPaid.Value - booking.TotalPrice.Value;
                var wallet = await _unitOfWork.CustomerWalletRepository.GetByCustomerIdForUpdateAsync(booking.CustomerId);
                if (wallet != null)
                {
                    var balanceBefore = wallet.Balance;
                    wallet.Balance += overpaidAmount;
                    wallet.UpdatedAt = DateTime.UtcNow;
                    var walletTx = new WalletTransaction
                    {
                        WalletId = wallet.WalletId,
                        Amount = overpaidAmount,
                        BalanceBefore = balanceBefore,
                        BalanceAfter = wallet.Balance,
                        Type = WalletTransactionType.BookingRefund,
                        Status = WalletTransactionStatus.Completed,
                        ReferenceId = booking.BookingId.ToString(),
                        ReferenceType = WalletReferenceType.Booking,
                        Description = $"Hoàn lại tiền cọc dư ({overpaidAmount:N0}đ) do giảm dịch vụ đơn hàng {booking.BookingId}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.WalletTransactionRepository.CreateAsync(walletTx);
                    _unitOfWork.CustomerWalletRepository.Update(wallet);

                    booking.AmountPaid = booking.TotalPrice.Value;
                }
            }
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Khách hàng Check-out thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> ConfirmBookingAsync(Guid bookingId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.Pending)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể xác nhận đơn ở trạng thái 'Pending'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            var activeChairs = await _unitOfWork.ChairRepository.GetActiveChairsBySalonAsync(booking.SalonId);
            var activeChairCount = activeChairs.Count();
            if(activeChairCount > 0)
            {
                var approvedOverlapCount = await _unitOfWork.BookingRepository.CountApprovedOverlappingAsync(
                    booking.SalonId,
                    booking.BookingDate,
                    booking.StartTime,
                    booking.TotalDuration,
                    excludeBookingId: bookingId);
                if(approvedOverlapCount >= activeChairCount)
                {
                    return new ApiErrorResult<BookingResponseDTO>(
                         $"Không thể duyệt: Salon đã có {approvedOverlapCount}/{activeChairCount} " +
                         $"ghế được đặt trong khung giờ này.");
                }
            }
            booking.Confirm(actorId);
            _unitOfWork.BookingRepository.Update(booking);
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(bookingId, trackChanges: true);
            if (procedures.Any() && booking.NailArtistId.HasValue)
            {
                // 1. Tính toán Timelxine thực tế bắt đầu từ StartTime
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);
                // 2. Cập nhật Estimated time và gán AssignedArtist cho các công đoạn có ActiveDuration > 0
                foreach (var segment in timeline)
                {
                    var procedure = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);

                    procedure.EstimatedStartTime = segment.StartTime;
                    procedure.EstimatedEndTime = segment.EndTime;
                    // Nếu công đoạn này thợ cần thao tác (ActiveDuration > 0), gán AssignedArtistId
                    if (procedure.ActiveDuration > 0 && procedure.IsMainStep)
                    {
                        procedure.AssignedArtistId = booking.NailArtistId.Value;
                    }

                    _unitOfWork.BookingProcedureRepository.Update(procedure);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            try
            {
                string customerName = booking.Customer?.User != null ? $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}".Trim() : "Khách hàng";
                string salonName = booking.Salon?.Name ?? "Salon";
                if (salonName == "Salon" && _unitOfWork.SalonRepository != null)
                {
                    var salonObj = await _unitOfWork.SalonRepository.GetByIdAsync(booking.SalonId);
                    if (salonObj != null) salonName = salonObj.Name;
                }
                string artistName = booking.NailArtist?.Account != null ? $"{booking.NailArtist.Account.FirstName} {booking.NailArtist.Account.LastName}".Trim() : "Thợ nail";
                string bookingCode = bookingId.ToString().Substring(0, 8).ToUpper();

                await _notificationService.SendNotificationToUserAsync(
                    booking.CustomerId.ToString(),
                    "BookingConfirmed",
                    new
                    {
                        BookingId = bookingId,
                        BookingCode = bookingCode,
                        SalonName = salonName,
                        ArtistName = artistName,
                        CustomerName = customerName,
                        Message = $"Đơn đặt lịch #{bookingCode} của bạn đã được Salon xác nhận."
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NotificationError] Failed to send booking confirmed notification: {ex.Message}");
            }
            /*
            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            */
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Duyệt đơn đặt lịch thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> RejectBookingAsync(Guid bookingId, Guid actorId, RejectRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.Pending)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể từ chối đơn ở trạng thái 'Pending'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            var refundResult = await _refundService.RefundToWalletByBookingAsync(
                bookingId,
                "Hoàn toàn bộ tiền cọc do lịch hẹn bị từ chối.",
                forceFullRefund: true);
            if (!refundResult.Success && refundResult.Message != "Paid transaction not found for this booking")
            {
                return new ApiErrorResult<BookingResponseDTO>(refundResult.Message);
            }
            if (booking.BookingDiscounts != null && booking.BookingDiscounts.Any())
            {
                await _promotionService.RollbackUsageAsync(booking.CustomerId, booking.BookingDiscounts);
            }

            booking.Reject(actorId, request.Reason);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            try
            {
                string customerName = booking.Customer?.User != null ? $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}".Trim() : "Khách hàng";
                string salonName = booking.Salon?.Name ?? "Salon";
                if (salonName == "Salon" && _unitOfWork.SalonRepository != null)
                {
                    var salonObj = await _unitOfWork.SalonRepository.GetByIdAsync(booking.SalonId);
                    if (salonObj != null) salonName = salonObj.Name;
                }
                string bookingCode = booking.BookingId.ToString().Substring(0, 8).ToUpper();

                await _notificationService.SendNotificationToUserAsync(
                    booking.CustomerId.ToString(),
                    "BookingRejected",
                    new
                    {
                        BookingId = booking.BookingId,
                        BookingCode = bookingCode,
                        SalonName = salonName,
                        CustomerName = customerName,
                        Reason = request.Reason,
                        Message = $"Đơn đặt lịch #{bookingCode} của bạn tại Salon {salonName} đã bị từ chối. Lý do: {request.Reason}"
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NotificationError] Failed to send booking rejected notification: {ex.Message}");
            }

            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Từ chối đơn đặt lịch thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> CancelBookingAsync(Guid bookingId, Guid actorId, CancelBookingRequestDTO request, bool isCustomerActor = false)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }

            if (isCustomerActor && booking.CustomerId != actorId)
            {
                return new ApiErrorResult<BookingResponseDTO>("Bạn không có quyền hủy lịch hẹn của người khác.");
            }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Approved)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ được hủy đơn ở trạng thái 'Pending' hoặc 'Approved'. Trạng thái hiện tại: '{booking.Status}'.");
            }

            var policyRefund = isCustomerActor || request.CustomerRequest == true;
            var refundResult = await _refundService.RefundToWalletByBookingAsync(
                bookingId,
                policyRefund ? request.Reason : $"Hoàn toàn bộ tiền cọc do Salon hủy lịch. Lý do: {request.Reason}",
                forceFullRefund: !policyRefund);
            if (!refundResult.Success && refundResult.Message != "Paid transaction not found for this booking")
            {
                return new ApiErrorResult<BookingResponseDTO>(refundResult.Message);
            }
            if (booking.BookingDiscounts != null && booking.BookingDiscounts.Any())
            {
                await _promotionService.RollbackUsageAsync(booking.CustomerId, booking.BookingDiscounts);
            }

            booking.Cancel(actorId, request.Reason);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            try
            {
                string customerName = booking.Customer?.User != null ? $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}".Trim() : "Khách hàng";
                string salonName = booking.Salon?.Name ?? "Salon";
                if (salonName == "Salon" && _unitOfWork.SalonRepository != null)
                {
                    var salonObj = await _unitOfWork.SalonRepository.GetByIdAsync(booking.SalonId);
                    if (salonObj != null) salonName = salonObj.Name;
                }
                string bookingCode = booking.BookingId.ToString().Substring(0, 8).ToUpper();

                if (policyRefund)
                {
                    // Khách hàng hủy -> Gửi thông báo SignalR tới Lễ tân / Staff của Salon
                    await _notificationService.SendNotificationToSalonStaffAsync(
                        booking.SalonId.ToString(),
                        "BookingCancelled",
                        new
                        {
                            BookingId = booking.BookingId,
                            BookingCode = bookingCode,
                            SalonName = salonName,
                            CustomerName = customerName,
                            Reason = request.Reason,
                            Message = $"Khách hàng {customerName} đã hủy đơn đặt lịch #{bookingCode}. Lý do: {request.Reason}"
                        }
                    );
                }
                else
                {
                    // Salon / Manager hủy -> Gửi thông báo SignalR tới Khách hàng
                    await _notificationService.SendNotificationToUserAsync(
                        booking.CustomerId.ToString(),
                        "BookingRejected",
                        new
                        {
                            BookingId = booking.BookingId,
                            BookingCode = bookingCode,
                            SalonName = salonName,
                            CustomerName = customerName,
                            Reason = request.Reason,
                            Message = $"Đơn đặt lịch #{bookingCode} của bạn tại Salon {salonName} đã bị Salon hủy. Lý do: {request.Reason}"
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NotificationError] Failed to send booking cancelled/rejected notification: {ex.Message}");
            }
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Hủy đơn đặt lịch thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDTO request, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            //if (booking.Status != BookingStatus.Pending)
            //{
            //    return new ApiErrorResult<BookingResponseDTO>("Không thể cập nhật đơn đặt lịch đã được xử lý hoặc đã hủy.");
            //}

            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<BookingResponseDTO>("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            }

            decimal oldPrice = booking.TotalPrice ?? 0;
            int oldDuration = booking.TotalDuration;

            int totalDuration = 0;
            decimal totalPrice = 0;
            var bookingItems = new List<BookingItem>();
            var newCustomNailRequests = new List<CustomerNailRequest>();
            Booking? oldBooking = null;
            if (booking.WarrantyForBookingId.HasValue)
            {
                oldBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.WarrantyForBookingId.Value);
            }
            foreach (var x in request.BookingItems)
            {
                CustomerNailRequest? createdCustomNailRequest = null;
                CustomerNail? customerNail = null;
                bool isWarrantyItem = oldBooking != null && IsItemFromOldBooking(x, oldBooking.BookingItems);
                if (x.CustomerNailId.HasValue)
                {
                    customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(x.CustomerNailId.Value);
                    if (customerNail == null)
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy mẫu móng custom ID {x.CustomerNailId.Value}");
                    }

                    createdCustomNailRequest = new CustomerNailRequest
                    {
                        CustomerNailRequestId = Guid.NewGuid(),
                        CustomerNailId = x.CustomerNailId.Value,
                        SalonId = booking.SalonId,
                        Status = CustomerNailStatus.Quoted,
                        Price = null,
                        Duration = null,
                        CreatedAt = DateTime.UtcNow.AddHours(7)
                    };

                    await _unitOfWork.CustomerNailRequestRepository.CreateAsync(createdCustomNailRequest);
                    x.CustomerNailRequestId = createdCustomNailRequest.CustomerNailRequestId;
                }

                var item = new BookingItem
                {
                    BookingItemId = Guid.NewGuid(),
                    BookingId = bookingId,
                    Quantity = x.Quantity,
                    ServiceId = x.ServiceId,
                    NailVariantId = x.NailVariantId,
                    ShapeMethodConfigId = x.ShapeMethodConfigId,
                    CustomerNailRequestId = x.CustomerNailRequestId
                };

                if (!x.NailVariantId.HasValue && !x.ServiceId.HasValue && !x.CustomerNailRequestId.HasValue && !x.CustomerNailId.HasValue)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ, một mẫu nail hoặc một mẫu custom.");
                }
                decimal itemPrice = 0;
                int itemDuration = 0;

                if (x.NailVariantId.HasValue)
                {
                    var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(x.NailVariantId.Value);
                    if (variant != null)
                    {
                        if (!isWarrantyItem)
                        {
                            itemPrice += variant.Price;
                        }
                        itemDuration += (variant.Duration ?? 60);
                    }
                    else
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy mẫu nail có ID {x.NailVariantId.Value}");
                    }
                }

                if (x.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(x.ServiceId.Value);
                    if (service != null)
                    {
                        itemPrice += service.Price;
                        itemDuration += service.Duration;
                    }
                    else
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy dịch vụ có ID {x.ServiceId.Value}");
                    }
                }

                if (x.CustomerNailRequestId.HasValue)
                {
                    var customNailRequest = createdCustomNailRequest
                        ?? await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(x.CustomerNailRequestId.Value);
                    if (customNailRequest == null)
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy yêu cầu mẫu móng custom ID {x.CustomerNailRequestId.Value}");
                    }

                    if (customNailRequest.SalonId != booking.SalonId ||
                        (customNailRequest.Status != CustomerNailStatus.Approved && customNailRequest.Status != CustomerNailStatus.Quoted))
                    {
                        return new ApiErrorResult<BookingResponseDTO>("Yêu cầu mẫu móng custom chưa được duyệt báo giá hoặc không thuộc chi nhánh này.");
                    }

                    customerNail ??= await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customNailRequest.CustomerNailId);
                    if (customerNail == null)
                    {
                        return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy mẫu móng custom của yêu cầu này.");
                    }

                    if (!isWarrantyItem)
                    {
                        itemPrice += customNailRequest.Price ?? customerNail.Price ?? 0;
                    }
                    itemDuration += customNailRequest.Duration ?? customerNail.Duration ?? 60;
                }

                if (x.ShapeMethodConfigId.HasValue)
                {
                    var shapeMethodConfig = await _unitOfWork.ShapeMethodConfigRepository.GetByIdAsync(x.ShapeMethodConfigId.Value);
                    if (shapeMethodConfig == null)
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy cấu hình cách làm dáng móng ID {x.ShapeMethodConfigId.Value}");
                    }

                    if (x.NailVariantId.HasValue)
                    {
                        var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(x.NailVariantId.Value);
                        if (variant?.NailShapeId != shapeMethodConfig.NailShapeId)
                        {
                            return new ApiErrorResult<BookingResponseDTO>("Cấu hình cách làm không thuộc dáng móng đã chọn.");
                        }
                    }

                    if (x.CustomerNailRequestId.HasValue)
                    {
                        customerNail ??= await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(
                            (createdCustomNailRequest ?? await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(x.CustomerNailRequestId.Value))!.CustomerNailId);
                        if (customerNail?.NailShapeId != shapeMethodConfig.NailShapeId)
                        {
                            return new ApiErrorResult<BookingResponseDTO>("Cấu hình cách làm không thuộc dáng móng đã chọn.");
                        }
                    }

                    if (!isWarrantyItem)
                    {
                        itemPrice += shapeMethodConfig.Price;
                    }
                    itemDuration += shapeMethodConfig.Duration;
                }

                item.Price = itemPrice;
                item.Duration = itemDuration;

                totalDuration += item.Duration * Math.Max(item.Quantity, 1);
                totalPrice += item.Price * Math.Max(item.Quantity, 1);

                bookingItems.Add(item);
            }
            var localDate = (request.BookingDate.Kind == DateTimeKind.Utc ? request.BookingDate.AddHours(7) : request.BookingDate).Date;
            var dayOfWeek = (int)localDate.DayOfWeek;

            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(booking.SalonId);
            var operatingHours = salon?.OperatingHours?.Where(x => x.DayOfWeek == dayOfWeek).ToList() ?? new List<SalonOperatingHour>();
            var targetEndTime = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));

            if (!operatingHours.IsWithinOperatingHours(request.StartTime, targetEndTime))
            {
                return new ApiErrorResult<BookingResponseDTO>("Thời gian cập nhật đặt lịch không nằm trong giờ hoạt động của Salon.");
            }
            if (request.NailArtistId.HasValue)
            {
                var artistBreaks = await _unitOfWork.NailArtistBreakRepository.GetApprovedBreaksByArtistAndDateAsync(request.NailArtistId.Value, request.BookingDate);
                bool overlapsBreak = artistBreaks.Any(b => request.StartTime < b.EndTime && targetEndTime > b.StartTime);
                if (overlapsBreak)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Thợ nail đã đăng ký nghỉ trong khung giờ này.");
                }
                var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(request.NailArtistId.Value);
                int capacity = artist?.ConcurrentCapacity ?? 1;
                var mockProcs = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(request.BookingItems.ToList(), booking.SalonId);
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(mockProcs, request.StartTime);
                var isConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                    request.NailArtistId.Value, request.BookingDate, timeline, capacity, bookingId);
                if (isConflict)
                {
                    // Quét thợ phụ rảnh hỗ trợ các bước bị lố của thợ chính
                    var activeArtists = await _unitOfWork.NailArtistRepository.GetArtistsWithSkillsBySalonIdAsync(booking.SalonId);
                    var candidateSecondary = activeArtists.Where(x => x.NailArtistId != request.NailArtistId.Value)
                                                          .ToList();

                    bool hasSecondaryFallback = false;
                    foreach (var candidate in candidateSecondary)
                    {
                        var isSecondaryConflict = await _bookingSchedulingService.HasCapacityConflictAsync(
                                                                                                            candidate.NailArtistId,
                                                                                                            request.BookingDate,
                                                                                                            timeline,
                                                                                                            candidate.ConcurrentCapacity,
                                                                                                            bookingId
                                                                                                          );
                        if (!isSecondaryConflict)
                        {
                            hasSecondaryFallback = true;
                            break;
                        }
                    }

                    if (!hasSecondaryFallback)
                    {
                        return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này thợ chính và thợ phụ đều bận, xin chọn giờ khác.");
                    }
                }
            }

            var selectedPromotionIds = request.SelectedPromotionIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList()
                ?? booking.BookingDiscounts
                    .Where(discount => discount.PromotionId.HasValue && !discount.IsAutoApplied)
                    .Select(discount => discount.PromotionId!.Value)
                    .Distinct()
                    .ToList();
            var applicablePromotions = await _promotionService.GetApplicablePromotionsAsync(
                booking.CustomerId,
                bookingItems,
                selectedPromotionIds.Any() ? selectedPromotionIds : null);
            var (promotionDiscountAmount, appliedBookingDiscounts) =
                await _promotionService.CalculateDiscountsAsync(new Booking
                {
                    BookingId = bookingId,
                    CustomerId = booking.CustomerId,
                    BookingItems = bookingItems
                }, applicablePromotions);

            var loyaltyResult = await _loyaltyTierService.GetMyLoyaltyAsync(booking.CustomerId);
            if (!loyaltyResult.IsSucceeded)
            {
                return new ApiErrorResult<BookingResponseDTO>(loyaltyResult.Message);
            }

            var loyaltyDiscountAmount = decimal.Round(
                totalPrice * loyaltyResult.Data.LoyaltyTier.DiscountRate,
                0,
                MidpointRounding.AwayFromZero);

            if (loyaltyDiscountAmount > 0)
            {
                appliedBookingDiscounts.Add(new BookingDiscount
                {
                    BookingId = bookingId,
                    Name = $"{loyaltyResult.Data.LoyaltyTier.Name} Tier",
                    DiscountAmount = loyaltyDiscountAmount,
                    IsAutoApplied = true,
                    AppliedDate = DateTime.UtcNow.AddHours(7),
                    LoyaltyTierId = loyaltyResult.Data.LoyaltyTier.LoyaltyTierId
                });
            }

            var totalDiscountAmount = promotionDiscountAmount + loyaltyDiscountAmount;

            // BẮT ĐẦU TRANSACTION AN TOÀN TRÁNH RACE CONDITION KHI CẬP NHẬT BOOKING
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Tạo các CustomerNailRequest mới (nếu có)
                foreach (var req in newCustomNailRequests)
                {
                    await _unitOfWork.CustomerNailRequestRepository.CreateAsync(req);
                }

                // 2. Xóa các items cũ khỏi DB trước
                var oldItems = await _unitOfWork.BookingItemRepository.GetBookingItemsByBookingIdAsync(bookingId);
                foreach (var oldItem in oldItems)
                {
                    oldItem.Booking = null!;
                    oldItem.NailVariant = null;
                    oldItem.Service = null;
                    oldItem.CustomerNailRequest = null;
                    _unitOfWork.BookingItemRepository.Delete(oldItem);
                }

                booking.BookingDate = request.BookingDate;
                booking.StartTime = request.StartTime;
                booking.NailArtistId = request.NailArtistId;

                booking.Price = totalPrice;
                booking.Discount = -totalDiscountAmount;
                booking.TotalPrice = Math.Max(0, totalPrice - totalDiscountAmount);
                booking.AmountDue = Math.Max(0, (booking.TotalPrice ?? 0) - (booking.AmountPaid ?? 0));
                booking.TotalDuration = totalDuration;
                booking.UpdatedAt = DateTime.UtcNow;

                // Clear list trong memory của booking
                booking.BookingItems.Clear();
                var oldDiscounts = await _unitOfWork.BookingDiscountRepository.GetByBookingIdAsync(bookingId);
                foreach (var oldDiscount in oldDiscounts)
                {
                    oldDiscount.Booking = null!;
                    oldDiscount.Promotion = null;
                    oldDiscount.LoyaltyTier = null;
                    oldDiscount.LoyaltyTransaction = null;
                    _unitOfWork.BookingDiscountRepository.Delete(oldDiscount);
                }
                booking.BookingDiscounts.Clear();

                booking.Updated(oldPrice, oldDuration, actorId);

                booking.Customer = null!;
                booking.Salon = null!;
                booking.NailArtist = null;
                booking.BookingHistories.Clear();

                _unitOfWork.BookingRepository.Update(booking);

                // 3. Tạo các BookingItem mới
                foreach (var item in bookingItems)
                {
                    await _unitOfWork.BookingItemRepository.CreateAsync(item);
                }

                foreach (var discount in appliedBookingDiscounts)
                {
                    await _unitOfWork.BookingDiscountRepository.CreateAsync(discount);
                }

                await _unitOfWork.SaveChangesAsync();

                // 4. Tạo các quy trình (Procedures) mặc định cho booking sau khi cập nhật
                foreach (var item in bookingItems)
                {
                    await _bookingProcedureService.DuplicateProceduresForBookingItemAsync(item);
                }
                await _unitOfWork.SaveChangesAsync();

                // 5. Tính toán timeline và gán ngay lập tức nếu đã chọn thợ
                if (booking.NailArtistId.HasValue)
                {
                    var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId, trackChanges: true);
                    if (procedures.Any())
                    {
                        var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);
                        var primaryArtistId = booking.NailArtistId.Value;
                        var activeArtists = await _unitOfWork.NailArtistRepository.GetArtistsWithSkillsBySalonIdAsync(booking.SalonId);
                        var allArtistIds = activeArtists.Select(x => x.NailArtistId).ToList();
                        var allBusySegments = await _unitOfWork.BookingProcedureRepository.GetArtistBusySegmentsForArtistsByDateAsync(
                            allArtistIds, booking.BookingDate, excludingBookingId: booking.BookingId);
                        var busySegmentsByArtist = allBusySegments.GroupBy(x => x.AssignedArtistId!.Value)
                            .ToDictionary(g => g.Key, g => g.ToList());
                        var primaryBusySegments = busySegmentsByArtist.GetValueOrDefault(primaryArtistId) ?? new List<ProcedureScheduleSegment>();

                        var secondaryAssignments = new List<(NailArtist SecondaryArtist, string ProcedureName, TimeSpan StartTime, TimeSpan EndTime)>();

                        foreach (var segment in timeline)
                        {
                            var procedure = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                            procedure.EstimatedStartTime = segment.StartTime;
                            procedure.EstimatedEndTime = segment.EndTime;
                            if (procedure.ActiveDuration > 0 && procedure.IsMainStep)
                            {
                                bool primaryHasConflictForSegment = _bookingSchedulingService.HasCapacityConflictInMemory(
                                    primaryArtistId, primaryBusySegments, new List<ProcedureScheduleSegment> { segment }, capacity: 1);
                                if (!primaryHasConflictForSegment)
                                {
                                    procedure.AssignedArtistId = primaryArtistId;
                                }
                                else
                                {
                                    NailArtist? chosenSecondary = null;
                                    if (request.SecondaryArtistId.HasValue)
                                    {
                                        chosenSecondary = activeArtists.FirstOrDefault(x => x.NailArtistId == request.SecondaryArtistId.Value);
                                    }

                                    chosenSecondary ??= activeArtists.FirstOrDefault(candidate =>
                                        candidate.NailArtistId != primaryArtistId &&
                                        !_bookingSchedulingService.HasCapacityConflictInMemory(
                                            candidate.NailArtistId,
                                            busySegmentsByArtist.GetValueOrDefault(candidate.NailArtistId) ?? new List<ProcedureScheduleSegment>(),
                                            new List<ProcedureScheduleSegment> { segment },
                                            candidate.ConcurrentCapacity)
                                    );

                                    procedure.AssignedArtistId = chosenSecondary?.NailArtistId ?? primaryArtistId;

                                    if (chosenSecondary != null)
                                    {
                                        secondaryAssignments.Add((chosenSecondary, procedure.ProcedureName, segment.StartTime, segment.EndTime));
                                    }
                                }
                            }
                            _unitOfWork.BookingProcedureRepository.Update(procedure);
                        }
                        await _unitOfWork.SaveChangesAsync();

                        // Bắn thông báo SignalR cho Lễ tân và Thợ phụ khi có công đoạn bị phân công cho thợ phụ
                        foreach (var assign in secondaryAssignments)
                        {
                            var secName = assign.SecondaryArtist.Account != null 
                                ? $"{assign.SecondaryArtist.Account.FirstName} {assign.SecondaryArtist.Account.LastName}".Trim() 
                                : "Thợ phụ";

                            var customerName = booking.Customer?.User != null
                                ? $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}".Trim()
                                : "Khách hàng";

                            int overflowMinutes = (int)(assign.EndTime - assign.StartTime).TotalMinutes;

                            var availableSecondaryList = activeArtists
                                .Where(candidate => candidate.NailArtistId != primaryArtistId &&
                                    !_bookingSchedulingService.HasCapacityConflictInMemory(
                                        candidate.NailArtistId,
                                        busySegmentsByArtist.GetValueOrDefault(candidate.NailArtistId) ?? new List<ProcedureScheduleSegment>(),
                                        timeline.Where(t => t.StartTime == assign.StartTime && t.EndTime == assign.EndTime).ToList(),
                                        candidate.ConcurrentCapacity))
                                .Select(candidate => new
                                {
                                    NailArtistId = candidate.NailArtistId,
                                    FullName = candidate.Account != null 
                                        ? $"{candidate.Account.FirstName} {candidate.Account.LastName}".Trim() 
                                        : "Thợ phụ"
                                })
                                .ToList();

                            string primaryArtistName = booking.NailArtist?.Account != null ? $"{booking.NailArtist.Account.FirstName} {booking.NailArtist.Account.LastName}".Trim() : "Thợ chính";
                            string salonName = booking.Salon?.Name ?? "Salon";
                            if (salonName == "Salon")
                            {
                                var salonObj = await _unitOfWork.SalonRepository.GetByIdAsync(booking.SalonId);
                                if (salonObj != null) salonName = salonObj.Name;
                            }

                            await _notificationService.SendNotificationToSalonReceptionistsAsync(
                                booking.SalonId.ToString(),
                                "BookingUpdatedWithSecondaryArtistNotification",
                                new
                                {
                                    BookingId = booking.BookingId,
                                    SalonName = salonName,
                                    CustomerName = customerName,
                                    PrimaryArtistId = primaryArtistId,
                                    PrimaryArtistName = primaryArtistName,
                                    SecondaryArtistId = assign.SecondaryArtist.NailArtistId,
                                    SecondaryArtistName = secName,
                                    ProcedureName = assign.ProcedureName,
                                    StartTime = assign.StartTime,
                                    EndTime = assign.EndTime,
                                    OverflowMinutes = overflowMinutes,
                                    AvailableSecondaryArtists = availableSecondaryList,
                                    Message = $"Khách {customerName} đổi dịch vụ bị lố {overflowMinutes} phút. Hệ thống gợi ý {secName} làm thợ phụ từ {assign.StartTime:hh\\:mm} - {assign.EndTime:hh\\:mm}."
                                }
                            );

                            if (request.SecondaryArtistId.HasValue && assign.SecondaryArtist.AccountId != Guid.Empty)
                            {
                                await _notificationService.SendNotificationToUserAsync(
                                    assign.SecondaryArtist.AccountId.ToString(),
                                    "SecondaryArtistTaskAssignedNotification",
                                    new
                                    {
                                        BookingId = booking.BookingId,
                                        SalonName = salonName,
                                        CustomerName = customerName,
                                        ProcedureName = assign.ProcedureName,
                                        StartTime = assign.StartTime,
                                        EndTime = assign.EndTime,
                                        Message = $"Bạn được phân công làm thợ phụ hỗ trợ ca lố đơn {booking.BookingId}, công đoạn '{assign.ProcedureName}' ({assign.StartTime:hh\\:mm} - {assign.EndTime:hh\\:mm})."
                                    }
                                );
                            }
                        }
                    }
                }

                await _unitOfWork.CommitTransactionAsync();

                var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
                var response = _mapper.Map<BookingResponseDTO>(savedBooking);
                return new ApiSuccessResult<BookingResponseDTO>(response, "Cập nhật đơn đặt lịch thành công.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi xảy ra khi UpdateBookingAsync");
                return new ApiErrorResult<BookingResponseDTO>("Có lỗi hệ thống xảy ra khi cập nhật đơn hàng.");
            }
        }
        private async Task<DiscountedPriceCalculation> CalculateDiscountedPriceAsync(
        Guid customerId,
        decimal price)
        {
            var loyaltyResult = await _loyaltyTierService.GetMyLoyaltyAsync(customerId);
            if (!loyaltyResult.IsSucceeded)
            {
                return DiscountedPriceCalculation.Failure(loyaltyResult.Message);
            }

            var discountRate = loyaltyResult.Data.LoyaltyTier.DiscountRate;
            var discountAmount = decimal.Round(price * discountRate, 0, MidpointRounding.AwayFromZero);
            return DiscountedPriceCalculation.Success(discountAmount, price - discountAmount);
        }

        public async Task<ApiResult<BookingResponseDTO>> LateCheckInBookingAsync(Guid bookingId, Guid actorId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.Cancelled)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể khôi phục Check-in trễ cho đơn ở trạng thái 'Cancelled'. Trạng thái hiện tại: '{booking.Status}'.");
            }

            booking.ReopenAndCheckInLate(actorId);
            _unitOfWork.BookingRepository.Update(booking);
            var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(bookingId, trackChanges: true);
            if (procedures.Any() && booking.NailArtistId.HasValue)
            {
                var timeline = _bookingSchedulingService.BuildProcedureTimeline(procedures, booking.StartTime);
                foreach (var segment in timeline)
                {
                    var procedure = procedures.First(x => x.BookingProcedureId == segment.BookingProcedureId);
                    procedure.EstimatedStartTime = segment.StartTime;
                    procedure.EstimatedEndTime = segment.EndTime;
                    if (procedure.ActiveDuration > 0 && procedure.IsMainStep)
                    {
                        procedure.AssignedArtistId = booking.NailArtistId.Value;
                    }
                    _unitOfWork.BookingProcedureRepository.Update(procedure);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Khôi phục và Check-in đơn trễ cho khách thành công.");
        }

        public async Task<ApiResult<string>> HandleCustomerDelayDecisionAsync(Guid bookingId, DelayResponseRequest request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<string>("Không tìm thấy lịch đặt");
            }
            switch (request.CustomerDecision)
            {
                case DelayCustomerDecision.Wait:
                    if (booking.Customer != null)
                    {
                        booking.Customer.LoyaltyPoint += 50;
                        booking.Customer.LifetimePoints += 50;
                        _unitOfWork.CustomerRepository.Update(booking.Customer);

                        var pointTransaction = new LoyaltyTransaction
                        {
                            CustomerId = booking.Customer.UserId,
                            BookingId = booking.BookingId,
                            Points = 50,
                            TransactionType = LoyaltyTransactionType.Earned,
                            Description = "Điểm đền bù do đồng ý chờ ca trễ",
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.LoyaltyTransactionRepository.CreateAsync(pointTransaction);
                    }
                    break;
                case DelayCustomerDecision.Reassign:
                    var newArtist = await _unitOfWork.NailArtistRepository
                                                     .GetAvailableAlternativeArtistAsync(booking.SalonId, booking.NailArtistId.Value, booking.BookingDate.Date, booking.StartTime, booking.TotalDuration);

                    if (newArtist == null)
                    {
                        return new ApiErrorResult<string>("Xin lỗi, hiện salon không còn thợ nào rảnh. Vui lòng đồng ý chờ thêm hoặc dời lịch.");
                    }

                    booking.NailArtistId = newArtist.NailArtistId;
                    _unitOfWork.BookingRepository.Update(booking);

                    string newAssignCustomerName = booking.Customer?.User != null ? $"{booking.Customer.User.FirstName} {booking.Customer.User.LastName}".Trim() : "Khách hàng";
                    string newAssignSalonName = booking.Salon?.Name ?? "Salon";
                    if (newAssignSalonName == "Salon")
                    {
                        var salonObj = await _unitOfWork.SalonRepository.GetByIdAsync(booking.SalonId);
                        if (salonObj != null) newAssignSalonName = salonObj.Name;
                    }

                    await _notificationService.SendNotificationToUserAsync(
                        newArtist.AccountId.ToString(),
                        "NewAssignment",
                        new
                        {
                            BookingId = booking.BookingId,
                            SalonName = newAssignSalonName,
                            CustomerName = newAssignCustomerName,
                            Message = $"Bạn có ca mới được assign cho đơn #{booking.BookingId.ToString().Substring(0, 8).ToUpper()} do thợ trước bị kẹt khách."
                        }
                    );
                    break;
                case DelayCustomerDecision.Reschedule:
                    if (request.NewDate.HasValue && request.NewTime.HasValue)
                    {
                        booking.ProposedBookingDate = request.NewDate.Value;
                        booking.ProposedStartTime = request.NewTime.Value;
                    }
                    booking.Status = BookingStatus.ReschedulePending;
                    booking.ProposedBy = "Customer";
                    _unitOfWork.BookingRepository.Update(booking);
                    await _promotionService.AddVoucherForRescheduleAsync(booking.BookingId);
                    break;
                default:
                    return new ApiErrorResult<string>("Lựa chọn không hợp lệ.");
            }
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<string>($"Đã xử lý quyết định thành công: {request.CustomerDecision}");
        }

        private sealed record DiscountedPriceCalculation(
           bool IsSucceeded,
           decimal DiscountAmount,
           decimal TotalPrice,
           string? ErrorMessage)
        {
            public static DiscountedPriceCalculation Success(decimal discountAmount, decimal totalPrice)
                => new(true, discountAmount, totalPrice, null);

            public static DiscountedPriceCalculation Failure(string message)
                => new(false, 0, 0, message);
        }
    }
}
