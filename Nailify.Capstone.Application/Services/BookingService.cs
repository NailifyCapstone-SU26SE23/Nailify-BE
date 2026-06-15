using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
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
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQRService _qrService;
        private readonly IBookingProcedureService _bookingProcedureService;
        private readonly INailVariantService _nailVariantService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IQRService qrService, IBookingProcedureService bookingProcedureService, INailVariantService nailVariantService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _qrService = qrService;
            _bookingProcedureService = bookingProcedureService;
            _nailVariantService = nailVariantService;
        }

        public async Task<ApiResult<BookingResponseDTO>> CheckInBookingAsync(CheckInRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            if (booking.Status != BookingStatus.Approved)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể check-in đơn đã được xác nhận duyệt ('Approved'). Trạng thái hiện tại: '{booking.Status}'.");
            }

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
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Khách hàng Check-in thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CheckOutBookingAsync(CheckOutRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            string finalUrls = string.Join(",", request.CheckOutImagesUrl);
            booking.CheckOutImagesUrl = finalUrls;
            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = DateTime.UtcNow;

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "Completed",
                Payload = $"Hoàn thành dịch vụ. Ảnh trạng thái tay sau khi làm: {finalUrls}",
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Khách hàng Check-out thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CreateBookingAsync(Guid customerId, CreateBookingRequestDTO request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<BookingResponseDTO>("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            }

            var bookingId = Guid.NewGuid();
            int totalDuration = 0;
            decimal totalPrice = 0;
            var bookingItems = _mapper.Map<List<BookingItem>>(request.BookingItems);

            foreach (var item in bookingItems)
            {
                item.BookingId = bookingId;

                if (!item.NailVariantId.HasValue && !item.ServiceId.HasValue)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ hoặc một mẫu nail.");
                }

                decimal itemPrice = 0;
                int itemDuration = 0;

                if (item.NailVariantId.HasValue)
                {
                    var variant = await _nailVariantService.GetNailVariantByIdAsync(item.NailVariantId.Value);
                    if (variant?.Data != null)
                    {
                        itemPrice += variant.Data.Price;
                        itemDuration += (variant.Data.Duration ?? 60);
                    }
                    else
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy mẫu nail có ID {item.NailVariantId.Value}");
                    }
                }

                if (item.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServicesRepository.GetByIdAsync(item.ServiceId.Value);
                    if (service != null)
                    {
                        itemPrice += service.Price;
                        itemDuration += service.Duration;
                    }
                    else
                    {
                        return new ApiErrorResult<BookingResponseDTO>($"Không tìm thấy dịch vụ có ID {item.ServiceId.Value}");
                    }
                }

                item.Price = itemPrice;
                item.Duration = itemDuration;

                totalDuration += item.Duration;
                totalPrice += item.Price;
            }
            string qrCodeToken = $"NAILIFY|{bookingId}|{request.BookingDate:yyyyMMdd}";
            string qrCodeBase64 = _qrService.GenerateQRCode(qrCodeToken);

            var booking = _mapper.Map<Booking>(request);
            booking.BookingId = bookingId;
            booking.CustomerId = customerId;
            booking.TotalPrice = totalPrice;
            booking.Price = totalPrice.ToString("N0") + " VND";
            booking.TotalDuration = totalDuration;
            booking.QRCode = qrCodeBase64;
            booking.Status = BookingStatus.Pending;
            booking.BookingItems = bookingItems;

            if (request.NailArtistId.HasValue)
            {
                var targetEndTime = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictAsync(
                    request.NailArtistId.Value,
                    request.BookingDate,
                    request.StartTime,
                    targetEndTime
                );
                if (isConflict)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này thợ đã bận, xin chọn giờ khác.");
                }
            }

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "BookingCreated",
                Payload = $"Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.",
                ActorId = customerId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BookingRepository.CreateAsync(booking);
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            foreach (var item in booking.BookingItems)
            {
                if (item.NailVariantId.HasValue)
                {
                    await _bookingProcedureService.DuplicateProceduresForBookingItemAsync(item.BookingItemId, item.NailVariantId.Value);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Tạo đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<ArtistAvailabilityResponseDTO>> GetArtistAvailableSlotAsync(GetArtistAvailableSlotsRequestDTO request)
        {
            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.NailArtistId);
            if (artist == null)
            {
                return new ApiErrorResult<ArtistAvailabilityResponseDTO>("Không tìm thấy thợ nail.");
            }

            var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(request.NailArtistId, request.BookingDate);
            if (schedule == null)
            {
                return new ApiErrorResult<ArtistAvailabilityResponseDTO>("Thợ nail không có lịch làm việc trong ngày này.");
            }

            var bookings = await _unitOfWork.BookingRepository.GetBookingsByArtistAndDateAsync(request.NailArtistId, request.BookingDate);

            var busySlots = bookings.Select(x => new BusyTimeSlotResponseDto
            {
                StartTime = x.StartTime,
                EndTime = x.StartTime.Add(TimeSpan.FromMinutes(x.TotalDuration))
            })
            .OrderBy(x => x.StartTime)
            .ToList();

            var timeSlots = new List<TimeSlotResponseDTO>();
            var currentStart = schedule.ShiftStart;
            var slotInterval = TimeSpan.FromMinutes(30);

            while (currentStart + slotInterval <= schedule.ShiftEnd)
            {
                var currentEnd = currentStart + slotInterval;
                bool isAvailable = !busySlots.Any(busy => currentStart < busy.EndTime && currentEnd > busy.StartTime);

                timeSlots.Add(new TimeSlotResponseDTO
                {
                    StartTime = currentStart,
                    EndTime = currentEnd,
                    IsAvailable = isAvailable
                });

                currentStart = currentEnd;
            }

            var response = new ArtistAvailabilityResponseDTO
            {
                NailArtistId = artist.NailArtistId,
                ArtistName = $"{artist.Account.FirstName} {artist.Account.LastName}",
                AvatarUrl = artist.Account.AvatarUrl ?? "",
                AvailabilityStatus = "Working",
                ShiftStart = schedule.ShiftStart,
                ShiftEnd = schedule.ShiftEnd,
                BusySlots = busySlots,
                TimeSlots = timeSlots
            };
            return new ApiSuccessResult<ArtistAvailabilityResponseDTO>(response, "Lấy thông tin slot làm việc thành công.");
        }

        public async Task<ApiResult<List<SuggestedArtistResponseDTO>>> GetSuggestedArtistAsync(GetSuggestedArtistsRequestDTO request)
        {
            if(request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<List<SuggestedArtistResponseDTO>>("Vui lòng chọn mẫu nail trước khi tìm thợ.");
            }
            var bookingItems = _mapper.Map<List<BookingItem>>(request.BookingItems);
            if (bookingItems.Any(item => !item.NailVariantId.HasValue && !item.ServiceId.HasValue))
            {
                return new ApiErrorResult<List<SuggestedArtistResponseDTO>>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ hoặc một mẫu nail.");
            }

            var variantIds =  _unitOfWork.NailVariantRepository.GetDistinctVariantIdsAsync(bookingItems);
            IEnumerable<NailArtist> suggestedArtist;

            if(variantIds.Any())
            {
                suggestedArtist = await _unitOfWork.NailArtistRepository.GetSuggestedArtistsAsync(request.SalonId, variantIds);
            }
            else
            {
                var activeArtists = await _unitOfWork.NailArtistRepository.GetNailArtistsBySalonIdAsync(request.SalonId);
                suggestedArtist = activeArtists.Where(x => x.Status == "Active");
            }
            var response = _mapper.Map<List<SuggestedArtistResponseDTO>>(suggestedArtist);

            return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(response, "Lấy danh sách thợ đề xuất thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId)
                                            ;
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không thể cập nhật đơn đặt lịch đã được xử lý hoặc đã hủy.");
            }

            if (request.BookingItems == null || !request.BookingItems.Any())
            {
                return new ApiErrorResult<BookingResponseDTO>("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            }

            int totalDuration = 0;
            decimal totalPrice = 0;
            var bookingItems = new List<BookingItem>();

            foreach (var x in request.BookingItems)
            {
                var item = new BookingItem
                {
                    BookingItemId = Guid.NewGuid(),
                    BookingId = bookingId,
                    Quantity = x.Quantity,
                    ServiceId = x.ServiceId,
                    NailVariantId = x.NailVariantId
                };

                if (!x.NailVariantId.HasValue && !x.ServiceId.HasValue)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Mỗi mục đặt lịch phải chứa ít nhất một dịch vụ hoặc một mẫu nail.");
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

                item.Price = itemPrice;
                item.Duration = itemDuration;

                totalDuration += item.Duration;
                totalPrice += item.Price;

                bookingItems.Add(item);
            }

            if (request.NailArtistId.HasValue)
            {
                var targetEndTime = request.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictExcludingCurrentAsync(
                    request.NailArtistId.Value,
                    request.BookingDate,
                    request.StartTime,
                    targetEndTime,
                    bookingId
                );
                if (isConflict)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này thợ đã bận, xin chọn giờ khác.");
                }
            }

            // Xóa các items cũ khỏi DB trước
            var oldItems = await _unitOfWork.BookingItemRepository.GetBookingItemsByBookingIdAsync(bookingId);
            foreach (var oldItem in oldItems)
            {
                oldItem.Booking = null!;
                oldItem.NailVariant = null;
                oldItem.Service = null;
                oldItem.CustomerNail = null;
                _unitOfWork.BookingItemRepository.Delete(oldItem);
            }

            booking.BookingDate = request.BookingDate;
            booking.StartTime = request.StartTime   ;
            booking.NailArtistId = request.NailArtistId;
            booking.TotalPrice = totalPrice;
            booking.Price = totalPrice.ToString("N0") + " VND";
            booking.TotalDuration = totalDuration;
            booking.UpdatedAt = DateTime.UtcNow;

            // Clear list trong memory của booking
            booking.BookingItems.Clear();

            _unitOfWork.BookingRepository.Update(booking);

            foreach (var item in bookingItems)
            {
                await _unitOfWork.BookingItemRepository.CreateAsync(item);
            }

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "BookingUpdated",
                Payload = $"Đơn đặt lịch được cập nhật. Tổng tiền mới: {booking.Price}. Tổng thời gian: {totalDuration} phút.",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();

            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Cập nhật đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CreateCustomBookingAsync(Guid customerId, CreateCustomBookingRequestDTO request)
        {
            var customNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(request.CustomerNailId);
            if (customNail == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy mẫu nail tùy chỉnh.");
            }

            var bookingId = Guid.NewGuid();
            string qrCodeToken = $"NAILIFY|{bookingId}|{request.BookingDate:yyyyMMdd}";
            string qrCodeBase64 = _qrService.GenerateQRCode(qrCodeToken);

            var booking = new Booking
            {
                BookingId = bookingId,
                CustomerId = customerId,
                SalonId = request.SalonId,
                BookingDate = request.BookingDate,
                StartTime = request.StartTime,
                Status = BookingStatus.Pending,
                TotalPrice = 0,
                Price = "0 VND",
                TotalDuration = 0,
                QRCode = qrCodeBase64,
                BookingItems = new List<BookingItem>
                {
                    new BookingItem
                    {
                        BookingItemId = Guid.NewGuid(),
                        BookingId = bookingId,
                        CustomerNailId = request.CustomerNailId,
                        Quantity = 1,
                        Price = 0,
                        Duration = 0
                    }
                }
            };

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "CustomBookingCreated",
                Payload = $"Khách hàng gửi yêu cầu đặt mẫu nail tùy chỉnh '{customNail.Name}'. Chờ quản lý phân bổ thợ.",
                ActorId = customerId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BookingRepository.CreateAsync(booking);
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();

            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Gửi yêu cầu đặt mẫu nail tùy chỉnh thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> AssignArtistAsync(Guid bookingId, AssignArtistRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không ở trạng thái chờ phân bổ thợ.");
            }

            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.StaffArtistId);
            if (artist == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thợ nail.");
            }

            booking.NailArtistId = request.StaffArtistId;
            booking.Status = BookingStatus.Assigned;
            booking.UpdatedAt = DateTime.UtcNow;

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = BookingStatus.Assigned.ToString(),
                Payload = $"Quản lý đã chỉ định thợ {artist.Account.FirstName} {artist.Account.LastName} thẩm định và báo giá.",
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();

            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Chỉ định thợ nail thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ArtistQuoteAsync(Guid bookingId, ArtistQuoteRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != BookingStatus.Assigned)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không ở trạng thái chờ thợ báo giá.");
            }

            booking.TotalPrice = request.QuotedPrice;
            booking.Price = request.QuotedPrice.ToString("N0") + " VND";
            booking.TotalDuration = request.QuotedDuration;
            booking.Status = BookingStatus.Reviewed;
            booking.UpdatedAt = DateTime.UtcNow;

            var item = booking.BookingItems.FirstOrDefault(bi => bi.CustomerNailId.HasValue);
            if (item != null)
            {
                item.Price = request.QuotedPrice;
                item.Duration = request.QuotedDuration;
            }

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = BookingStatus.Reviewed.ToString(),
                Payload = $"Thợ nail đề xuất giá: {booking.Price}, thời gian: {request.QuotedDuration} phút.",
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();

            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Thợ nail đề xuất giá và thời lượng thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ManagerApproveQuoteAsync(Guid bookingId, ManagerApproveQuoteRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != BookingStatus.Reviewed)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không ở trạng thái chờ quản lý duyệt báo giá.");
            }

            booking.TotalPrice = request.FinalPrice;
            booking.Price = request.FinalPrice.ToString("N0") + " VND";
            booking.TotalDuration = request.FinalDuration;
            booking.Status = BookingStatus.Pending;
            booking.UpdatedAt = DateTime.UtcNow;

            var item = booking.BookingItems.FirstOrDefault(bi => bi.CustomerNailId.HasValue);
            if (item != null)
            {
                item.Price = request.FinalPrice;
                item.Duration = request.FinalDuration;
            }

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "ManagerApprovedQuote",
                Payload = $"Quản lý đã duyệt báo giá cuối cùng: {booking.Price}, thời gian: {request.FinalDuration} phút. Đơn đặt lịch sẵn sàng phục vụ.",
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();

            var savedBooking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(booking.BookingId);
            var response = _mapper.Map<BookingResponseDTO>(savedBooking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Quản lý chốt báo giá và gửi khách hàng thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> CancelBookingAsync(Guid bookingId, Guid customerId, CancelBookingRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
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

            var oldStatus = booking.Status;
            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.BookingRepository.Update(booking);
            var history = new BookingHistory
            {
                BookingId = booking.BookingId,
                EventType = BookingStatus.Cancelled.ToString(),
                Payload = $"Hủy đơn từ trạng thái '{oldStatus}' sang 'Cancelled'. Lý do: {request.Reason}",
                ActorId = customerId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Hủy đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> ConfirmBookingAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.Pending)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể xác nhận đơn ở trạng thái 'Pending'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            booking.Status = BookingStatus.Approved;
            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.BookingRepository.Update(booking);
            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "BookingConfirmed",
                Payload = "Quản lý Salon xác nhận duyệt đơn đặt lịch.",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Duyệt đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> RejectBookingAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.Pending)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể từ chối đơn ở trạng thái 'Pending'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            booking.Status = BookingStatus.Rejected;
            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.BookingRepository.Update(booking);
            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "BookingRejected",
                Payload = "Quản lý Salon từ chối đơn đặt lịch.",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Từ chối đơn đặt lịch thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> StartServiceAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không tồn tại.");
            }
            if (booking.Status != BookingStatus.CheckedIn)
            {
                return new ApiErrorResult<BookingResponseDTO>($"Chỉ có thể bắt đầu làm khi khách đã 'CheckedIn'. Trạng thái hiện tại: '{booking.Status}'.");
            }
            booking.Status = BookingStatus.InProgress;
            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.BookingRepository.Update(booking);
            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "ServiceStarted",
                Payload = "Thợ làm móng bắt đầu thực hiện các dịch vụ trong đơn.",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Bắt đầu làm móng thành công.");
        }

        public async Task<ApiResult<IEnumerable<BookingResponseDTO>>> GetMyBookingsAsync(Guid customerId)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsByCustomerAsync(customerId);
            var response = _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
            return new ApiSuccessResult<IEnumerable<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của khách hàng thành công.");
        }

        public async Task<ApiResult<IEnumerable<BookingResponseDTO>>> GetBookingsBySalonAsync(Guid salonId)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsBySalonAsync(salonId);
            var response = _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
            return new ApiSuccessResult<IEnumerable<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của Salon thành công.");
        }

        public async Task<ApiResult<IEnumerable<BookingResponseDTO>>> GetBookingsByArtistAsync(Guid artistId)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsByArtistAsync(artistId);
            var response = _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
            return new ApiSuccessResult<IEnumerable<BookingResponseDTO>>(response, "Lấy danh sách đặt lịch của Thợ làm móng thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }
            var response = _mapper.Map<BookingResponseDTO>(booking);
            return new ApiSuccessResult<BookingResponseDTO>(response, "Lấy thông tin chi tiết đặt lịch thành công.");
        }
    }
}
