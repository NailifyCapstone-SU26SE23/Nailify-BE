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
    public class NailProcedureRepository : GenericRepository<NailProcedure>, INailProcedureRepository
    {
        public NailProcedureRepository(NailifyDbContext context) : base(context)
        {
        }
        public async Task<List<NailProcedure>> GetActiveProceduresByVariantIdAsync(int nailVariantId)
        {
            return await FindByCondition(np => np.NailVariantId == nailVariantId && np.Status == "Active")
                .Include(np => np.Procedure)
                .Where(np => np.Procedure.Status == "Active")
                .OrderBy(np => np.StepOrder)
                .ToListAsync();
        }

        public async Task<List<NailProcedure>> GetActiveProceduresByCustomerNailIdAsync(int customerNailId)
        {
            return await FindByCondition(np => np.CustomerNailId == customerNailId && np.Status == "Active")
                .Include(np => np.Procedure)
                .Where(np => np.Procedure.Status == "Active")
                .OrderBy(np => np.StepOrder)
                .ToListAsync();
        }

        public async Task<NailProcedure?> GetNailProcedureWithProcedureAsync(Guid nailProcedureId)
        {
            return await FindByCondition(np => np.NailProcedureId == nailProcedureId)
                .Include(np => np.Procedure)
                .FirstOrDefaultAsync();
        }
    }
}
