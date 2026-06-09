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
    }
}
