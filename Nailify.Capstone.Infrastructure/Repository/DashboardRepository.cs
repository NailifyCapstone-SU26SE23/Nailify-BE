using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly NailifyDbContext _context;

        public DashboardRepository(NailifyDbContext context)
        {
            _context = context;
        }

        // Admin Dashboard
        public async Task<int> GetActiveSalonsCountAsync()
            => await _context.Salons.AsNoTracking().CountAsync(s => s.Status == "Open");

        public async Task<decimal> GetPlatformRevenueAsync(DateTime start, DateTime end)
            => await _context.Transactions.AsNoTracking()
                .Where(t => t.Status == TransactionStatus.Paid && t.CreatedAt >= start && t.CreatedAt <= end)
                .SumAsync(t => t.Amount);

        public async Task<int> GetRegisteredCustomersCountAsync()
            => await _context.Customers.AsNoTracking().CountAsync();

        public async Task<int> GetActiveStaffCountAsync()
            => await _context.NailArtists.AsNoTracking().CountAsync(n => n.Status == "Active");

        public async Task<double?> GetPlatformAverageRatingAsync()
            => await _context.BookingRatings.AsNoTracking().Select(r => (double?)r.OverallScore).AverageAsync();

        public async Task<int> GetActivePromotionsCountAsync(DateTime now)
            => await _context.Promotions.AsNoTracking().CountAsync(p => p.Status == "Active" && p.StartDate <= now && (p.EndDate == null || p.EndDate >= now));

        public async Task<List<(DateTime Date, decimal Total)>> GetRevenueTrendAsync(DateTime start, DateTime end)
        {
            var result = await _context.Transactions.AsNoTracking()
                .Where(t => t.Status == TransactionStatus.Paid && t.CreatedAt >= start && t.CreatedAt <= end)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return result.Select(r => (r.Date, r.Total)).ToList();
        }

        public async Task<List<(DateTime Date, int Count)>> GetCustomerGrowthAsync(DateTime start, DateTime end)
        {
            var result = await _context.Users.AsNoTracking()
                .Where(u => u.Role == UserRole.Customer && u.CreatedAt >= start && u.CreatedAt <= end)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return result.Select(r => (r.Date, r.Count)).ToList();
        }

        public async Task<List<Booking>> GetPaidBookingsForPeriodAsync(DateTime start, DateTime end)
        {
            return await _context.Bookings.AsNoTracking()
                .Where(b => b.BookingDate >= start && b.BookingDate <= end)
                .Include(b => b.Salon)
                .Include(b => b.BookingItems)
                    .ThenInclude(i => i.Service)
                .Include(b => b.BookingItems)
                    .ThenInclude(i => i.NailVariant)
                        .ThenInclude(v => v.NailDesign)
                .Include(b => b.BookingDiscounts)
                    .ThenInclude(d => d.Promotion)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetPaidTransactionsForBookingsAsync(IEnumerable<Guid> bookingIds)
        {
            var idList = bookingIds.ToList();
            return await _context.Transactions.AsNoTracking()
                .Where(t => idList.Contains((Guid)t.BookingId) && t.Status == TransactionStatus.Paid)
                .ToListAsync();
        }

        public async Task<List<BookingRating>> GetSalonRatingsForPeriodAsync(DateTime start, DateTime end)
        {
            return await _context.BookingRatings.AsNoTracking()
                .Where(r => r.Booking.BookingDate >= start && r.Booking.BookingDate <= end)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Salon)
                .ToListAsync();
        }

        // Nail Artist Dashboard
        public async Task<List<Booking>> GetNailArtistBookingsForPeriodAsync(Guid artistId, DateTime start, DateTime end)
        {
            return await _context.Bookings.AsNoTracking()
                .Where(b => b.NailArtistId == artistId && b.BookingDate >= start && b.BookingDate <= end)
                .Include(b => b.Customer)
                    .ThenInclude(c => c.User)
                .Include(b => b.Rating)
                .OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<double?> GetNailArtistAverageRatingAsync(Guid artistId, DateTime start, DateTime end)
        {
            return await _context.BookingRatings.AsNoTracking()
                .Where(r => r.Booking.NailArtistId == artistId && r.Booking.BookingDate >= start && r.Booking.BookingDate <= end)
                .Select(r => (double?)r.OverallScore)
                .AverageAsync();
        }

        public async Task<decimal> GetNailArtistEarningsAsync(IEnumerable<Guid> bookingIds)
        {
            var idList = bookingIds.ToList();
            return await _context.Transactions.AsNoTracking()
                .Where(t => idList.Contains((Guid)t.BookingId) && t.Status == TransactionStatus.Paid)
                .SumAsync(t => t.Amount);
        }

        public async Task<List<(DateTime Date, decimal Total)>> GetNailArtistEarningsTrendAsync(IEnumerable<Guid> bookingIds, DateTime start, DateTime end)
        {
            var idList = bookingIds.ToList();
            var result = await _context.Transactions.AsNoTracking()
                .Where(t => idList.Contains((Guid)t.BookingId) && t.Status == TransactionStatus.Paid && t.CreatedAt >= start && t.CreatedAt <= end)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return result.Select(r => (r.Date, r.Total)).ToList();
        }

        public async Task<List<NailArtistBreak>> GetNailArtistBreaksForPeriodAsync(Guid artistId, DateTime start, DateTime end)
        {
            return await _context.NailArtistBreaks.AsNoTracking()
                .Where(b => b.NailArtistId == artistId && b.BreakDate >= start && b.BreakDate <= end)
                .ToListAsync();
        }

        public async Task<List<NailArtistSkill>> GetNailArtistSkillsAsync(Guid artistId)
        {
            return await _context.NailArtistSkills.AsNoTracking()
                .Where(s => s.NailArtistId == artistId)
                .Include(s => s.SkillType)
                .ToListAsync();
        }

        public async Task<List<BookingRating>> GetNailArtistRecentFeedbackAsync(Guid artistId, int count = 5)
        {
            return await _context.BookingRatings.AsNoTracking()
                .Where(r => r.Booking.NailArtistId == artistId)
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        // Receptionist Dashboard
        public async Task<List<WalkInQueue>> GetWalkInQueueForDayAsync(Guid salonId, DateTime startOfDay, DateTime endOfDay)
        {
            return await _context.WalkInQueues.AsNoTracking()
                .Where(w => w.SalonId == salonId && w.Status == QueueStatus.Waiting && w.ArrivalTime >= startOfDay && w.ArrivalTime <= endOfDay)
                .OrderBy(w => w.QueuePosition)
                .ToListAsync();
        }

        public async Task<List<BookingWaitlist>> GetWaitlistForDayAsync(Guid salonId, DateTime startOfDay, DateTime endOfDay)
        {
            return await _context.BookingWaitlists.AsNoTracking()
                .Where(w => w.SalonId == salonId && w.Status == WaitlistStatus.Waiting && w.RequestedDate >= startOfDay && w.RequestedDate <= endOfDay)
                .Include(w => w.Customer)
                    .ThenInclude(c => c.User)
                .Include(w => w.PreferredNailArtist)
                    .ThenInclude(a => a.Account)
                .OrderBy(w => w.Position)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetTodaysBookingsForSalonAsync(Guid salonId, DateTime startOfDay, DateTime endOfDay)
        {
            return await _context.Bookings.AsNoTracking()
                .Where(b => b.SalonId == salonId && b.BookingDate >= startOfDay && b.BookingDate <= endOfDay)
                .Include(b => b.NailArtist)
                    .ThenInclude(n => n.Account)
                .Include(b => b.Customer)
                    .ThenInclude(c => c.User)
                .Include(b => b.Chair)
                .ToListAsync();
        }

        public async Task<List<NailArtist>> GetActiveStaffForSalonAsync(Guid salonId)
        {
            return await _context.NailArtists.AsNoTracking()
                .Where(a => a.Account.SalonId == salonId && a.Status == "Active")
                .ToListAsync();
        }

        public async Task<List<Guid>> GetArtistIdsOnBreakAsync(Guid salonId, DateTime date, TimeSpan timeOfDay)
        {
            return await _context.NailArtistBreaks.AsNoTracking()
                .Where(b => b.NailArtist.Account.SalonId == salonId && b.BreakDate == date && b.StartTime <= timeOfDay && b.EndTime >= timeOfDay)
                .Select(b => b.NailArtistId)
                .ToListAsync();
        }

        public async Task<List<Chair>> GetChairsForSalonAsync(Guid salonId)
        {
            return await _context.Chairs.AsNoTracking()
                .Where(c => c.SalonId == salonId)
                .ToListAsync();
        }

        public async Task<List<NailArtistBreak>> GetSalonBreaksForDayAsync(Guid salonId, DateTime startOfDay, DateTime endOfDay)
        {
            return await _context.NailArtistBreaks.AsNoTracking()
                .Where(b => b.NailArtist.Account.SalonId == salonId && b.BreakDate >= startOfDay && b.BreakDate <= endOfDay)
                .Include(b => b.NailArtist)
                    .ThenInclude(a => a.Account)
                .ToListAsync();
        }

        // Salon Manager Dashboard
        public async Task<List<Booking>> GetSalonBookingsForPeriodAsync(Guid salonId, DateTime start, DateTime end)
        {
            return await _context.Bookings.AsNoTracking()
                .Where(b => b.SalonId == salonId && b.BookingDate >= start && b.BookingDate <= end)
                .Include(b => b.NailArtist)
                    .ThenInclude(n => n.Account)
                .Include(b => b.Rating)
                .Include(b => b.Customer)
                    .ThenInclude(c => c.LoyaltyTier)
                .Include(b => b.Customer)
                    .ThenInclude(c => c.User)
                .Include(b => b.Chair)
                .Include(b => b.BookingDiscounts)
                    .ThenInclude(d => d.Promotion)
                .Include(b => b.BookingItems)
                    .ThenInclude(i => i.Service)
                .Include(b => b.BookingItems)
                    .ThenInclude(i => i.NailVariant)
                        .ThenInclude(v => v.NailDesign)
                .ToListAsync();
        }

        public async Task<int> GetSalonActiveStaffCountAsync(Guid salonId)
        {
            return await _context.NailArtists.AsNoTracking()
                .CountAsync(a => a.Account.SalonId == salonId && a.Status == "Active");
        }

        public async Task<List<NailArtistBreak>> GetSalonBreaksForPeriodAsync(Guid salonId, DateTime start, DateTime end)
        {
            return await _context.NailArtistBreaks.AsNoTracking()
                .Where(b => b.NailArtist.Account.SalonId == salonId && b.BreakDate >= start && b.BreakDate <= end)
                .Include(b => b.NailArtist.Account)
                .ToListAsync();
        }
    }
}
