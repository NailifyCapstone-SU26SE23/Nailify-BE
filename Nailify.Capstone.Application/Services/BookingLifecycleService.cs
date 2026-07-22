using AutoMapper;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Helpers;
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

        public BookingLifecycleService(
                                        IUnitOfWork unitOfWork,
                                        IMapper mapper,
                                        IWalkInQueueService queueService,
                                        IBookingSchedulingService bookingSchedulingService,
                                        ILoyaltyTierService loyaltyTierService,
                                        ILogger<BookingService> logger,
                                        IBookingProcedureService bookingProcedureService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _queueService = queueService;
            _bookingSchedulingService = bookingSchedulingService;
            _loyaltyTierService = loyaltyTierService;
            _logger = logger;
            _bookingProcedureService = bookingProcedureService;
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

                _unitOfWork.BookingRepository.Update(booking);
                //await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
                await _unitOfWork.SaveChangesAsync();
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
            if (booking.Status != BookingStatus.Approved && booking.Status != BookingStatus.CheckedIn)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể check-in đơn đã được xác nhận duyệt ('Approved') hoặc đang check-in. Trạng thái hiện tại: '{booking.Status}'.");
            }
            /*
            booking.CheckInImageUrl = request.CheckInImageUrl;
            booking.Status = BookingStatus.CheckedIn;
            booking.UpdatedAt = DateTime.UtcNow;
            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "CheckedIn",
                Payload = $"Check-in thành công. Đã chụp trạng thái tay trước khi làm: {request.CheckInImageUrl}",
                CreatedAt = DateTime.UtcNow
            };
            */
            booking.CheckIn(request.CheckInImageUrl, actorId);
            _unitOfWork.BookingRepository.Update(booking);
            //await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            if (booking.IsLateArrival)
            {
                var originalArtistId = booking.NailArtistId;
                booking.NailArtistId = null;

                var procedures = await _unitOfWork.BookingProcedureRepository.GetProceduresByBookingIdAsync(booking.BookingId);
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
                            : "Khách hàng Check-in thành công.");
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
            _unitOfWork.BookingRepository.Update(booking);
            //await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
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
            booking.Reject(actorId, request.Reason);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Từ chối đơn đặt lịch thành công.");
        }
        public async Task<ApiResult<BookingResponseDTO>> CancelBookingAsync(Guid bookingId, Guid customerId, CancelBookingRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId, trackChanges: true);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }

            // if (booking.CustomerId != customerId)
            // {
            //     return new ApiErrorResult<BookingResponseDTO>("Bạn không có quyền hủy lịch hẹn của người khác.");
            // }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Approved)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ được hủy đơn ở trạng thái 'Pending' hoặc 'Approved'. Trạng thái hiện tại: '{booking.Status}'.");
            }

            booking.Cancel(customerId, request.Reason);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();
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

            foreach (var x in request.BookingItems)
            {
                CustomerNailRequest? createdCustomNailRequest = null;
                CustomerNail? customerNail = null;

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
                        CreatedAt = DateTime.UtcNow
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
                        itemPrice += variant.Price;
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

                    itemPrice += customNailRequest.Price ?? customerNail.Price ?? 0;
                    itemDuration += customNailRequest.Duration ?? customerNail.Duration ?? 60;
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
                    return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này thợ đã bận, xin chọn giờ khác.");
                }
            }

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
                var priceResult = await CalculateDiscountedPriceAsync(booking.CustomerId, totalPrice);
                if (!priceResult.IsSucceeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiErrorResult<BookingResponseDTO>(priceResult.ErrorMessage!);
                }

                booking.Price = totalPrice;
                booking.Discount = -priceResult.DiscountAmount;
                booking.TotalPrice = priceResult.TotalPrice;
                booking.AmountDue = Math.Max(0, priceResult.TotalPrice - (booking.AmountPaid ?? 0));
                booking.TotalDuration = totalDuration;
                booking.UpdatedAt = DateTime.UtcNow;

                // Clear list trong memory của booking
                booking.BookingItems.Clear();

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
                        await _unitOfWork.SaveChangesAsync();
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
