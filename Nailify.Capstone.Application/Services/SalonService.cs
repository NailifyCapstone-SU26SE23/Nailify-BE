using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;
using System.Linq.Expressions;
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
    public class SalonService : ISalonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<SalonResponseDTO>> CreateSalonAsync(SalonCreateRequest request, string? imageUrl = null)
        {
            var salon = _mapper.Map<Salon>(request);
            salon.SalonId = Guid.NewGuid();
            salon.Status = "Open";
            salon.ImageUrl = imageUrl;

            // Tự sinh 7 ca làm mặc định
            var defaultHours = new List<SalonOperatingHour>();
            for(int i = 0; i <= 6; i++)
            {
                bool isWeekend = (i == 0 || i == 6);
                defaultHours.Add(new SalonOperatingHour
                {
                    OperatingHourId = Guid.NewGuid(),
                    SalonId = salon.SalonId,
                    DayOfWeek = i,
                    OpenTime = isWeekend ? TimeSpan.FromHours(10) : TimeSpan.FromHours(8),
                    CloseTime = isWeekend ? TimeSpan.FromHours(17) : TimeSpan.FromHours(19),
                    IsClosed = false
                });
            }

            salon.OperatingHours = defaultHours;

            await _unitOfWork.SalonRepository.CreateAsync(salon);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<SalonResponseDTO>(salon);
            return new ApiSuccessResult<SalonResponseDTO>(response, "Tạo mới chi nhánh thành công.");

        }

        public async Task<ApiResult<bool>> DeleteSalonAsync(Guid id)
        {
            var salon = await _unitOfWork.SalonRepository.GetByIdAsync(id);
            if (salon == null)
                return new ApiResult<bool>(false, "Không tìm thấy chi nhánh để xóa.");
            _unitOfWork.SalonRepository.Delete(salon);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa chi nhánh thành công.");
        }

        public async Task<ApiResult<PagedList<SalonResponseDTO>>> GetPagedSalonsAsync(SalonRequestParameters parameters)
        {
            var pagedSalons = await _unitOfWork.SalonRepository.GetPagedSalonsAsync(parameters);

            var mappedItems = _mapper.Map<List<SalonResponseDTO>>(pagedSalons.Items);

            var response = new PagedList<SalonResponseDTO>(
                mappedItems,
                pagedSalons.MetaData.TotalItems,
                pagedSalons.MetaData.CurrentPage,
                pagedSalons.MetaData.PageSize
            );

            return new ApiSuccessResult<PagedList<SalonResponseDTO>>(response, "Lấy danh sách chi nhánh phân trang thành công.");
        }

        public async Task<ApiResult<SalonResponseDTO>> GetSalonByIdAsync(Guid id)
        {
            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(id);
            if (salon == null)
                return new ApiResult<SalonResponseDTO>(false, "Không tìm thấy thông tin chi nhánh.");
            var response = _mapper.Map<SalonResponseDTO>(salon);
            return new ApiSuccessResult<SalonResponseDTO>(response, "Lấy thông tin chi nhánh thành công.");
        }

        public async Task<ApiResult<bool>> UpdateOperatingHoursAsync(Guid salonId, List<SalonOperatingHourUpdateRequest> operatingHours)
        {
            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(salonId);
            if (salon == null)
                return new ApiErrorResult<bool>("Không tìm thấy chi nhánh để cập nhật giờ hoạt động.");

            foreach (var item in operatingHours)
            {
                var existingHour = salon.OperatingHours.FirstOrDefault(oh => oh.DayOfWeek == item.DayOfWeek);
                if (existingHour != null)
                {
                    existingHour.OpenTime = TimeSpan.Parse(item.OpenTime);
                    existingHour.CloseTime = TimeSpan.Parse(item.CloseTime);
                    existingHour.IsClosed = item.IsClosed;
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Cập nhật giờ hoạt động thành công.");
        }

        public async Task<ApiResult<SalonResponseDTO>> UpdateSalonAsync(Guid id, SalonUpdateRequest request, string? imageUrl = null)
        {
            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(id);
            if (salon == null)
                return new ApiResult<SalonResponseDTO>(false, "Không tìm thấy chi nhánh để cập nhật.");
            _mapper.Map(request, salon);
            if (imageUrl != null)
            {
                salon.ImageUrl = imageUrl;
            }
            _unitOfWork.SalonRepository.Update(salon);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<SalonResponseDTO>(salon);
            return new ApiSuccessResult<SalonResponseDTO>(response, "Cập nhật chi nhánh thành công.");
        }

        public async Task<ApiResult<SalonResponseDTO>> PatchSalonAsync(Guid id, SalonPatchRequest request, string? imageUrl = null)
        {
            var salon = await _unitOfWork.SalonRepository.GetSalonWithOperatingHoursAsync(id);
            if (salon == null)
                return new ApiResult<SalonResponseDTO>(false, "Không tìm thấy chi nhánh để cập nhật.");

            if (request.Name != null) salon.Name = request.Name;
            if (request.Address != null) salon.Address = request.Address;
            if (request.Phone != null) salon.Phone = request.Phone;
            if (request.Latitude.HasValue) salon.Latitude = request.Latitude.Value;
            if (request.Longitude.HasValue) salon.Longitude = request.Longitude.Value;
            if (request.Status != null) salon.Status = request.Status;
            if (imageUrl != null) salon.ImageUrl = imageUrl;

            _unitOfWork.SalonRepository.Update(salon);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<SalonResponseDTO>(salon);
            return new ApiSuccessResult<SalonResponseDTO>(response, "Cập nhật một phần chi nhánh thành công.");
        }
    }
}
