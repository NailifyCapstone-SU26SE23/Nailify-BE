using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailArtistService
    {
        Task<ApiResult<NailArtistResponseDTO>> GetNailArtistByIdAsync(Guid artistId);
        Task<ApiResult<PagedList<NailArtistResponseDTO>>> GetPagedNailArtistsAsync(int pageNumber, int pageSize, Guid? salonId = null);
        Task<ApiResult<NailArtistResponseDTO>> UpdateNailArtistAsync(Guid artistId, NailArtistUpdateRequest request);
        Task<ApiResult<NailArtistResponseDTO>> PatchNailArtistAsync(Guid artistId, NailArtistPatchRequest request);
        Task<ApiResult<bool>> DeleteNailArtistAsync(Guid artistId);
    }
}
