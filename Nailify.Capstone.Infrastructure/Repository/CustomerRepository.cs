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
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(NailifyDbContext context) : base(context)
        {

        }
    }
}
