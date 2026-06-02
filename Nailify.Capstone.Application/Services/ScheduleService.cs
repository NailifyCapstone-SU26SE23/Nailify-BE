using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ScheduleRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ScheduleResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<ScheduleResponseDTO>> CreateScheduleAsync(ScheduleCreateRequest request)
        {
            // Kiem tra trung lap ca lam viec
            var existingSchedule = await _unitOfWork.ScheduleRepository
                                              .ExistsAsync(x => x.NailArtistId == request.NailArtistId &&
                                              x.WorkDate == request.WorkDate.Date &&
                                              x.ShiftStart < request.ShiftEnd &&
                                              x.ShiftEnd > request.ShiftStart);
            if (existingSchedule)
            {
                return new ApiErrorResult<ScheduleResponseDTO>("Lịch làm việc trùng với ca làm việc hiện có.");
            }

            var schedule = _mapper.Map<Schedule>(request);
            schedule.WorkDate = schedule.WorkDate.Date; // Đảm bảo chỉ lưu ngày, không lưu thời gian
            await _unitOfWork.ScheduleRepository.CreateAsync(schedule);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ScheduleResponseDTO>(schedule);
            return new ApiSuccessResult<ScheduleResponseDTO>(response, "Tạo ca làm việc mới thành công.");
        }

        public async Task<ApiResult<bool>> DeleteScheduleAsync(Guid scheduleId)
        {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                return new ApiErrorResult<bool>("Không tìm thấy ca trực.");
            _unitOfWork.ScheduleRepository.Delete(schedule);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa ca trực thành công.");
        }

        public async Task<ApiResult<PagedList<ScheduleResponseDTO>>> GetPagedSchedulesAsync(int pageNumber, int pageSize, Guid? artistId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var pagedSchedules = await _unitOfWork.ScheduleRepository.GetPagedAsync(
                            pageNumber,
                            pageSize,
                            x =>
                                (!artistId.HasValue || x.NailArtistId == artistId.Value) &&
                                (!startDate.HasValue || x.WorkDate >= startDate.Value) &&
                                (!endDate.HasValue || x.WorkDate <= endDate.Value)
                        );
            var mappedItems = _mapper.Map<List<ScheduleResponseDTO>>(pagedSchedules.Items);
            var response = new PagedList<ScheduleResponseDTO>(mappedItems, pagedSchedules.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<ScheduleResponseDTO>>(response, "Lấy danh sách lịch phân trang thành công.");
        }

        public async Task<ApiResult<IEnumerable<ScheduleResponseDTO>>> GetSchedulesByArtistIdAsync(Guid artistId, DateTime? startDate, DateTime? endDate)
        {
            var schedules = await _unitOfWork.ScheduleRepository.GetSchedulesByArtistIdAsync(artistId, startDate, endDate);
            var response = _mapper.Map<IEnumerable<ScheduleResponseDTO>>(schedules);
            return new ApiSuccessResult<IEnumerable<ScheduleResponseDTO>>(response, "Lấy lịch ca trực thành công.");
        }

        public async Task<ApiResult<ScheduleResponseDTO>> PatchScheduleAsync(Guid scheduleId, SchedulePatchRequest request)
        {
            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                return new ApiErrorResult<ScheduleResponseDTO>("Không tìm thấy ca trực.");
            _mapper.Map(request, schedule);
            if (request.WorkDate.HasValue)
                schedule.WorkDate = request.WorkDate.Value.Date;
            _unitOfWork.ScheduleRepository.Update(schedule);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ScheduleResponseDTO>(schedule);
            return new ApiSuccessResult<ScheduleResponseDTO>(response, "Cập nhật một phần ca trực thành công.");
        }

        public async Task<ApiResult<ScheduleResponseDTO>> UpdateScheduleAsync(Guid scheduleId, ScheduleUpdateRequest request)
        {

            var schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                return new ApiErrorResult<ScheduleResponseDTO>("Không tìm thấy ca trực.");
            _mapper.Map(request, schedule);
            schedule.WorkDate = request.WorkDate.Date;
            _unitOfWork.ScheduleRepository.Update(schedule);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ScheduleResponseDTO>(schedule);
            return new ApiSuccessResult<ScheduleResponseDTO>(response, "Cập nhật ca trực thành công.");
        }
    }
}
