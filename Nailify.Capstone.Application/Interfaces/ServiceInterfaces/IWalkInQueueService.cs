using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalkInQueueResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IWalkInQueueService
    {
        Task<ApiResult<WalkInQueueResponseDTO>> AddToQueueAsync(Guid actorId, AddToQueueRequestDTO request);
        Task<ApiResult<List<WalkInQueueResponseDTO>>> GetTodayQueueAsync(Guid salonId);
        Task<ApiResult<WalkInQueueResponseDTO>> CallQueueAsync(Guid queueId, Guid actorId);
        Task<ApiResult<WalkInQueueResponseDTO>> AssignArtistAsync(Guid queueId, AssignQueueArtistRequestDTO request, Guid actorId);
        Task<ApiResult<WalkInQueueResponseDTO>> CompleteQueueEntryAsync(Guid queueId, Guid actorId);
        Task<ApiResult<WalkInQueueResponseDTO>> MarkLeftAsync(Guid queueId, Guid actorId);
        Task<ApiResult<WalkInQueueResponseDTO>> PrioritizeQueueEntryAsync(Guid queueId, Guid actorId);
        Task<ApiResult<BookingResponseDTO>> ConvertWalkInToBookingAsync(Guid queueId, Guid actorId);
        Task<int> CalculateEstimatedWaitTimeAsync(Guid salonId, List<BookingItemRequestDTO> requestedItems);
    }
}
