using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly NailifyDbContext _context;

        public UserRepository(NailifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower() && u.Status == "Active");
        }
       
    }
}
