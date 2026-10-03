using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class ServicesRepository : GenericRepository<Domain.Entities.Services>, Application.Interfaces.RepositoryInterfaces.IServicesRepository
    {
        public ServicesRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<Services>> GetServicesByIdsAsync(IEnumerable<Guid> serviceIds)
        {
            var ids = serviceIds.Distinct().ToList();
            if (!ids.Any())
            {
                return new List<Domain.Entities.Services>();
            }

            return await _dbSet.Where(x => ids.Contains(x.ServiceId)).ToListAsync();
        }
    }
}
