using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IPointConversionLogRepository : IGenericRepository<PointConversionLog>
    {
        Task<IEnumerable<PointConversionLog>> GetByCustomerIdAsync(Guid customerId);
    }
}
