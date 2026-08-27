using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailCategoryRepository : IGenericRepository<NailCategory>
    {
        Task<List<NailCategory>> GetByNailDesignIdAsync(int nailDesignId);
    }
}
