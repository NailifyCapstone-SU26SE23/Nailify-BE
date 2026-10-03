using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IServicesRepository : IGenericRepository<Nailify.Capstone.Domain.Entities.Services>
    {
        Task<List<Domain.Entities.Services>> GetServicesByIdsAsync(IEnumerable<Guid> serviceIds);
    }
}
