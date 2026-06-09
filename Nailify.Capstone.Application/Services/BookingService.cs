using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
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

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IQRService qrService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _qrService = qrService;
        }

        public async Task<ApiResult<BookingResponseDTO>> CheckInBookingAsync(CheckInRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.BookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            booking.CheckInImageUrl = request.CheckInImageUrl;
            booking.Status = "CheckedIn";
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
            booking.Status = "Completed";
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

                decimal itemPrice = 0;
                int itemDuration = 0;

                if (item.NailVariantId.HasValue)
                {
                    var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(item.NailVariantId.Value);
                    if (variant != null)
                    {
                        itemPrice += variant.Price;
                        itemDuration += (variant.Duration ?? 60);
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
            booking.Status = "Pending";
            booking.BookingItems = bookingItems;

            if (request.NailArtistId.HasValue)
            {
                var targetEndTime = request.ExpectedTime.Add(TimeSpan.FromMinutes(totalDuration));
                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictAsync(
                    request.NailArtistId.Value,
                    request.BookingDate,
                    request.ExpectedTime,
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
                StartTime = x.ExpectedTime,
                EndTime = x.ExpectedTime.Add(TimeSpan.FromMinutes(x.TotalDuration))
            })
            .OrderBy(x => x.StartTime)
            .ToList();

            var response = new ArtistAvailabilityResponseDTO
            {
                NailArtistId = artist.NailArtistId,
                ArtistName = $"{artist.Account.FirstName} {artist.Account.LastName}",
                AvatarUrl = artist.Account.AvatarUrl ?? "",
                AvailabilityStatus = "Working",
                ShiftStart = schedule.ShiftStart,
                ShiftEnd = schedule.ShiftEnd,
                BusySlots = busySlots
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

            var variantIds =  _unitOfWork.NailVariantRepository.GetDistinctVariantIdsAsync(bookingItems);

            if(!variantIds.Any())
            {
                return new ApiErrorResult<List<SuggestedArtistResponseDTO>>("Vui lòng chọn mẫu nail trước khi tìm thợ.");
            }

            var suggestedArtists = await _unitOfWork.NailArtistRepository.GetSuggestedArtistsAsync(request.SalonId, variantIds);

            var response = _mapper.Map<List<SuggestedArtistResponseDTO>>(suggestedArtists);

            return new ApiSuccessResult<List<SuggestedArtistResponseDTO>>(response, "Lấy danh sách thợ đề xuất thành công.");
        }

        public async Task<ApiResult<BookingResponseDTO>> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDTO request)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(bookingId);
            if (booking == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thông tin đặt lịch.");
            }

            if (booking.Status != "Pending")
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
                var targetEndTime = request.ExpectedTime.Add(TimeSpan.FromMinutes(totalDuration));
                var isConflict = await _unitOfWork.BookingRepository.HasBookingConflictExcludingCurrentAsync(
                    request.NailArtistId.Value,
                    request.BookingDate,
                    request.ExpectedTime,
                    targetEndTime,
                    bookingId
                );
                if (isConflict)
                {
                    return new ApiErrorResult<BookingResponseDTO>("Khoảng thời gian này thợ đã bận, xin chọn giờ khác.");
                }
            }

            booking.BookingDate = request.BookingDate;
            booking.ExpectedTime = request.ExpectedTime;
            booking.NailArtistId = request.NailArtistId;
            booking.TotalPrice = totalPrice;
            booking.Price = totalPrice.ToString("N0") + " VND";
            booking.TotalDuration = totalDuration;
            booking.UpdatedAt = DateTime.UtcNow;

            booking.BookingItems.Clear();
            foreach (var item in bookingItems)
            {
                booking.BookingItems.Add(item);
            }

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "BookingUpdated",
                Payload = $"Đơn đặt lịch được cập nhật. Tổng tiền mới: {booking.Price}. Tổng thời gian: {totalDuration} phút.",
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.BookingRepository.Update(booking);
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
                ExpectedTime = request.ExpectedTime,
                Status = "CustomPending",
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

            if (booking.Status != "CustomPending")
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không ở trạng thái chờ phân bổ thợ.");
            }

            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.StaffArtistId);
            if (artist == null)
            {
                return new ApiErrorResult<BookingResponseDTO>("Không tìm thấy thợ nail.");
            }

            booking.NailArtistId = request.StaffArtistId;
            booking.Status = "ArtistAssigned";
            booking.UpdatedAt = DateTime.UtcNow;

            var history = new BookingHistory
            {
                BookingHistoryId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                EventType = "ArtistAssigned",
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

            if (booking.Status != "ArtistAssigned")
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không ở trạng thái chờ thợ báo giá.");
            }

            booking.TotalPrice = request.QuotedPrice;
            booking.Price = request.QuotedPrice.ToString("N0") + " VND";
            booking.TotalDuration = request.QuotedDuration;
            booking.Status = "ArtistQuoted";
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
                EventType = "ArtistQuoted",
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

            if (booking.Status != "ArtistQuoted")
            {
                return new ApiErrorResult<BookingResponseDTO>("Đơn đặt lịch không ở trạng thái chờ quản lý duyệt báo giá.");
            }

            booking.TotalPrice = request.FinalPrice;
            booking.Price = request.FinalPrice.ToString("N0") + " VND";
            booking.TotalDuration = request.FinalDuration;
            booking.Status = "Pending";
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
    }
}
