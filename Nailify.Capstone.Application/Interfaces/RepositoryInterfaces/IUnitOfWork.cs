using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        ICategoryTypeRepository CategoryTypeRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        INailDesignRepository NailDesignRepository { get; }
        ISalonOperatingHourRepository SalonOperatingHourRepository { get; }
        ISalonRepository SalonRepository { get; }
        INailArtistRepository NailArtistRepository { get; }
        IScheduleRepository ScheduleRepository { get; }
        IComponentRepository ComponentRepository { get; }
        INailShapeRepository NailShapeRepository { get; }
        INailSurfaceRepository NailSurfaceRepository { get; }
        INailVariantRepository NailVariantRepository { get; }
        INailComponentRepository NailComponentRepository { get; }
        ICustomerComponentRepository CustomerComponentRepository { get; }
        ICustomerNailRepository CustomerNailRepository { get; }
        ICustomerNailComponentRepository CustomerNailComponentRepository { get; }
        ISkillTypeRepository SkillTypeRepository { get; }
        INailArtistSkillRepository NailArtistSkillRepository { get; }
        INailRequiredSkillRepository NailRequiredSkillRepository { get; }
        IBookingRepository BookingRepository { get; }
        IBookingItemRepository BookingItemRepository { get; }
        IBookingHistoryRepository BookingHistoryRepository { get; }
        IServicesRepository ServicesRepository { get; }
        IProcedureRepository ProcedureRepository { get; }
        INailProcedureRepository NailProcedureRepository { get; }
        IBookingProcedureRepository BookingProcedureRepository { get; }
        IFavoriteNailRepository FavoriteNailRepository { get; }
        ILoyaltyTierRepository LoyaltyTierRepository { get; }
        ILoyaltyTransactionRepository LoyaltyTransactionRepository { get; }
        ICustomerNailRequestRepository CustomerNailRequestRepository { get; }
        IBookingRatingRepository BookingRatingRepository { get; }
        IPromotionRepository PromotionRepository { get; }
        IBookingDiscountRepository BookingDiscountRepository { get; }
        IUserPromotionUsageRepository UserPromotionUsageRepository { get; }
        IBookingWaitlistRepository BookingWaitlistRepository { get; }
        IWalkInQueueRepository WalkInQueueRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IChairRepository ChairRepository { get; }
        INailArtistBreakRepository NailArtistBreakRepository { get; }
        IQuizQuestionRepository QuizQuestionRepository { get; }
        IQuizOptionRepository QuizOptionRepository { get; }
        ICustomerQuizAnswerRepository CustomerQuizAnswerRepository { get; }
        ISalonOffDateRepository SalonOffDateRepository { get; }


        // Quản lý Transaction
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}
