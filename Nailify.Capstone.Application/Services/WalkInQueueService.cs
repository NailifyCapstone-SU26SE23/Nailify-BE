using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalkInQueueResponseDTOs;
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
    public class WalkInQueueService : IWalkInQueueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBookingSchedulingService _bookingSchedulingService;

        public WalkInQueueService(IUnitOfWork unitOfWork, IMapper mapper, IBookingSchedulingService bookingSchedulingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookingSchedulingService = bookingSchedulingService;
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> AddToQueueAsync(Guid actorId, AddToQueueRequestDTO request)
        {
            var nextPost = await _unitOfWork.WalkInQueueRepository.GetNextPositionAsync(request.SalonId);
            var queue = _mapper.Map<WalkInQueue>(request);
            queue.QueuePosition = nextPost;
            queue.Status = QueueStatus.Waiting;
            queue.ArrivalTime = DateTime.UtcNow;

            var items = request.BookingItems ?? new List<BookingItemRequestDTO>();
            if (request.OriginalBookingId.HasValue && !items.Any())
            {
                var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(request.OriginalBookingId.Value);
                if (booking != null)
                {
                    items = booking.BookingItems.Select(x => new BookingItemRequestDTO
                    {
                        ServiceId = x.ServiceId,
                        NailVariantId = x.NailVariantId,
                        CustomerNailRequestId = x.CustomerNailRequestId,
                        Quantity = x.Quantity
                    }).ToList();
                }
            }
            queue.EstimatedWait = await CalculateEstimatedWaitTimeAsync(request.SalonId, items);
            await _unitOfWork.WalkInQueueRepository.CreateAsync(queue);
            await _unitOfWork.SaveChangesAsync();
            // Recalculate cho toàn bộ hàng chờ hiện tại
            await RecalculateQueueWaitTimesAsync(request.SalonId);
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã thêm khách vào hàng chờ vãng lai thành công.");
            /*
            await _unitOfWork.WalkInQueueRepository.CreateAsync(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã thêm khách vào hàng chờ vãng lai thành công.");
            */
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> AssignArtistAsync(Guid queueId, AssignQueueArtistRequestDTO request, Guid actorId)
        {
            var queue = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if (queue == null)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy bản ghi hàng chờ.");
            }

            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.NailArtistId);
            if (artist == null || artist.Status != "Active")
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Thợ làm móng không hoạt động hoặc không tồn tại.");
            }

            int capacity = artist.ConcurrentCapacity;
            var localNow = DateTime.UtcNow.AddHours(7);
            var todayDate = localNow.Date;
            var currentTime = localNow.TimeOfDay;

            // Lấy danh sách BookingItems của lượt hàng chờ này
            var items = new List<BookingItemRequestDTO>();
            if (queue.OriginalBookingId.HasValue)
            {
                var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(queue.OriginalBookingId.Value);
                if (booking != null)
                {
                    items = booking.BookingItems.Select(x => new BookingItemRequestDTO
                    {
                        ServiceId = x.ServiceId,
                        NailVariantId = x.NailVariantId,
                        CustomerNailRequestId = x.CustomerNailRequestId,
                        Quantity = x.Quantity
                    }).ToList();
                }
            }

            var walkInProcs = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(items, queue.SalonId);
            if (!walkInProcs.Any())
            {
                var bookProcedures = new BookingProcedure
                {
                    Duration = 30,
                    ActiveDuration = 20,
                    PassiveDuration = 10,
                    CanOverlap = true,
                    TransitionBuffer = 1
                };

                walkInProcs.Add(bookProcedures);
            }

            var timeline = _bookingSchedulingService.BuildProcedureTimeline(walkInProcs, currentTime);

            var isConflict = await _bookingSchedulingService.HasSimulationConflictAsync(
                request.NailArtistId,
                todayDate,
                timeline,
                new List<ProcedureScheduleSegment>(),
                capacity
            );

            if (isConflict || await WillDelayApprovedBookingsAsync(request.NailArtistId, todayDate, timeline))
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>(
                    $"Thợ {artist.Account.FirstName} {artist.Account.LastName} không có đủ thời gian trống hoặc sẽ làm trễ lịch hẹn đã đặt trước. " +
                    $"Vui lòng chọn thợ khác hoặc chờ thợ hoàn thành công việc.");
            }

            queue.AssignedNailArtistId = request.NailArtistId;
            _unitOfWork.WalkInQueueRepository.Update(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Phân bổ thợ nail thành công.");
        }

        public async Task<int> CalculateEstimatedWaitTimeAsync(Guid salonId, List<BookingItemRequestDTO> requestedItems)
        {
            var activeArtists = await _unitOfWork.NailArtistRepository.GetNailArtistsBySalonIdAsync(salonId);
            var workingArtists = activeArtists.Where(a => a.Status == "Active").ToList();
            if (!workingArtists.Any())
            {
                return 60;
            }
            // Giả lập mock procedure cho khách hàng mới này
            var mockProcedures = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(requestedItems, salonId);
            if (!mockProcedures.Any())
            {
                var bookProcedures = new BookingProcedure
                {
                    Duration = 30,
                    ActiveDuration = 20,
                    PassiveDuration = 10,
                    CanOverlap = true,
                    TransitionBuffer = 1
                };
                // Mặc định 30 phút nếu không có dịch vụ cụ thể
                mockProcedures.Add(bookProcedures);
            }
            int totalDuration = mockProcedures.Sum(p => p.Duration);

            var localNow = DateTime.UtcNow.AddHours(7).TimeOfDay;
            var minWaitTime = int.MaxValue;

            var today = DateTime.UtcNow.Date;
            var queueList = await _unitOfWork.WalkInQueueRepository.GetTodayQueueAsync(salonId);
            var waitingQueue = queueList
                                       .Where(q => q.Status == QueueStatus.Waiting || q.Status == QueueStatus.Called)
                                       .OrderBy(q => q.QueuePosition)
                                       .ToList();
            foreach (var artist in workingArtists)
            {
                var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(artist.NailArtistId, DateTime.Today);
                if (schedule == null)
                {
                    continue;
                }
                var candidateStart = localNow < schedule.ShiftStart ? schedule.ShiftStart : localNow;
                var artistSimulatedSegments = new List<ProcedureScheduleSegment>();

                var waitingQueueForArtist = waitingQueue.Where(x => x.AssignedNailArtistId == artist.NailArtistId || !x.AssignedNailArtistId.HasValue);
                foreach (var ahead in waitingQueueForArtist)
                {
                    // Lấy các item của khách đi trước
                    var aheadItems = new List<BookingItemRequestDTO>();
                    if (ahead.OriginalBookingId.HasValue)
                    {
                        var b = await _unitOfWork.BookingRepository.GetBookingDetailAsync(ahead.OriginalBookingId.Value);
                        if (b != null)
                        {
                            aheadItems = b.BookingItems.Select(x => new BookingItemRequestDTO { ServiceId = x.ServiceId, NailVariantId = x.NailVariantId, CustomerNailRequestId = x.CustomerNailRequestId, Quantity = x.Quantity }).ToList();
                        }
                    }
                    var aheadProcs = await _bookingSchedulingService.GenerateMockBookingProceduresAsync(aheadItems, salonId);
                    if (!aheadProcs.Any())
                    {
                        var bookingProcedures = new BookingProcedure
                        {
                            Duration = 30,
                            ActiveDuration = 20,
                            PassiveDuration = 10,
                            CanOverlap = true,
                            TransitionBuffer = 1
                        };
                        aheadProcs.Add(bookingProcedures);
                    }
                    // Tìm slot rảnh sớm nhất cho khách đi trước
                    var aheadStart = candidateStart;
                    while (aheadStart.Add(TimeSpan.FromMinutes(aheadProcs.Sum(x => x.Duration))) <= schedule.ShiftEnd)
                    {
                        var aheadTimeline = _bookingSchedulingService.BuildProcedureTimeline(aheadProcs, aheadStart);

                        // Kiểm tra overlap với segment bận thực tế và các segment đã giả lập trước đó
                        bool conflict = await HasSimulationConflictAsync(artist.NailArtistId, today, aheadTimeline, artistSimulatedSegments, artist.ConcurrentCapacity);
                        if (!conflict)
                        {
                            artistSimulatedSegments.AddRange(aheadTimeline);
                            candidateStart = aheadStart.Add(TimeSpan.FromMinutes(aheadProcs.Sum(x => x.Duration)));
                            break;
                        }
                        aheadStart = aheadStart.Add(TimeSpan.FromMinutes(5));
                    }
                }
                // Tìm slot rảnh cho khách hiện tại sau khi đã xếp khách đi trước
                while (candidateStart.Add(TimeSpan.FromMinutes(totalDuration)) <= schedule.ShiftEnd)
                {
                    var testTimeline = _bookingSchedulingService.BuildProcedureTimeline(mockProcedures, candidateStart);

                    // Check conflict bao gồm cả check xem có đẩy lùi lịch hẹn trước Approved nào quá 15 phút không (Appointment Protection)
                    bool isConflict = await HasSimulationConflictAsync(artist.NailArtistId, today, testTimeline, artistSimulatedSegments, artist.ConcurrentCapacity);
                    if (!isConflict && !await WillDelayApprovedBookingsAsync(artist.NailArtistId, today, testTimeline))
                    {
                        var waitMinutes = (int)(candidateStart - localNow).TotalMinutes;
                        if (waitMinutes < minWaitTime)
                        {
                            minWaitTime = Math.Max(0, waitMinutes);
                        }
                        break;
                    }
                    candidateStart = candidateStart.Add(TimeSpan.FromMinutes(5));
                }
            }
            return minWaitTime == int.MaxValue ? 60 : minWaitTime;
        }

        private async Task<bool> HasSimulationConflictAsync(Guid artistId, DateTime date, List<ProcedureScheduleSegment> newSegments, List<ProcedureScheduleSegment> simulatedSegments, int capacity)
        {
            // Lấy segment bận thực tế trong DB
            var dbSegments = await _unitOfWork.BookingProcedureRepository.GetArtistBusySegmentsByDateAsync(artistId, date);
            var allExisting = dbSegments.Concat(simulatedSegments).ToList();
            // Check Active Capacity (limit = 1)
            foreach (var newSegment in newSegments.Where(x => x.ArtistBusyEnd > x.ArtistBusyStart))
            {
                var activeOverlap = allExisting.Count(ex =>
                    ex.ArtistBusyEnd > ex.ArtistBusyStart &&
                    ex.ArtistBusyStart < newSegment.ArtistBusyEnd &&
                    ex.ArtistBusyEnd > newSegment.ArtistBusyStart);
                if (activeOverlap >= 1) return true;
            }
            // Check Total Capacity
            foreach (var newSegment in newSegments)
            {
                var conflictingTotals = allExisting.Where(ex =>
                    ex.StartTime < newSegment.EndTime &&
                    ex.EndTime > newSegment.StartTime).ToList();

                var totalOverlapCount = conflictingTotals
                    .GroupBy(ex => ex.BookingId ?? ex.BookingItemId ?? Guid.NewGuid())
                    .Count();

                if (totalOverlapCount >= capacity) return true;
            }
            return false;
        }
        // Walk-in không được phép làm trễ lịch hẹn Approved sắp tới của thợ quá 15 phút.
        private async Task<bool> WillDelayApprovedBookingsAsync(Guid artistId, DateTime date, List<ProcedureScheduleSegment> walkInSegments)
        {
            var range = GetDateRangeUtc(date);
            var upcomingApprovedBookings = await _unitOfWork.BookingRepository.GetBookingsByArtistAndDateAsync(artistId, date);
            var approvedList = upcomingApprovedBookings.Where(b => b.Status == BookingStatus.Approved).ToList();
            foreach (var booking in approvedList)
            {
                // Kiểm tra xem walk-in có đè vào giờ hẹn của booking này không
                var bookingEnd = booking.StartTime.Add(TimeSpan.FromMinutes(booking.TotalDuration));
                var overlapWalkIn = walkInSegments.FirstOrDefault(w => w.StartTime < bookingEnd && w.EndTime > booking.StartTime);
                if (overlapWalkIn != null)
                {
                    // Nếu có đè vào, xem lượng thời gian đè (delay) là bao nhiêu
                    var delayMinutes = (overlapWalkIn.EndTime - booking.StartTime).TotalMinutes;
                    if (delayMinutes > 5)
                    {
                        return true; // Làm trễ hẹn khách đặt trước quá 5 phút -> Block!
                    }
                }
            }
            return false;
        }
        private (DateTime start, DateTime end) GetDateRangeUtc(DateTime date)
        {
            var localDate = date.Date;
            var start = DateTime.SpecifyKind(localDate.AddHours(-7), DateTimeKind.Utc);
            var end = start.AddDays(1).AddTicks(-1);
            return (start, end);
        }
        public async Task RecalculateQueueWaitTimesAsync(Guid salonId)
        {
            var today = DateTime.UtcNow.Date;
            var queueList = await _unitOfWork.WalkInQueueRepository.GetTodayQueueAsync(salonId, trackChanges: true);
            var waitingQueue = queueList
                .Where(q => q.Status == QueueStatus.Waiting || q.Status == QueueStatus.Called)
                .OrderBy(q => q.QueuePosition)
                .ToList();
            foreach (var q in waitingQueue)
            {
                var items = new List<BookingItemRequestDTO>();
                if (q.OriginalBookingId.HasValue)
                {
                    var booking = await _unitOfWork.BookingRepository.GetBookingDetailAsync(q.OriginalBookingId.Value);
                    if (booking != null)
                    {
                        items = booking.BookingItems.Select(x => new BookingItemRequestDTO
                        {
                            ServiceId = x.ServiceId,
                            NailVariantId = x.NailVariantId,
                            CustomerNailRequestId = x.CustomerNailRequestId,
                            Quantity = x.Quantity
                        }).ToList();
                    }
                }
                q.EstimatedWait = await CalculateEstimatedWaitTimeAsync(salonId, items);
                _unitOfWork.WalkInQueueRepository.Update(q);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> CallQueueAsync(Guid queueId, Guid actorId)
        {
            var queue = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if(queue == null)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy khách hàng trong hàng chờ.");
            }
            queue.Status = QueueStatus.Called;
            queue.CalledTime = DateTime.UtcNow;
            _unitOfWork.WalkInQueueRepository.Update(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã gọi khách lên quầy chuẩn bị thực hiện.");
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> CompleteQueueEntryAsync(Guid queueId, Guid actorId)
        {
            var queue = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if (queue == null)
            {     
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy bản ghi hàng chờ.");
            }
            queue.Status = QueueStatus.Done;
            _unitOfWork.WalkInQueueRepository.Update(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã hoàn thành lượt chờ của khách (chuyển sang dịch vụ/booking).");
        }

        public async Task<ApiResult<List<WalkInQueueResponseDTO>>> GetTodayQueueAsync(Guid salonId)
        {
            var queueList = await _unitOfWork.WalkInQueueRepository.GetTodayQueueAsync(salonId);
            var response = _mapper.Map<List<WalkInQueueResponseDTO>>(queueList);
            return new ApiSuccessResult<List<WalkInQueueResponseDTO>>(response, "Lấy danh sách hàng chờ hôm nay thành công.");
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> MarkLeftAsync(Guid queueId, Guid actorId)
        {
            var queue = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if (queue == null)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy khách hàng trong hàng chờ.");
            }
            queue.Status = QueueStatus.Left;
            _unitOfWork.WalkInQueueRepository.Update(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã đánh dấu khách hàng rời hàng chờ (vắng mặt/không làm).");
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> PrioritizeQueueEntryAsync(Guid queueId, Guid actorId)
        {
            var walkIn = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if (walkIn == null)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy lượt hàng chờ.");
            }
            if (walkIn.Status != QueueStatus.Waiting)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Chỉ có thể ưu tiên khách hàng đang ở trạng thái chờ.");
            }

            var oldPosition = walkIn.QueuePosition;
            if (oldPosition == 1)
            {
                return new ApiSuccessResult<WalkInQueueResponseDTO>(_mapper.Map<WalkInQueueResponseDTO>(walkIn), "Khách hàng đã ở đầu hàng chờ.");
            }

            // Lấy danh sách những người đang chờ khác cùng salon trong ngày hôm nay
            var today = DateTime.UtcNow.Date;
            var waitingList = await _unitOfWork.WalkInQueueRepository.GetActiveWaitingEntriesAsync(walkIn.SalonId, walkIn.AssignedNailArtistId,trackChanges: true);

            // Đẩy lùi vị trí các khách hàng đang đứng trước khách hàng được ưu tiên
            foreach (var item in waitingList)
            {
                if (item.QueuePosition < oldPosition)
                {
                    item.QueuePosition += 1;
                    _unitOfWork.WalkInQueueRepository.Update(item);
                }
            }

            // Đặt khách hàng được chọn lên vị trí đầu tiên
            walkIn.QueuePosition = 1;
            _unitOfWork.WalkInQueueRepository.Update(walkIn);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<WalkInQueueResponseDTO>(walkIn);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã ưu tiên khách hàng lên đầu hàng chờ tại sảnh.");
        }

    }
}
