using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.StaffTransferRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailArtistTransferService
    {
        Task<ApiResult<NailArtistTransferResultDTO>> CreateTransferAsync(CreateNailArtistTransferRequestDTO request, Guid actorId);
        Task<ApiResult<NailArtistTransferResponseDTO>> CancelTransferAsync(Guid transferId);
        Task<ApiResult<PagedList<NailArtistTransferResultDTO>>> GetPagedTransfersAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, NailArtistTransferStatus? status);
        Task<ApiResult<NailArtistTransferResponseDTO>> GetTransferByIdAsync(Guid transferId);
    }
}
