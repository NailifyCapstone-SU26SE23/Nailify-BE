using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistBreakRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistBreakResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Domain.Entities;
namespace Nailify.Capstone.Application.Services
{
    public class NailArtistBreakService : INailArtistBreakService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailArtistBreakService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<NailArtistBreakResponseDTO>> ApproveRejectBreakAsync(Guid breakId, ApproveRejectBreakRequest request)
        {
            var artistBreak = await _unitOfWork.NailArtistBreakRepository.GetByIdAsync(breakId);
            if (artistBreak == null)
                return new ApiErrorResult<NailArtistBreakResponseDTO>("Không tìm thấy yêu cầu nghỉ.");

            if (request.Status == ArtistBreakStatus.Pending)
            {
                return new ApiErrorResult<NailArtistBreakResponseDTO>("Trạng thái duyệt không hợp lệ. Vui lòng chọn Approved hoặc Rejected.");
            }

            if (request.Status == ArtistBreakStatus.Rejected)
            {
                if (string.IsNullOrWhiteSpace(request.RejectReason))
                {
                    return new ApiErrorResult<NailArtistBreakResponseDTO>("Vui lòng cung cấp lý do từ chối yêu cầu nghỉ.");
                }
                artistBreak.RejectReason = request.RejectReason;
            }
            else
            {
                artistBreak.RejectReason = null;
            }

            artistBreak.Status = request.Status;
            _unitOfWork.NailArtistBreakRepository.Update(artistBreak);
            await _unitOfWork.SaveChangesAsync();

            var message = request.Status == ArtistBreakStatus.Approved ? "Đã duyệt yêu cầu nghỉ." : "Đã từ chối yêu cầu nghỉ.";
            var response = _mapper.Map<NailArtistBreakResponseDTO>(artistBreak);
            return new ApiSuccessResult<NailArtistBreakResponseDTO>(response, message);
        }

        public async Task<ApiResult<NailArtistBreakResponseDTO>> CreateBreakAsync(NailArtistBreakCreateRequestDTO request)
        {
            var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(request.NailArtistId, request.BreakDate);
            if(schedule == null)
            {
                return new ApiErrorResult<NailArtistBreakResponseDTO>("Thợ nail không có lịch trực trong ngày này để đăng ký nghỉ.");
            }

            var start = TimeSpan.Parse(request.StartTime);
            var end = TimeSpan.Parse(request.EndTime);
            
            if(start < schedule.ShiftStart || end > schedule.ShiftEnd)
            {
                return new ApiErrorResult<NailArtistBreakResponseDTO>($"Thời gian nghỉ phải nằm trong ca làm việc ({schedule.ShiftStart} - {schedule.ShiftEnd}).");
            }

            var isOverlap = await _unitOfWork.NailArtistBreakRepository.ExistsAsync(x => x.NailArtistId == request.NailArtistId
                                                                                    && x.BreakDate.Date == request.BreakDate.Date
                                                                                    && x.Status !=  ArtistBreakStatus.Rejected
                                                                                    && x.StartTime < end && x.EndTime > start);

            if (isOverlap)
            {
                return new ApiErrorResult<NailArtistBreakResponseDTO>("Thời gian nghỉ bị trùng với một yêu cầu nghỉ khác.");
            }
            var artistBreak = _mapper.Map<NailArtistBreak>(request);
            artistBreak.Status = ArtistBreakStatus.Pending;

            await _unitOfWork.NailArtistBreakRepository.CreateAsync(artistBreak);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<NailArtistBreakResponseDTO>(artistBreak);
            return new ApiSuccessResult<NailArtistBreakResponseDTO>(response, "Đăng ký giờ nghỉ thành công, chờ Quản lý duyệt.");
        }

        public async Task<ApiResult<bool>> DeleteBreakAsync(Guid breakId)
        {
            var artistBreak = await _unitOfWork.NailArtistBreakRepository.GetByIdAsync(breakId);
            if (artistBreak == null)
                return new ApiErrorResult<bool>("Không tìm thấy lịch nghỉ.");
            artistBreak.Status = ArtistBreakStatus.Rejected;
            _unitOfWork.NailArtistBreakRepository.Update(artistBreak);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Hủy yêu cầu nghỉ thành công.");
        }

        public async Task<ApiResult<PagedList<NailArtistBreakResponseDTO>>> GetPagedBreaksAsync(int pageNumber, int pageSize, Guid? artistId = null, DateTime? date = null, string? status = null,
          string? orderBy = null)
        {
            var paged = await _unitOfWork.NailArtistBreakRepository.GetPagedAsync(
                       pageNumber,
                       pageSize,
                       x => (!artistId.HasValue || x.NailArtistId == artistId.Value) &&
                            (!date.HasValue || x.BreakDate.Date == date.Value.Date), status, orderBy
                   );
            var mapped = _mapper.Map<List<NailArtistBreakResponseDTO>>(paged.Items);
            var response = new PagedList<NailArtistBreakResponseDTO>(mapped, paged.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<NailArtistBreakResponseDTO>>(response, "Lấy danh sách nghỉ phép thành công.");
        }

        public async Task<ApiResult<NailArtistBreakResponseDTO>> UpdateBreakAsync(Guid breakId, NailArtistBreakUpdateRequestDTO request)
        {
            var artistBreak = await _unitOfWork.NailArtistBreakRepository.GetByIdAsync(breakId);
            if (artistBreak == null)
            {
                return new ApiErrorResult<NailArtistBreakResponseDTO>("Không tìm thấy lịch nghỉ.");
            }
            var start = TimeSpan.Parse(request.StartTime);
            var end = TimeSpan.Parse(request.EndTime);

            var schedule = await _unitOfWork.ScheduleRepository.GetScheduleByArtistAndDateAsync(artistBreak.NailArtistId, artistBreak.BreakDate);
            if (schedule != null && (start < schedule.ShiftStart || end > schedule.ShiftEnd))
            {
                return new ApiErrorResult<NailArtistBreakResponseDTO>($"Thời gian nghỉ phải nằm trong ca làm việc ({schedule.ShiftStart} - {schedule.ShiftEnd}).");
            }

            _mapper.Map(request, artistBreak);
            artistBreak.Status = ArtistBreakStatus.Pending;

            _unitOfWork.NailArtistBreakRepository.Update(artistBreak);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<NailArtistBreakResponseDTO>(artistBreak);
            return new ApiSuccessResult<NailArtistBreakResponseDTO>(response, "Cập nhật lịch nghỉ thành công, chờ Quản lý duyệt lại.");
        }
    }
}
