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
        private ICustomerRepository? _customerRepository;
        private ICategoryTypeRepository? _categoryTypeRepository;
        private ICategoryRepository? _categoryRepository;
        private INailDesignRepository? _nailDesignRepository;
        private ISalonOperatingHourRepository? _salonOperatingHourRepository;
        private ISalonRepository? _salonRepository;
        private INailArtistRepository? _nailArtistRepository;
        private IScheduleRepository? _scheduleRepository;
        public UnitOfWork(NailifyDbContext context)
        {
            _context = context;
        }
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public ICustomerRepository CustomerRepository => _customerRepository ??= new CustomerRepository(_context);
        public ICategoryTypeRepository CategoryTypeRepository => _categoryTypeRepository ??= new CategoryTypeRepository(_context);
        public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_context);
        public INailDesignRepository NailDesignRepository => _nailDesignRepository ??= new NailDesignRepository(_context);

        public ISalonOperatingHourRepository SalonOperatingHourRepository => _salonOperatingHourRepository ??= new SalonOperatingHourRepository(_context);

        public ISalonRepository SalonRepository => _salonRepository ??= new SalonRepository(_context);

        public INailArtistRepository NailArtistRepository => _nailArtistRepository ??= new NailArtistRepository(_context);

        public IScheduleRepository ScheduleRepository => _scheduleRepository ??= new ScheduleRepository(_context);

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
