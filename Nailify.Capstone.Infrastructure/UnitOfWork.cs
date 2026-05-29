using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Infrastructure.DBContext;
using Nailify.Capstone.Infrastructure.Repository;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NailifyDbContext _context;
        private IUserRepository? _userRepository;

        public UnitOfWork(NailifyDbContext context)
        {
            _context = context;
        }
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
