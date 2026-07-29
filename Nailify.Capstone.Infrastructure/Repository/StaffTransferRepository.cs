using Nailify.Capstone.Application.Common;
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
    public class StaffTransferRepository : GenericRepository<StaffTransfer>, IStaffTransferRepository
    {
        public StaffTransferRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<StaffTransfer?> GetActiveTransferByArtistAndDateAsync(Guid artistId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<StaffTransfer>> GetPagedTransferAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, StaffTransferStatus? status)
        {
            throw new NotImplementedException();
        }

        public Task<List<Guid>> GetTransferredOutArtistIdsAsync(Guid salonId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<List<StaffTransfer>> GetTransfersIntoSalonByDateAsync(Guid salonId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasOverlappingTransferAsync(Guid artistId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}
