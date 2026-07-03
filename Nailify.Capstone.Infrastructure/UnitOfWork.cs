using Microsoft.EntityFrameworkCore.Storage;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Infrastructure.DBContext;
using Nailify.Capstone.Infrastructure.Repository;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;
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
        private IComponentRepository? _componentRepository;
        private INailShapeRepository? _nailShapeRepository;
        private INailSurfaceRepository? _nailSurfaceRepository;
        private INailVariantRepository? _nailVariantRepository;
        private INailComponentRepository? _nailComponentRepository;
        private ICustomerComponentRepository? _customerComponentRepository;
        private ICustomerNailRepository? _customerNailRepository;
        private ICustomerNailComponentRepository? _customerNailComponentRepository;
        private ISkillTypeRepository? _skillTypeRepository;
        private INailArtistSkillRepository? _nailArtistSkillRepository;
        private INailRequiredSkillRepository? _nailRequiredSkillRepository;
        private IBookingRepository? _bookingRepository;
        private IBookingItemRepository? _bookingItemRepository;
        private IBookingHistoryRepository? _bookingHistoryRepository;
        private IServicesRepository? _servicesRepository;
        private IProcedureRepository? _procedureRepository;
        private INailProcedureRepository? _nailProcedureRepository;
        private IBookingProcedureRepository? _bookingProcedureRepository;
        private IFavoriteNailRepository? _favoriteNailRepository;
        private ILoyaltyTierRepository? _loyaltyTierRepository;
        private ILoyaltyTransactionRepository? _loyaltyTransactionRepository;
        private ICustomerNailRequestRepository? _customerNailRequestRepository;
        private IBookingRatingRepository? _bookingRatingRepository;
        private IPromotionRepository? _promotionRepository;
        private IBookingDiscountRepository? _bookingDiscountRepository;
        private IUserPromotionUsageRepository? _userPromotionUsageRepository;
        private IBookingWaitlistRepository? _bookingWaitlistRepository;
        private IWalkInQueueRepository? _walkInQueueRepository;

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
        public IComponentRepository ComponentRepository => _componentRepository ??= new ComponentRepository(_context);
        public INailShapeRepository NailShapeRepository => _nailShapeRepository ??= new NailShapeRepository(_context);
        public INailSurfaceRepository NailSurfaceRepository => _nailSurfaceRepository ??= new NailSurfaceRepository(_context);
        public INailVariantRepository NailVariantRepository => _nailVariantRepository ??= new NailVariantRepository(_context);
        public INailComponentRepository NailComponentRepository => _nailComponentRepository ??= new NailComponentRepository(_context);
        public ICustomerComponentRepository CustomerComponentRepository => _customerComponentRepository ??= new CustomerComponentRepository(_context);
        public ICustomerNailRepository CustomerNailRepository => _customerNailRepository ??= new CustomerNailRepository(_context);
        public ICustomerNailComponentRepository CustomerNailComponentRepository => _customerNailComponentRepository ??= new CustomerNailComponentRepository(_context);

        public ISkillTypeRepository SkillTypeRepository => _skillTypeRepository ??= new SkillTypeRepository(_context);

        public INailArtistSkillRepository NailArtistSkillRepository => _nailArtistSkillRepository ??= new NailArtistSkillRepository(_context);

        public INailRequiredSkillRepository NailRequiredSkillRepository => _nailRequiredSkillRepository ??= new NailRequiredSkillRepository(_context);

        public IBookingRepository BookingRepository => _bookingRepository ??= new BookingRepository(_context);

        public IBookingItemRepository BookingItemRepository => _bookingItemRepository ??= new BookingItemRepository(_context);

        public IBookingHistoryRepository BookingHistoryRepository => _bookingHistoryRepository ??= new BookingHistoryRepository(_context);

        public IServicesRepository ServicesRepository => _servicesRepository ??= new ServicesRepository(_context);
        public IProcedureRepository ProcedureRepository => _procedureRepository ??= new ProcedureRepository(_context);
        public INailProcedureRepository NailProcedureRepository => _nailProcedureRepository ??= new NailProcedureRepository(_context);
        public IBookingProcedureRepository BookingProcedureRepository => _bookingProcedureRepository ??= new BookingProcedureRepository(_context);
        public IFavoriteNailRepository FavoriteNailRepository => _favoriteNailRepository ??= new FavoriteNailRepository(_context);
        public ILoyaltyTierRepository LoyaltyTierRepository => _loyaltyTierRepository ??= new LoyaltyTierRepository(_context);
        public ILoyaltyTransactionRepository LoyaltyTransactionRepository => _loyaltyTransactionRepository ??= new LoyaltyTransactionRepository(_context);

        public ICustomerNailRequestRepository CustomerNailRequestRepository => _customerNailRequestRepository ??= new CustomerNailRequestRepository(_context);
        public IBookingRatingRepository BookingRatingRepository => _bookingRatingRepository ??= new BookingRatingRepository(_context);
        public IPromotionRepository PromotionRepository => _promotionRepository ??= new PromotionRepository(_context);
        public IBookingDiscountRepository BookingDiscountRepository => _bookingDiscountRepository ??= new BookingDiscountRepository(_context);
        public IUserPromotionUsageRepository UserPromotionUsageRepository => _userPromotionUsageRepository ??= new UserPromotionUsageRepository(_context);

        public IBookingWaitlistRepository BookingWaitlistRepository => _bookingWaitlistRepository ??= new BookingWaitlistRepository(_context);

        public IWalkInQueueRepository WalkInQueueRepository => _walkInQueueRepository ??= new WalkInQueueRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
