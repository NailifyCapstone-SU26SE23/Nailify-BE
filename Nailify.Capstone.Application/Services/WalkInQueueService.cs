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

            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.NailArtistId);
            if (artist == null || artist.Status != "Active")
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Thợ làm móng không hoạt động hoặc không tồn tại.");
            }

            // Luật phân bổ Walk-in: Kiểm tra sức chứa ConcurrentCapacity của thợ
            int capacity = artist.ConcurrentCapacity;
            var localNow = DateTime.UtcNow.AddHours(7);
            var todayDate = localNow.Date;
            var currentTime = localNow.TimeOfDay;

            // 1. Số khách Booking thợ đang phục vụ thực tế (CheckedIn / InProgress)
            var activeBookings = await _unitOfWork.BookingRepository.CountServingBookingsAsync(request.NailArtistId, localNow);

            // 2. Số khách vãng lai thợ đang phục vụ thực tế (InService)
            var activeWalkIns = await _unitOfWork.WalkInQueueRepository.CountServingWalkInsAsync(request.NailArtistId, localNow);

            int totalServing = activeBookings + activeWalkIns;

            // 3. Số khách Booking hẹn trước sắp đến trong vòng 30 phút (Status == Approved)
            var upcomingThreshold = currentTime.Add(TimeSpan.FromMinutes(30));
            var upcomingBookings = await _unitOfWork.BookingRepository.CountUpcomingBookingsAsync(request.NailArtistId, localNow, currentTime, upcomingThreshold);

            // 4. Kiểm tra luật nghiệp vụ: Nếu vượt quá sức chứa, không cho phân bổ thêm khách vãng lai
            if (totalServing + upcomingBookings >= capacity)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>(
                    $"Thợ {artist.Account.FirstName} {artist.Account.LastName} đã đạt giới hạn phục vụ đồng thời. " +
                    $"(Sức chứa: {capacity}, Đang phục vụ: {totalServing}, Lịch hẹn sắp đến: {upcomingBookings}). " +
                    $"Vui lòng chọn thợ khác hoặc chờ thợ hoàn thành công việc.");
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

        public async Task<ApiResult<WalkInQueueResponseDTO>> PrioritizeQueueEntryAsync(Guid queueId, Guid actorId)
        {
            var walkIn = await _unitOfWork.WalkInQueueRepository.GetByIdAsync(queueId);
            if (walkIn == null)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Không tìm thấy lượt hàng chờ.");
            }
            if (walkIn.Status != QueueStatus.Waiting)
            {
                return new ApiErrorResult<WalkInQueueResponseDTO>("Chỉ có thể ưu tiên khách hàng đang ở trạng thái chờ.");
            }

            var oldPosition = walkIn.QueuePosition;
            if (oldPosition == 1)
            {
                return new ApiSuccessResult<WalkInQueueResponseDTO>(_mapper.Map<WalkInQueueResponseDTO>(walkIn), "Khách hàng đã ở đầu hàng chờ.");
            }

            // Lấy danh sách những người đang chờ khác cùng salon trong ngày hôm nay
            var today = DateTime.UtcNow.Date;
            var waitingList = await _unitOfWork.WalkInQueueRepository.GetActiveWaitingEntriesAsync(walkIn.SalonId, walkIn.AssignedNailArtistId,trackChanges: true);

            // Đẩy lùi vị trí các khách hàng đang đứng trước khách hàng được ưu tiên
            foreach (var item in waitingList)
            {
                if (item.QueuePosition < oldPosition)
                {
                    item.QueuePosition += 1;
                    _unitOfWork.WalkInQueueRepository.Update(item);
                }
            }

            // Đặt khách hàng được chọn lên vị trí đầu tiên
            walkIn.QueuePosition = 1;
            _unitOfWork.WalkInQueueRepository.Update(walkIn);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<WalkInQueueResponseDTO>(walkIn);
            return new ApiSuccessResult<WalkInQueueResponseDTO>(response, "Đã ưu tiên khách hàng lên đầu hàng chờ tại sảnh.");
        }

    }
}
