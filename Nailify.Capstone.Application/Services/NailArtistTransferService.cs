using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.StaffTransferRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class NailArtistTransferService : INailArtistTransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INailArtistEmergencyService _emergencyService;
        private readonly INotificationService _notificationService;

        public NailArtistTransferService(IUnitOfWork unitOfWork, IMapper mapper, INailArtistEmergencyService emergencyService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emergencyService = emergencyService;
            _notificationService = notificationService;
        }

        public async Task<ApiResult<NailArtistTransferResponseDTO>> CancelTransferAsync(Guid transferId)
        {
            var transfer = await _unitOfWork.NailArtistTransferRepository.GetByIdAsync(transferId);
            if(transfer == null)
            {
                return new ApiErrorResult<NailArtistTransferResponseDTO>("Không tìm thấy lịch điều chuyển.");
            }
            if(transfer.Status != NailArtistTransferStatus.Scheduled)
            {
                return new ApiErrorResult<NailArtistTransferResponseDTO>("Chỉ có thể hủy lịch điều chuyển đang ở trạng thái Scheduled.");
            }
            var today = DateTime.UtcNow.AddHours(7).Date;
            if (transfer.StartDate > today)
            {
                transfer.Status = NailArtistTransferStatus.Cancelled;
            }
            else
            {
                transfer.EndDate = today.AddDays(-1);
                transfer.Status = transfer.EndDate < transfer.StartDate
                    ? NailArtistTransferStatus.Cancelled
                    : NailArtistTransferStatus.Completed;
            }
            transfer.UpdatedAt = DateTime.UtcNow.AddHours(7);
            _unitOfWork.NailArtistTransferRepository.Update(transfer);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<NailArtistTransferResponseDTO>(_mapper.Map<NailArtistTransferResponseDTO>(transfer), "Hủy/kết thúc điều chuyển thành công.");
        }

        public Task<ApiResult<NailArtistTransferResultDTO>> CreateTransferAsync(CreateNailArtistTransferRequestDTO request, Guid actorId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<PagedList<NailArtistTransferResultDTO>>> GetPagedTransfersAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, NailArtistTransferStatus? status)
        {
            var paged = await _unitOfWork.NailArtistTransferRepository.GetPagedTransferAsync(pageNumber, pageSize, salonId, artistId, status);
            var dtos = _mapper.Map<List<NailArtistTransferResultDTO>>(paged);
            var result = new PagedList<NailArtistTransferResultDTO>(dtos, paged.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<NailArtistTransferResultDTO>>(result, "Lấy danh sách điều chuyển thành công.");
        }

        public Task<ApiResult<NailArtistTransferResponseDTO>> GetTransferByIdAsync(Guid transferId)
        {
            throw new NotImplementedException();
        }
    }
}
