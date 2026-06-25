using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
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

        public async Task<List<BookingProcedure>> GetProceduresByBookingIdAsync(Guid bookingId)
            => await FindByCondition(x => x.BookingItem.BookingId == bookingId)
                     .Include(x => x.CompletedBy)
                          .ThenInclude(x =>x.Account)
                     .OrderBy(x => x.StepOrder)
                     .ToListAsync();

        public async Task<List<BookingProcedure>> GetProceduresByBookingItemIdAsync(Guid bookingItemId)
        {
            return await FindByCondition(bp => bp.BookingItemId == bookingItemId)
                .Include(bp => bp.CompletedBy)
                .ThenInclude(na => na.Account)
                .OrderBy(bp => bp.StepOrder)
                .ToListAsync();
        }
    }
}
