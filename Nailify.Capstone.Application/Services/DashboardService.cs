using Microsoft.Extensions.Caching.Distributed;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.Dashboard;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using System.Text.Json;

namespace Nailify.Capstone.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;

        public DashboardService(IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Default to today if not provided (GMT+7 Vietnam Time)
            var start = startDate.HasValue ? (startDate.Value.Kind == DateTimeKind.Utc ? startDate.Value.AddHours(7) : startDate.Value).Date : DateTime.UtcNow.AddHours(7).Date;
            var end = endDate.HasValue ? (endDate.Value.Kind == DateTimeKind.Utc ? endDate.Value.AddHours(7) : endDate.Value).Date.AddDays(1).AddTicks(-1) : DateTime.UtcNow.AddHours(7).Date.AddDays(1).AddTicks(-1);

            string cacheKey = $"AdminDashboard_{start:yyyyMMdd}_{end:yyyyMMdd}";

            // 1. Try to get from Redis Cache
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<AdminDashboardDto>(cachedData) ?? new AdminDashboardDto();
            }

            var dto = new AdminDashboardDto();

            dto.TotalActiveSalons = await _unitOfWork.DashboardRepository.GetActiveSalonsCountAsync();

            dto.TotalPlatformRevenue = await _unitOfWork.DashboardRepository.GetPlatformRevenueAsync(start, end);

            dto.TotalRegisteredCustomers = await _unitOfWork.DashboardRepository.GetRegisteredCustomersCountAsync();

            dto.TotalActiveStaff = await _unitOfWork.DashboardRepository.GetActiveStaffCountAsync();

            var platformAverageRating = await _unitOfWork.DashboardRepository.GetPlatformAverageRatingAsync();
            dto.PlatformAverageRating = platformAverageRating.HasValue ? Math.Round(platformAverageRating.Value, 2) : 0;

            var now = DateTime.UtcNow;
            dto.ActivePromotionsRunning = await _unitOfWork.DashboardRepository.GetActivePromotionsCountAsync(now);

            var revenueByDay = await _unitOfWork.DashboardRepository.GetRevenueTrendAsync(start, end);

            dto.RevenueTrend.Labels = revenueByDay.Select(g => g.Date.ToString("dd/MM")).ToList();
            dto.RevenueTrend.Datasets.Add(new ChartDataset<decimal>
            {
                Label = "Revenue",
                Data = revenueByDay.Select(g => g.Total).ToList()
            });

            var customerGrowth = await _unitOfWork.DashboardRepository.GetCustomerGrowthAsync(start, end);

            dto.UserGrowth.Labels = customerGrowth.Select(g => g.Date.ToString("dd/MM")).ToList();
            dto.UserGrowth.Datasets.Add(new ChartDataset<int>
            {
                Label = "New Customers",
                Data = customerGrowth.Select(g => g.Count).ToList()
            });

            var paidBookings = await _unitOfWork.DashboardRepository.GetPaidBookingsForPeriodAsync(start, end);

            var periodBookingIds = paidBookings.Select(b => b.BookingId).ToList();
            var paidTransactions = await _unitOfWork.DashboardRepository.GetPaidTransactionsForBookingsAsync(periodBookingIds);

            var paidBookingIds = paidTransactions.Select(t => t.BookingId).ToHashSet();
            var topSalons = paidBookings
                .Where(b => paidBookingIds.Contains(b.BookingId))
                .GroupBy(b => new { b.SalonId, SalonName = b.Salon.Name })
                .Select(g => new
                {
                    g.Key.SalonName,
                    Revenue = paidTransactions.Where(t => g.Any(b => b.BookingId == t.BookingId)).Sum(t => t.Amount)
                })
                .OrderByDescending(g => g.Revenue)
                .Take(10)
                .ToList();

            dto.TopPerformingSalons.Labels = topSalons.Select(g => g.SalonName).ToList();
            dto.TopPerformingSalons.Datasets.Add(new ChartDataset<decimal>
            {
                Label = "Revenue",
                Data = topSalons.Select(g => g.Revenue).ToList()
            });

            var servicePopularity = paidBookings
                .SelectMany(b => b.BookingItems)
                .Select(i => i.Service?.Name ?? i.NailVariant?.NailDesign?.Name ?? i.NailVariant?.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .GroupBy(name => name!)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToList();

            dto.GlobalServicePopularity.Labels = servicePopularity.Select(g => g.Name).ToList();
            dto.GlobalServicePopularity.Datasets.Add(new ChartDataset<int>
            {
                Label = "Bookings",
                Data = servicePopularity.Select(g => g.Count).ToList()
            });

            var salonRatings = await _unitOfWork.DashboardRepository.GetSalonRatingsForPeriodAsync(start, end);

            dto.SalonRatingDistribution = salonRatings
                .GroupBy(r => new { r.Booking.SalonId, SalonName = r.Booking.Salon.Name })
                .Select(g => new SalonRatingDistributionDto
                {
                    SalonId = g.Key.SalonId,
                    SalonName = g.Key.SalonName,
                    AverageRating = Math.Round(g.Average(r => r.OverallScore), 2),
                    RatingCount = g.Count()
                })
                .OrderByDescending(s => s.AverageRating)
                .ToList();

            dto.GlobalPromotionPerformance = paidBookings
                .SelectMany(b => b.BookingDiscounts.Select(d => new { Booking = b, Discount = d }))
                .Where(x => x.Discount.PromotionId.HasValue || !string.IsNullOrWhiteSpace(x.Discount.Name))
                .GroupBy(x => new { x.Discount.PromotionId, PromotionName = x.Discount.Promotion?.Name ?? x.Discount.Name })
                .Select(g => new PromotionPerformanceDto
                {
                    PromotionId = g.Key.PromotionId ?? 0,
                    PromotionName = g.Key.PromotionName,
                    UsageCount = g.Count(),
                    DiscountGiven = g.Sum(x => x.Discount.DiscountAmount),
                    RevenueGenerated = paidTransactions
                        .Where(t => g.Select(x => x.Booking.BookingId).Contains(t.BookingId))
                        .Sum(t => t.Amount)
                })
                .OrderByDescending(p => p.RevenueGenerated)
                .ToList();

            // 3. Save to Redis Cache (TTL = 15 minutes)
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);

            return dto;
        }

        public async Task<NailArtistDashboardDto> GetNailArtistDashboardDataAsync(Guid artistId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate.HasValue ? (startDate.Value.Kind == DateTimeKind.Utc ? startDate.Value.AddHours(7) : startDate.Value).Date : DateTime.UtcNow.AddHours(7).Date;
            var end = endDate.HasValue ? (endDate.Value.Kind == DateTimeKind.Utc ? endDate.Value.AddHours(7) : endDate.Value).Date.AddDays(1).AddTicks(-1) : DateTime.UtcNow.AddHours(7).Date.AddDays(1).AddTicks(-1);
            var now = DateTime.UtcNow.AddHours(7);

            string cacheKey = $"NailArtistDashboard_{artistId}_{start:yyyyMMdd}_{end:yyyyMMdd}";

            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<NailArtistDashboardDto>(cachedData) ?? new NailArtistDashboardDto();
            }

            var dto = new NailArtistDashboardDto();

            var periodBookings = await _unitOfWork.DashboardRepository.GetNailArtistBookingsForPeriodAsync(artistId, start, end);

            // KPIs
            dto.RemainingAppointmentsCount = periodBookings.Count(b => (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Approved) && b.BookingDate.Add(b.StartTime) >= now);
            dto.CompletedAppointmentsCount = periodBookings.Count(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.ServiceCompleted);

            var artistRatingAverage = await _unitOfWork.DashboardRepository.GetNailArtistAverageRatingAsync(artistId, start, end);
            dto.AverageRatingScore = artistRatingAverage.HasValue ? Math.Round(artistRatingAverage.Value, 2) : 0;

            var bookingIds = periodBookings.Select(b => b.BookingId).ToList();
            dto.EstimatedEarnings = await _unitOfWork.DashboardRepository.GetNailArtistEarningsAsync(bookingIds);

            var earningsByDay = await _unitOfWork.DashboardRepository.GetNailArtistEarningsTrendAsync(bookingIds, start, end);

            dto.EarningsTracker.Labels = earningsByDay.Select(g => g.Date.ToString("dd/MM")).ToList();
            dto.EarningsTracker.Datasets.Add(new ChartDataset<decimal>
            {
                Label = "Earnings",
                Data = earningsByDay.Select(g => g.Total).ToList()
            });

            var completedWithActualTime = periodBookings
                .Where(b => (b.Status == BookingStatus.Completed || b.Status == BookingStatus.ServiceCompleted)
                    && b.ActualStartTime.HasValue
                    && b.UpdatedAt.HasValue
                    && b.UpdatedAt.Value > b.ActualStartTime.Value)
                .ToList();

            var averagePlannedMinutes = completedWithActualTime.Any()
                ? Math.Round(completedWithActualTime.Average(b => b.TotalDuration), 2)
                : 0;
            var averageActualMinutes = completedWithActualTime.Any()
                ? Math.Round(completedWithActualTime.Average(b => (b.UpdatedAt!.Value - b.ActualStartTime!.Value).TotalMinutes), 2)
                : 0;

            dto.ServiceTimeEfficiency.Labels = new List<string> { "Planned Avg", "Actual Avg" };
            dto.ServiceTimeEfficiency.Datasets.Add(new ChartDataset<double>
            {
                Label = "Minutes",
                Data = new List<double> { averagePlannedMinutes, averageActualMinutes }
            });

            var breaks = await _unitOfWork.DashboardRepository.GetNailArtistBreaksForPeriodAsync(artistId, start, end);

            var scheduleItems = new List<ArtistScheduleItemDto>();
            foreach (var b in periodBookings.Where(x => x.Status != BookingStatus.Cancelled))
            {
                scheduleItems.Add(new ArtistScheduleItemDto
                {
                    Date = b.BookingDate.Date,
                    StartTime = b.StartTime,
                    DurationMinutes = b.TotalDuration,
                    CustomerName = b.Customer?.User?.FirstName + " " + b.Customer?.User?.LastName,
                    Type = "Booking"
                });
            }

            foreach (var brk in breaks)
            {
                scheduleItems.Add(new ArtistScheduleItemDto
                {
                    Date = brk.BreakDate.Date,
                    StartTime = brk.StartTime,
                    DurationMinutes = (int)(brk.EndTime - brk.StartTime).TotalMinutes,
                    CustomerName = brk.Reason ?? "Break",
                    Type = "Break"
                });
            }

            dto.MySchedule = scheduleItems.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToList();

            // Next Customer Profile
            var nextBooking = periodBookings.FirstOrDefault(b => (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Approved) && b.BookingDate.Add(b.StartTime) >= now);
            if (nextBooking != null && nextBooking.Customer != null)
            {
                dto.NextCustomer = new NextCustomerProfileDto
                {
                    CustomerName = nextBooking.Customer.User?.FirstName + " " + nextBooking.Customer.User?.LastName,
                    PreferredComplexity = nextBooking.Customer.PreferredComplexity,
                    PreferredNailShapeId = nextBooking.Customer.PreferredNailShapeId,
                    Note = nextBooking.Customer.NailCondition ?? ""
                };
            }

            var skills = await _unitOfWork.DashboardRepository.GetNailArtistSkillsAsync(artistId);

            dto.SkillOverview = skills.Select(s => s.SkillType.Name).ToList();

            var recentFeedbacks = await _unitOfWork.DashboardRepository.GetNailArtistRecentFeedbackAsync(artistId, 5);

            dto.RecentFeedback = recentFeedbacks.Select(r => new FeedbackCardDto
            {
                CustomerName = r.Customer.User?.FirstName + " " + r.Customer.User?.LastName,
                Score = r.OverallScore,
                Comment = r.Comment ?? string.Empty,
                Date = r.CreatedAt
            }).ToList();

            // Save to Redis (TTL = 10 mins)
            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);

            return dto;
        }

        public async Task<ReceptionistDashboardDto> GetReceptionistDashboardDataAsync(Guid salonId, DateTime? date = null)
        {
            var targetDate = date.HasValue ? (date.Value.Kind == DateTimeKind.Utc ? date.Value.AddHours(7) : date.Value).Date : DateTime.UtcNow.AddHours(7).Date;
            var startOfDay = targetDate;
            var endOfDay = targetDate.AddDays(1).AddTicks(-1);
            var now = DateTime.UtcNow.AddHours(7);

            string cacheKey = $"ReceptionistDashboard_{salonId}_{targetDate:yyyyMMdd}";

            // 1. Try to get from Redis
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<ReceptionistDashboardDto>(cachedData) ?? new ReceptionistDashboardDto();
            }

            // 2. Build DTO from DB
            var dto = new ReceptionistDashboardDto();

            // Queues and Waitlists
            var walkInQueue = await _unitOfWork.DashboardRepository.GetWalkInQueueForDayAsync(salonId, startOfDay, endOfDay);

            var waitlist = await _unitOfWork.DashboardRepository.GetWaitlistForDayAsync(salonId, startOfDay, endOfDay);

            dto.CurrentWalkInQueueSize = walkInQueue.Count;
            dto.CurrentWaitlistSize = waitlist.Count;

            // Average Wait Time (Estimation based on current queue items)
            dto.AverageWaitTimeMinutes = walkInQueue.Any(w => w.EstimatedWait.HasValue)
                ? Math.Round(walkInQueue.Where(w => w.EstimatedWait.HasValue).Average(w => w.EstimatedWait.Value), 2)
                : 0;

            // Remaining Appointments Today
            var todaysBookings = await _unitOfWork.DashboardRepository.GetTodaysBookingsForSalonAsync(salonId, startOfDay, endOfDay);

            var pendingBookings = todaysBookings.Where(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Approved).ToList();
            dto.RemainingAppointmentsToday = pendingBookings.Count;

            // Staff on Duty
            var activeStaff = await _unitOfWork.DashboardRepository.GetActiveStaffForSalonAsync(salonId);

            var totalStaffCount = activeStaff.Count;
            var artistsOnBreakIds = await _unitOfWork.DashboardRepository.GetArtistIdsOnBreakAsync(salonId, targetDate, now.TimeOfDay);

            var onDutyStaffCount = totalStaffCount - artistsOnBreakIds.Count;
            dto.StaffOnDutyText = $"{onDutyStaffCount} of {totalStaffCount} artists on Duty";

            // Estimated Time to Clear Queue (Simple mock: (Avg Duration * Queue Size) / StaffOnDuty )
            var avgDuration = 30; // 30 mins
            if (onDutyStaffCount > 0 && dto.CurrentWalkInQueueSize > 0)
            {
                dto.EstimatedTimeToClearQueueMins = Math.Round((double)(avgDuration * dto.CurrentWalkInQueueSize) / onDutyStaffCount, 2);
            }

            // Live Walk-In Queue list
            dto.LiveWalkInQueue = walkInQueue.Select(w => new WalkInQueueItemDto
            {
                GuestName = w.GuestName ?? "Unknown Guest",
                RequestNote = w.RequestNote ?? "No notes",
                QueuePosition = w.QueuePosition,
                EstimatedWait = w.EstimatedWait ?? 0
            }).ToList();

            dto.LiveWaitlist = waitlist.Select(w => new WaitlistDashboardItemDto
            {
                WaitlistId = w.WailistId,
                CustomerName = w.Customer?.User != null
                    ? $"{w.Customer.User.FirstName} {w.Customer.User.LastName}"
                    : "Unknown Customer",
                RequestedStartTime = w.RequestedStartTime,
                EstimatedDuration = w.EstimatedDuration,
                Position = w.Position,
                PreferredArtistName = w.PreferredNailArtist?.Account != null
                    ? $"{w.PreferredNailArtist.Account.FirstName} {w.PreferredNailArtist.Account.LastName}"
                    : "Any artist"
            }).ToList();

            // Upcoming Arrivals (Next 2 hours)
            var upcomingBookings = pendingBookings
                .Where(b => b.BookingDate.Add(b.StartTime) >= now && b.BookingDate.Add(b.StartTime) <= now.AddHours(2))
                .OrderBy(b => b.StartTime)
                .ToList();

            dto.UpcomingArrivals = upcomingBookings.Select(b => new UpcomingArrivalDto
            {
                CustomerName = b.Customer?.User?.FirstName + " " + b.Customer?.User?.LastName,
                ArrivalTime = b.BookingDate.Add(b.StartTime),
                AssignedArtistName = b.NailArtist != null ? $"{b.NailArtist.Account.FirstName} {b.NailArtist.Account.LastName}" : "Unassigned"
            }).ToList();

            // Live Chair Status
            var allChairs = await _unitOfWork.DashboardRepository.GetChairsForSalonAsync(salonId);

            var occupiedChairs = todaysBookings
                .Where(b => b.Status == BookingStatus.InProgress && b.ChairId.HasValue)
                .ToDictionary(b => b.ChairId.Value, b => b.Customer?.User?.FirstName + " " + b.Customer?.User?.LastName);

            dto.LiveChairStatus = allChairs.Select(c => new ChairStatusDto
            {
                ChairId = c.ChairId,
                Name = c.ChairName,
                IsOccupied = occupiedChairs.ContainsKey(c.ChairId),
                CurrentCustomer = occupiedChairs.ContainsKey(c.ChairId) ? occupiedChairs[c.ChairId] : string.Empty
            }).ToList();

            var todaysBreaks = await _unitOfWork.DashboardRepository.GetSalonBreaksForDayAsync(salonId, startOfDay, endOfDay);

            var scheduleItems = todaysBookings
                .Where(b => b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Rejected)
                .Select(b => new SalonScheduleItemDto
                {
                    BookingId = b.BookingId,
                    ArtistId = b.NailArtistId,
                    ArtistName = b.NailArtist?.Account != null
                        ? $"{b.NailArtist.Account.FirstName} {b.NailArtist.Account.LastName}"
                        : "Unassigned",
                    CustomerName = b.Customer?.User != null
                        ? $"{b.Customer.User.FirstName} {b.Customer.User.LastName}"
                        : "Unknown Customer",
                    Date = b.BookingDate.Date,
                    StartTime = b.StartTime,
                    DurationMinutes = b.TotalDuration,
                    Type = "Booking",
                    Status = b.Status.ToString()
                })
                .Concat(todaysBreaks.Select(b => new SalonScheduleItemDto
                {
                    ArtistId = b.NailArtistId,
                    ArtistName = b.NailArtist?.Account != null
                        ? $"{b.NailArtist.Account.FirstName} {b.NailArtist.Account.LastName}"
                        : "Unknown Artist",
                    CustomerName = b.Reason ?? "Break",
                    Date = b.BreakDate.Date,
                    StartTime = b.StartTime,
                    DurationMinutes = (int)(b.EndTime - b.StartTime).TotalMinutes,
                    Type = "Break",
                    Status = b.Status.ToString()
                }))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToList();

            dto.MasterSalonSchedule = scheduleItems;

            // No-Show / Late Alerts (Pending/Approved bookings that are 10+ mins past StartTime)
            var lateBookings = pendingBookings
                .Where(b => b.BookingDate.Add(b.StartTime).AddMinutes(10) < now)
                .ToList();

            dto.NoShowLateAlerts = lateBookings.Select(b => new NoShowAlertDto
            {
                BookingId = b.BookingId,
                CustomerName = b.Customer?.User?.FirstName + " " + b.Customer?.User?.LastName,
                MinutesLate = (int)(now - b.BookingDate.Add(b.StartTime)).TotalMinutes
            }).ToList();

            // 3. Save to Redis Cache (TTL = 5 mins)
            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);

            return dto;
        }

        public async Task<SalonManagerDashboardDto> GetSalonDashboardDataAsync(Guid salonId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate.HasValue ? (startDate.Value.Kind == DateTimeKind.Utc ? startDate.Value.AddHours(7) : startDate.Value).Date : DateTime.UtcNow.AddHours(7).Date;
            var end = endDate.HasValue ? (endDate.Value.Kind == DateTimeKind.Utc ? endDate.Value.AddHours(7) : endDate.Value).Date.AddDays(1).AddTicks(-1) : DateTime.UtcNow.AddHours(7).Date.AddDays(1).AddTicks(-1);

            string cacheKey = $"SalonDashboard_{salonId}_{start:yyyyMMdd}_{end:yyyyMMdd}";

            // 1. Try get from Redis
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<SalonManagerDashboardDto>(cachedData) ?? new SalonManagerDashboardDto();
            }

            // 2. Query from DB
            var dto = new SalonManagerDashboardDto();

            // Bookings for the period
            var periodBookings = await _unitOfWork.DashboardRepository.GetSalonBookingsForPeriodAsync(salonId, start, end);

            // KPIs
            dto.TotalPendingBookings = periodBookings.Count(b => b.Status == BookingStatus.Pending);
            dto.TotalCompletedBookings = periodBookings.Count(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.ServiceCompleted);

            var cancelledOrNoShow = periodBookings.Count(b => b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.Rejected);
            var totalBookings = periodBookings.Count;
            dto.CancellationRate = totalBookings > 0 ? Math.Round((double)cancelledOrNoShow / totalBookings * 100, 2) : 0;

            // Revenue
            var bookingIds = periodBookings.Select(b => b.BookingId).ToList();
            var periodTransactions = await _unitOfWork.DashboardRepository.GetPaidTransactionsForBookingsAsync(bookingIds);

            dto.TodaysRevenue = periodTransactions.Sum(t => t.Amount);
            dto.AverageTicketValue = dto.TotalCompletedBookings > 0 ? Math.Round(dto.TodaysRevenue / dto.TotalCompletedBookings, 2) : 0;

            var paidBookingIds = periodTransactions.Select(t => t.BookingId).ToHashSet();
            var paidBookings = periodBookings.Where(b => paidBookingIds.Contains(b.BookingId)).ToList();

            var serviceRevenue = paidBookings
                .SelectMany(b => b.BookingItems)
                .Where(i => i.Service != null)
                .GroupBy(i => i.Service!.Name)
                .Select(g => new { Label = $"Service: {g.Key}", Total = g.Sum(i => i.Price * i.Quantity) });

            var nailDesignRevenue = paidBookings
                .SelectMany(b => b.BookingItems)
                .Where(i => i.NailVariant != null)
                .GroupBy(i => i.NailVariant!.NailDesign?.Name ?? i.NailVariant.Name)
                .Select(g => new { Label = $"Nail design: {g.Key}", Total = g.Sum(i => i.Price * i.Quantity) });

            var promotionDiscounts = paidBookings
                .SelectMany(b => b.BookingDiscounts)
                .Where(d => d.PromotionId.HasValue || !string.IsNullOrWhiteSpace(d.Name))
                .GroupBy(d => d.Promotion?.Name ?? d.Name)
                .Select(g => new { Label = $"Promotion: {g.Key}", Total = g.Sum(d => d.DiscountAmount) });

            var revenueBreakdown = serviceRevenue
                .Concat(nailDesignRevenue)
                .Concat(promotionDiscounts)
                .Where(x => x.Total > 0)
                .OrderByDescending(x => x.Total)
                .ToList();

            dto.RevenueBreakdown.Labels = revenueBreakdown.Select(x => x.Label).ToList();
            dto.RevenueBreakdown.Datasets.Add(new ChartDataset<decimal>
            {
                Label = "Amount",
                Data = revenueBreakdown.Select(x => x.Total).ToList()
            });

            var peakHours = periodBookings
                .GroupBy(b => b.StartTime.Hours)
                .OrderBy(g => g.Key)
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .ToList();

            dto.PeakHoursHeatmap.Labels = peakHours.Select(g => $"{g.Hour:00}:00").ToList();
            dto.PeakHoursHeatmap.Datasets.Add(new ChartDataset<int>
            {
                Label = "Bookings",
                Data = peakHours.Select(g => g.Count).ToList()
            });

            var chairs = await _unitOfWork.DashboardRepository.GetChairsForSalonAsync(salonId);

            dto.ChairUtilization = chairs.Select(c => new ChairUtilizationDto
            {
                ChairId = c.ChairId,
                ChairName = c.ChairName,
                Bookings = periodBookings
                    .Where(b => b.ChairId == c.ChairId)
                    .OrderBy(b => b.BookingDate)
                    .ThenBy(b => b.StartTime)
                    .Select(b => new ChairBookingTimelineDto
                    {
                        CustomerName = b.Customer?.User != null
                            ? $"{b.Customer.User.FirstName} {b.Customer.User.LastName}"
                            : "Unknown Customer",
                        StartTime = b.StartTime,
                        DurationMinutes = b.TotalDuration
                    })
                    .ToList()
            }).ToList();

            var retentionByDay = periodBookings
                .GroupBy(b => b.BookingDate.Date)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var totalCustomers = g.Select(b => b.CustomerId).Distinct().Count();
                    var returningCustomers = g
                        .Where(b => periodBookings.Any(previous => previous.CustomerId == b.CustomerId && previous.BookingDate.Date < g.Key))
                        .Select(b => b.CustomerId)
                        .Distinct()
                        .Count();

                    return new
                    {
                        Date = g.Key,
                        Rate = totalCustomers > 0 ? Math.Round((double)returningCustomers / totalCustomers * 100, 2) : 0
                    };
                })
                .ToList();

            dto.CustomerRetentionRate.Labels = retentionByDay.Select(g => g.Date.ToString("dd/MM")).ToList();
            dto.CustomerRetentionRate.Datasets.Add(new ChartDataset<double>
            {
                Label = "Returning Customers (%)",
                Data = retentionByDay.Select(g => g.Rate).ToList()
            });

            // Staff Utilization (Mock calculation)
            var totalStaff = await _unitOfWork.DashboardRepository.GetSalonActiveStaffCountAsync(salonId);
            if (totalStaff > 0)
            {
                // Simple mock: Total booked hours / (Total staff * 8 hours)
                var totalBookedMinutes = periodBookings.Where(b => b.Status != BookingStatus.Cancelled).Sum(b => b.TotalDuration);
                var availableMinutes = totalStaff * 8 * 60;
                dto.StaffUtilizationRate = availableMinutes > 0 ? Math.Round((double)totalBookedMinutes / availableMinutes * 100, 2) : 0;
            }

            // Artist Performance Leaderboard
            var artistGroups = periodBookings
                .Where(b => b.NailArtistId.HasValue && (b.Status == BookingStatus.Completed || b.Status == BookingStatus.ServiceCompleted))
                .GroupBy(b => b.NailArtistId.Value)
                .ToList();

            foreach (var group in artistGroups)
            {
                var artistBookings = group.ToList();
                var artistRev = periodTransactions.Where(t => artistBookings.Any(ab => ab.BookingId == t.BookingId)).Sum(t => t.Amount);
                var ratings = artistBookings.Where(b => b.Rating != null).Select(b => b.Rating.OverallScore).ToList();

                dto.ArtistPerformanceLeaderboard.Add(new ArtistPerformanceDto
                {
                    ArtistId = group.Key,
                    ArtistName = artistBookings.First().NailArtist?.Account?.FirstName + " " + artistBookings.First().NailArtist?.Account?.LastName,
                    CompletedBookings = artistBookings.Count,
                    RevenueGenerated = artistRev,
                    AverageRating = ratings.Any() ? Math.Round(ratings.Average(), 2) : 0
                });
            }

            dto.ArtistPerformanceLeaderboard = dto.ArtistPerformanceLeaderboard.OrderByDescending(a => a.RevenueGenerated).ToList();

            // Staff Leave Alerts
            var breaks = await _unitOfWork.DashboardRepository.GetSalonBreaksForPeriodAsync(salonId, start, end);

            dto.StaffLeaveAlerts = breaks.Select(b => new StaffLeaveAlertDto
            {
                ArtistName = $"{b.NailArtist.Account.FirstName} {b.NailArtist.Account.LastName}",
                BreakDate = b.BreakDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Reason = b.Reason ?? "Not specified"
            }).OrderBy(b => b.BreakDate).ThenBy(b => b.StartTime).ToList();

            // 3. Save to Redis
            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);

            return dto;
        }
    }
}
