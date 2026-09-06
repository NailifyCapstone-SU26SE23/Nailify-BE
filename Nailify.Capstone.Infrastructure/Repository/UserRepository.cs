using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await FindByCondition(u => u.Email.ToLower() == email.Trim().ToLower() && u.Status == "Active").FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByPhoneAsync(string phone)
        {
            return await FindByCondition(u => u.Phone == phone.Trim() && u.Status == "Active").FirstOrDefaultAsync();
        }
    }
}
