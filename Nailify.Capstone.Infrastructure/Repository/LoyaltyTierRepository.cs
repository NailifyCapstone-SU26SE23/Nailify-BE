using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class LoyaltyTierRepository : GenericRepository<LoyaltyTier>, ILoyaltyTierRepository
    {
        public LoyaltyTierRepository(NailifyDbContext context) : base(context) { }
    }
}
