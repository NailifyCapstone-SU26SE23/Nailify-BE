using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IComponentService
    {
        Task<ApiResult<PagedList<ComponentDto>>> GetPagedComponentsAsync(int pageNumber, int pageSize, string? name = null, ComponentType? componentType = null);
        Task<ApiResult<ComponentDto>> GetComponentByIdAsync(int id);
        Task<ApiResult<ComponentDto>> CreateComponentAsync(ComponentCreateRequest request, string? imageUrl = null);
        Task<ApiResult<ComponentDto>> UpdateComponentAsync(ComponentUpdateRequest request);
        Task<ApiResult<bool>> DeleteComponentAsync(int id);
    }
}
