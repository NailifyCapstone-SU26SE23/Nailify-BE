using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistBreakRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistBreakResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailArtistBreakService
    {
        Task<ApiResult<NailArtistBreakResponseDTO>> CreateBreakAsync(NailArtistBreakCreateRequestDTO request);
        Task<ApiResult<NailArtistBreakResponseDTO>> UpdateBreakAsync(Guid breakId, NailArtistBreakUpdateRequestDTO request);
        Task<ApiResult<bool>> DeleteBreakAsync(Guid breakId);
        Task<ApiResult<NailArtistBreakResponseDTO>> ApproveRejectBreakAsync(Guid breakId, ApproveRejectBreakRequest request);
        Task<ApiResult<PagedList<NailArtistBreakResponseDTO>>> GetPagedBreaksAsync(int pageNumber, int pageSize, Guid? artistId = null, DateTime? date = null);
    }
}
