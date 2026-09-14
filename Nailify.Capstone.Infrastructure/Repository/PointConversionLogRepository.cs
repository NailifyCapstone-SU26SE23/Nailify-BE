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
    public class PointConversionLogRepository : GenericRepository<PointConversionLog>, IPointConversionLogRepository
    {
        public PointConversionLogRepository(NailifyDbContext context) : base(context) { }
        public async Task<IEnumerable<PointConversionLog>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _dbSet.Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
