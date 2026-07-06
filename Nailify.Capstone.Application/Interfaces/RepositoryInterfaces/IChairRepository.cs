using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IChairRepository : IGenericRepository<Chair>
    {
        Task<Chair?> GetChairWithSalonAsync(Guid chairId);
        Task<IEnumerable<Chair>> GetActiveChairsBySalonAsync(Guid salonId);
        Task<PagedList<Chair>> GetPagedChairsBySalonAsync(Guid salonId, int pageNumber, int pageSize);
    }
}
