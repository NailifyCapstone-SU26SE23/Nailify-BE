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
    public class ProcedureRepository : GenericRepository<Procedure>, IProcedureRepository
    {
        public ProcedureRepository(NailifyDbContext context) : base(context)
        {
        }
        public async Task<List<Procedure>> GetActiveProceduresAsync()
        {
            return await FindByCondition(p => p.Status == "Active").ToListAsync();
        }
    }
}