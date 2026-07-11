using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class BookingProcedureRepository : GenericRepository<BookingProcedure>, IBookingProcedureRepository
    {
        public BookingProcedureRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<ProcedureScheduleSegment>> GetArtistBusySegmentsByDateAsync(
                                           Guid artistId,
                                           DateTime bookingDate,
                                           Guid? excludingBookingId = null)
        {
            var date = bookingDate.Date;

            var procedures = await _context.BookingProcedures
                .AsNoTracking()
                .Include(x => x.BookingItem)
                    .ThenInclude(x => x.Booking)
                .Where(x =>
                    x.AssignedArtistId == artistId &&
                    x.EstimatedStartTime.HasValue &&
                    x.BookingItem.Booking.BookingDate.Date == date &&
                    x.BookingItem.Booking.Status != BookingStatus.Cancelled &&
                    x.BookingItem.Booking.Status != BookingStatus.Rejected &&
                    (!excludingBookingId.HasValue || x.BookingItem.BookingId != excludingBookingId.Value))
                .ToListAsync();

            return procedures.Select(x =>
            {
                var busyStart = x.EstimatedStartTime!.Value;

                return new ProcedureScheduleSegment
                {
                    BookingProcedureId = x.BookingProcedureId,
                    BookingItemId = x.BookingItemId,
                    AssignedArtistId = x.AssignedArtistId,
                    StartTime = x.EstimatedStartTime.Value,
                    EndTime = x.EstimatedEndTime ?? x.EstimatedStartTime.Value.Add(TimeSpan.FromMinutes(x.Duration)),
                    ArtistBusyStart = busyStart,
                    ArtistBusyEnd = busyStart.Add(TimeSpan.FromMinutes(x.ActiveDuration)),
                    CanOverlap = x.CanOverlap
                };
            }).ToList();
        }

        public async Task<List<BookingProcedure>> GetProceduresByBookingIdAsync(Guid bookingId, bool trackChanges = false)
            => await FindByCondition(x => x.BookingItem.BookingId == bookingId,trackChanges)
                     .Include(x => x.CompletedBy)
                          .ThenInclude(x =>x.Account)
                     .Include(x => x.AssignedArtist)
                          .ThenInclude(x =>x.Account)
                     .OrderBy(x => x.StepOrder)
                     .ToListAsync();


        public async Task<List<BookingProcedure>> GetProceduresByBookingItemIdAsync(Guid bookingItemId)
        {
            return await FindByCondition(bp => bp.BookingItemId == bookingItemId)
                .Include(bp => bp.CompletedBy)
                    .ThenInclude(na => na.Account)
                .Include(x => x.AssignedArtist)
                    .ThenInclude(na => na.Account)
                .OrderBy(bp => bp.StepOrder)
                .ToListAsync();
        }

        public async Task<bool> HasAnyInProgressProcedureAsync(Guid artistId)
        {
            return await FindByCondition(p => p.AssignedArtistId == artistId 
                                    && p.Status == Nailify.Capstone.Domain.Enums.BookingProcedureStatus.InProgress, false)
                        .AnyAsync();
        }

        public async Task<BookingProcedure?> GetProcedureWithBookingItemAsync(Guid bookingProcedureId)
        {
            return await FindByCondition(x => x.BookingProcedureId == bookingProcedureId, false)
                .Include(x => x.BookingItem)
                    .ThenInclude(x => x.Booking)
                .FirstOrDefaultAsync();
        }
    }
}
