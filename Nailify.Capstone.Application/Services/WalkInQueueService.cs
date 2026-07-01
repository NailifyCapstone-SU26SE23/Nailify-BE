using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalkInQueueResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class WalkInQueueService : IWalkInQueueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WalkInQueueService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> AddToQueueAsync(Guid actorId, AddToQueueRequestDTO request)
        {
            var nextPost = await _unitOfWork.WalkInQueueRepository.GetNextPositionAsync(request.SalonId);
            var queue = _mapper.Map<WalkInQueue>(request);
            queue.QueuePosition = nextPost;
            queue.Status = QueueStatus.Waiting;
            queue.ArrivalTime = DateTime.UtcNow;

            await _unitOfWork.WalkInQueueRepository.CreateAsync(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã thêm khách vào hàng chờ vãng lai thành công.");
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> AssignArtistAsync(Guid queueId, AssignQueueArtistRequestDTO request, Guid actorId)
        {
            var queue = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if (queue == null)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy bản ghi hàng chờ.");
            }
            var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(request.NailArtistId);
            if (artist == null || artist.Status != "Active")
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Thợ làm móng không hoạt động hoặc không tồn tại.");
            }
            queue.AssignedNailArtistId = request.NailArtistId;
            _unitOfWork.WalkInQueueRepository.Update(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Phân bổ thợ nail thành công.");
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> CallQueueAsync(Guid queueId, Guid actorId)
        {
            var queue = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if(queue == null)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy khách hàng trong hàng chờ.");
            }
            queue.Status = QueueStatus.Called;
            queue.CalledTime = DateTime.UtcNow;
            _unitOfWork.WalkInQueueRepository.Update(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã gọi khách lên quầy chuẩn bị thực hiện.");
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> CompleteQueueEntryAsync(Guid queueId, Guid actorId)
        {
            var queue = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if (queue == null)
            {     
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy bản ghi hàng chờ.");
            }
            queue.Status = QueueStatus.Done;
            _unitOfWork.WalkInQueueRepository.Update(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã hoàn thành lượt chờ của khách (chuyển sang dịch vụ/booking).");
        }

        public async Task<ApiResult<List<WalkInQueueResponseDTO>>> GetTodayQueueAsync(Guid salonId)
        {
            var queueList = await _unitOfWork.WalkInQueueRepository.GetTodayQueueAsync(salonId);
            var response = _mapper.Map<List<WalkInQueueResponseDTO>>(queueList);
            return new ApiSuccessResult<List<WalkInQueueResponseDTO>>(response, "Lấy danh sách hàng chờ hôm nay thành công.");
        }

        public async Task<ApiResult<WalkInQueueResponseDTO>> MarkLeftAsync(Guid queueId, Guid actorId)
        {
            var queue = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if (queue == null)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy khách hàng trong hàng chờ.");
            }
            queue.Status = QueueStatus.Left;
            _unitOfWork.WalkInQueueRepository.Update(queue);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<WalkInQueueResponseDTO>(queue);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã đánh dấu khách hàng rời hàng chờ (vắng mặt/không làm).");
        }
    }
}
