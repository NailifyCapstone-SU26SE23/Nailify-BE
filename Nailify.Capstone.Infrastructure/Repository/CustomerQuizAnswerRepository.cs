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
    public class CustomerQuizAnswerRepository : GenericRepository<CustomerQuizAnswer>, ICustomerQuizAnswerRepository
    {
        public CustomerQuizAnswerRepository(NailifyDbContext context) : base(context)
        {
        }
        public async Task<List<CustomerQuizAnswer>> GetAnswersByCustomerIdAsync(Guid customerId)
        => await FindByCondition(a => a.CustomerId == customerId, trackChanges: true)
                .ToListAsync();
    }
}
