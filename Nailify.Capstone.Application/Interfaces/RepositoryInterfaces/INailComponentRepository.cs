using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailComponentRepository : IGenericRepository<NailComponent>
    {
        Task<List<NailComponent>> GetAllNailComponentsAsync();
        Task<PagedList<NailComponent>> GetPagedNailComponentsAsync(int pageNumber, int pageSize);
        Task<NailComponent?> GetNailComponentDetailAsync(int nailComponentId);
    }
}
