using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IBookingProcedureRepository : IGenericRepository<BookingProcedure>
    {
        Task<List<BookingProcedure>> GetProceduresByBookingItemIdAsync(Guid bookingItemId);
        Task<List<BookingProcedure>> GetProceduresByBookingIdAsync(Guid bookingId, bool trackChanges = false);
        Task<bool> HasAnyInProgressProcedureAsync(Guid artistId);
        Task<List<ProcedureScheduleSegment>> GetArtistBusySegmentsByDateAsync(
          Guid artistId,
          DateTime bookingDate,
          Guid? excludingBookingId = null);
    }
}
