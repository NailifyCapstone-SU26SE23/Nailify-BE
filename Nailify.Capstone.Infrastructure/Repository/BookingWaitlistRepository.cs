using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailify.Capstone.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class BookingWaitlistRepository : GenericRepository<BookingWaitlist>, IBookingWaitlistRepository
    {
        public BookingWaitlistRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BookingWaitlist>> GetExpiredOrPastEntriesAsync(DateTime referenceDateTime, bool trackChanges = false)
         => await FindByCondition(x => (x.Status == WaitlistStatus.Notified
                                        && x.ExpiresAt.HasValue
                                        && x.ExpiresAt.Value < referenceDateTime)
                                    || (x.Status == WaitlistStatus.Waiting
                                        && x.RequestedDate.Date < referenceDateTime.Date), trackChanges)
                  .ToListAsync();

        public async Task<int> GetNextPositionAsync(Guid salonId, DateTime date, TimeSpan startTime, Guid? preferredNailArtistId)
        {
            // Read-only count, using default AsNoTracking via FindByCondition
            var maxPosition = await FindByCondition(x => x.SalonId == salonId 
                                                    && x.RequestedDate.Date == date.Date
                                                    && x.RequestedStartTime == startTime
                                                    && x.PreferredNailArtistId == preferredNailArtistId)
                            .MaxAsync(x => (int?)x.Position) ?? 0;
            return maxPosition + 1;
        }

        public async Task<BookingWaitlist?> GetNextWaitingEntryAsync(Guid salonId, DateTime date, TimeSpan startTime, Guid? preferredNailArtistId)
          => await FindByCondition(x => x.SalonId == salonId 
                                   && x.RequestedDate.Date == date.Date
                                   && x.RequestedStartTime == startTime
                                   && x.PreferredNailArtistId == preferredNailArtistId
                                   && x.Status == WaitlistStatus.Waiting, true)
                   .Include(x => x.Customer).ThenInclude(c => c.User)
                   .Include(x => x.Salon)
                   .Include(x => x.PreferredNailArtist).ThenInclude(a => a.Account)
                   .OrderBy(x => x.Position)
                   .FirstOrDefaultAsync();   

        public async Task<BookingWaitlist?> GetWaitlistWithDetailsAsync(Guid waitlistId)
        {
            return await FindByCondition(x => x.WailistId == waitlistId, false)
                .Include(x => x.Customer).ThenInclude(c => c.User)
                .Include(x => x.Salon)
                .Include(x => x.PreferredNailArtist)
                    .ThenInclude(a => a.Account)
                .OrderBy(x => x.Position)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<BookingWaitlist>> GetSalonWaitlistWithDetailsAsync(Guid salonId, int pageNumber, int pageSize)
        {
            var query = FindByCondition(x => x.SalonId == salonId && x.Status == WaitlistStatus.Waiting, false)
                .Include(x => x.Customer).ThenInclude(c => c.User)
                .Include(x => x.Salon)
                .Include(x => x.PreferredNailArtist).ThenInclude(a => a.Account);

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<BookingWaitlist>(items, count, pageNumber, pageSize);
        }

        public async Task<bool> IsDuplicateAsync(Guid customerId, Guid salonId, DateTime date, TimeSpan startTime, Guid? preferredNailArtistId)
        {
            // Read-only check, using default AsNoTracking via FindByCondition
            return await FindByCondition(x => x.CustomerId == customerId 
                                         && x.SalonId == salonId 
                                         && x.RequestedDate.Date == date.Date
                                         && x.RequestedStartTime == startTime
                                         && x.PreferredNailArtistId == preferredNailArtistId
                                         && x.Status == WaitlistStatus.Waiting)
                        .AnyAsync();
        }

        public async Task<BookingWaitlist?> GetActiveWaitlistByCustomerAsync(Guid customerId, Guid salonId)
        {
            return await FindByCondition(x => x.CustomerId == customerId
                                            && x.SalonId == salonId
                                            && (x.Status == WaitlistStatus.Waiting || x.Status == WaitlistStatus.Notified), false)
                .Include(x => x.Customer).ThenInclude(c => c.User)
                .Include(x => x.Salon)
                .Include(x => x.PreferredNailArtist).ThenInclude(a => a.Account)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BookingWaitlist>> GetActiveWaitlistsByCustomerAsync(Guid customerId)
        {
            return await FindByCondition(x => x.CustomerId == customerId
                                            && (x.Status == WaitlistStatus.Waiting || x.Status == WaitlistStatus.Notified), false)
                .Include(x => x.Customer).ThenInclude(c => c.User)
                .Include(x => x.Salon)
                .Include(x => x.PreferredNailArtist).ThenInclude(a => a.Account)
                .Include(x => x.WaitlistItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingWaitlist>> GetActiveNotifiedWaitlistsAsync(Guid artistId, DateTime date)
        {
            var dateOnly = (date.Kind == DateTimeKind.Utc ? date.AddHours(7) : date).Date;
            return await FindByCondition(x => x.PreferredNailArtistId == artistId
                                         && x.RequestedDate.Date == dateOnly
                                         && x.Status == WaitlistStatus.Notified
                                         && x.ExpiresAt.HasValue
                                         && x.ExpiresAt.Value > DateTime.UtcNow, false)
                         .Include(x => x.WaitlistItems)
                         .ToListAsync();
        }
        public async Task<BookingWaitlist?> GetWaitlistWithItemsAsync(Guid waitlistId)
        {
            return await FindByCondition(x => x.WailistId == waitlistId, false)
                .Include(x => x.Customer).ThenInclude(c => c.User)
                .Include(x => x.Salon)
                .Include(x => x.PreferredNailArtist).ThenInclude(a => a.Account)
                .Include(x => x.WaitlistItems)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetActiveWailistCountAsync(Guid salonId, DateTime date, TimeSpan startTime)
        {
            var dateOnly = (date.Kind == DateTimeKind.Utc ? date.AddHours(7) : date).Date;
            return await FindByCondition(x => x.SalonId == salonId
                                         && x.RequestedDate.Date == dateOnly
                                         && x.RequestedStartTime == startTime
                                         && (
                                         x.Status == WaitlistStatus.Waiting 
                                         || x.Status == WaitlistStatus.Notified
                                         ), false)
                .CountAsync();
        }

        public async Task<BookingWaitlist?> GetSmartNextWaitingEntryAsync(Guid salonId, DateTime date, TimeSpan startTime, Guid? preferredNailArtistId, int freedDurationMinutes, int continuousWindowMinutes)
        {
            // 1. Lấy tất cả Waitlist entries ở trạng thái Waiting cho khung giờ & salon này
            var candidates = await FindByCondition(x => x.SalonId == salonId
                                                       && x.RequestedDate.Date == date.Date
                                                       && x.RequestedStartTime == startTime
                                                       && (preferredNailArtistId == null || x.PreferredNailArtistId == preferredNailArtistId)
                                                       && x.Status == WaitlistStatus.Waiting, true)
                                  .Include(x => x.Customer)
                                        .ThenInclude(c => c.User)
                                  .Include(x => x.Salon)
                                  .Include(x => x.PreferredNailArtist)
                                        .ThenInclude(a => a.Account)
                                  .Include(x => x.WaitlistItems)
                                  .ToListAsync();

            if(!candidates.Any())
            {
                return null;
            }

            var scoredCandidates = candidates.Select(x =>
            {
                int duration = x.EstimatedDuration > 0 ? x.EstimatedDuration : 60;
                double score = 0;
                if (duration > continuousWindowMinutes)
                {
                    score = -9999;  // Quá thời gian rảnh liên tục thực tế => Loai
                }
                else if (duration == freedDurationMinutes)
                {
                    score += 100; // Ưu tiên hoàn toàn nếu trùng khớp
                }
                else if (duration <= continuousWindowMinutes)
                {
                    double fillRatio = (double)duration / continuousWindowMinutes;
                    score += fillRatio * 80;
                }

                score += Math.Max(0, 20 - x.Position); // Ưu tiên những người ở vị trí cao hơn

                return new { Candidate = x, TotalScore = score };
            })
            .Where(x => x.TotalScore > 0)
            .OrderByDescending(x => x.TotalScore)
            .ThenBy(x => x.Candidate.Position)
            .FirstOrDefault();

            return scoredCandidates?.Candidate;
        }
    }
}
