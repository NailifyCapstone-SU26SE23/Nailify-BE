using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs;
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
    public class SalonOffDateService : ISalonOffDateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalonOffDateService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<SalonOffDateResponseDTO>> AddSalonOffDateAsync(Guid salonId, CreateSalonOffDateRequestDTO request)
        {
            var salon = await _unitOfWork.SalonRepository.GetByIdAsync(salonId);
            if (salon == null)
            {
                return new ApiErrorResult<SalonOffDateResponseDTO>("Không tìm thấy chi nhánh.");
            }
            var startDate = (request.StartDate.Kind == DateTimeKind.Utc ? request.StartDate.AddHours(7) : request.StartDate).Date;
            var endDate = request.EndDate.HasValue ? (request.EndDate.Value.Kind == DateTimeKind.Utc ? request.EndDate.Value.AddHours(7) : request.EndDate.Value).Date : startDate;
            if (endDate < startDate)
            {
                return new ApiErrorResult<SalonOffDateResponseDTO>("Ngày kết thúc không thể nhỏ hơn ngày bắt đầu.");
            }
            var exists = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x =>
                                                                                 x.SalonId == salonId 
                                                                                 && x.StartDate.Date <= endDate 
                                                                                 && x.EndDate.Date >= startDate);
            if (exists)
            {
                return new ApiErrorResult<SalonOffDateResponseDTO>("Khoảng ngày nghỉ này trùng lặp với ngày nghỉ đã có của chi nhánh.");
            }
            var offDate = _mapper.Map<SalonOffDate>(request);
            offDate.SalonId = salonId;
            offDate.StartDate = startDate;
            offDate.EndDate = endDate;
            await _unitOfWork.SalonOffDateRepository.CreateAsync(offDate);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<SalonOffDateResponseDTO>(offDate);
            return new ApiSuccessResult<SalonOffDateResponseDTO>(response, "Đăng ký ngày nghỉ cho chi nhánh thành công.");
        }

        public async Task<ApiResult<bool>> DeleteSalonOffDateAsync(Guid salonOffDateId)
        {
            var offDate = await _unitOfWork.SalonOffDateRepository.GetByIdAsync(salonOffDateId);
            if (offDate == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy ngày nghỉ chi nhánh.");
            }
            _unitOfWork.SalonOffDateRepository.Delete(offDate);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa ngày nghỉ chi nhánh thành công.");
        }

        public async Task<ApiResult<List<SalonOffDateResponseDTO>>> GetSalonOffDatesAsync(Guid salonId)
        {
            var salon = await _unitOfWork.SalonRepository.GetByIdAsync(salonId);
            if (salon == null)
            {
                return new ApiErrorResult<List<SalonOffDateResponseDTO>>("Không tìm thấy chi nhánh.");
            }
            var dates = await _unitOfWork.SalonOffDateRepository.GetSalonOffDatesAsync(salonId);
            var response = _mapper.Map<List<SalonOffDateResponseDTO>>(dates);
            return new ApiSuccessResult<List<SalonOffDateResponseDTO>>(response, "Lấy danh sách ngày nghỉ chi nhánh thành công.");
        }

        public async Task<ApiResult<SalonOffDateResponseDTO>> UpdateSalonOffDateAsync(Guid salonOffDateId, UpdateSalonOffDateRequestDTO request)
        {
            var offDate = await _unitOfWork.SalonOffDateRepository.GetByIdAsync(salonOffDateId);
            if (offDate == null)
            {
                return new ApiErrorResult<SalonOffDateResponseDTO>("Không tìm thấy ngày nghỉ để cập nhật.");
            }
            var targetStartDate = request.StartDate?.Date ?? offDate.StartDate;
            var targetEndDate = request.EndDate?.Date ?? offDate.EndDate;
            if (request.StartDate.HasValue && !request.EndDate.HasValue && targetStartDate > offDate.EndDate)
            {
                targetEndDate = targetStartDate;
            }
            if (request.EndDate.HasValue && !request.StartDate.HasValue && targetEndDate < offDate.StartDate)
            {
                targetStartDate = targetEndDate;
            }
            if (targetEndDate < targetStartDate)
            {
                return new ApiErrorResult<SalonOffDateResponseDTO>("Ngày kết thúc không thể nhỏ hơn ngày bắt đầu.");
            }
            // Kiểm tra trùng lặp với các ngày nghỉ khác ngoài chính nó
            var exists = await _unitOfWork.SalonOffDateRepository.ExistsAsync(x =>
                                                                                 x.SalonId == offDate.SalonId 
                                                                                 && x.SalonOffDateId != salonOffDateId 
                                                                                 && x.StartDate.Date <= targetEndDate 
                                                                                 && x.EndDate.Date >= targetStartDate);
            if (exists)
            {
                return new ApiErrorResult<SalonOffDateResponseDTO>("Cập nhật thất bại vì khoảng ngày nghỉ mới trùng lặp với ngày nghỉ đã có.");
            }
            _mapper.Map(request, offDate);
            offDate.StartDate = targetStartDate;
            offDate.EndDate = targetEndDate;
            _unitOfWork.SalonOffDateRepository.Update(offDate);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<SalonOffDateResponseDTO>(offDate);
            return new ApiSuccessResult<SalonOffDateResponseDTO>(response, "Cập nhật ngày nghỉ chi nhánh thành công.");
        }
    }
}
