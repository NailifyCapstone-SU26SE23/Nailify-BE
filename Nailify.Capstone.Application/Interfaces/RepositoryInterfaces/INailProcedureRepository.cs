using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailProcedureRepository : IGenericRepository<NailProcedure>
    {
        Task<List<NailProcedure>> GetActiveProceduresByVariantIdAsync(int nailVariantId);
        Task<List<NailProcedure>> GetActiveProceduresByCustomerNailIdAsync(int customerNailId);
        Task<NailProcedure?> GetNailProcedureWithProcedureAsync(Guid nailProcedureId);
    }

}
