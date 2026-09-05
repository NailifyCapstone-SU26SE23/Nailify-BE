using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ServiceRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ServiceResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServicesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<ServiceResponseDTO>> CreateServiceAsync(ServiceCreateRequestDTO request)
        {
            var service = _mapper.Map<Nailify.Capstone.Domain.Entities.Services>(request);
            await _unitOfWork.ServicesRepository.CreateAsync(service);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ServiceResponseDTO>(service);
            return new ApiSuccessResult<ServiceResponseDTO>(response, "Tạo dịch vụ thành công.");
        }

        public async Task<ApiResult<bool>> DeleteServiceAsync(Guid serviceId)
        {
            var service = await _unitOfWork.ServicesRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy dịch vụ cần xóa.");
            }

            _unitOfWork.ServicesRepository.Delete(service);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa dịch vụ thành công.");
        }

        public async Task<ApiResult<PagedList<ServiceResponseDTO>>> GetPagedServicesAsync(int pageNumber, int pageSize, string? searchName = null, string? status = null,
          string? orderBy = null)
        {
            Expression<Func<Nailify.Capstone.Domain.Entities.Services, bool>> predicate = null;
            if(!string.IsNullOrEmpty(searchName))
            {
                predicate = s => s.Name.ToLower().Contains(searchName.Trim().ToLower());
            }

            var pagedServices = await _unitOfWork.ServicesRepository.GetPagedAsync(pageNumber, pageSize, predicate, status, orderBy);

            var mapItems = _mapper.Map<List<ServiceResponseDTO>>(pagedServices.Items);
            var response = new PagedList<ServiceResponseDTO>(mapItems, pagedServices.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<ServiceResponseDTO>>(response, "Lấy danh sách dịch vụ thành công.");
        }

        public async Task<ApiResult<ServiceResponseDTO>> GetServiceByIdAsync(Guid serviceId)
        {
            var service = await _unitOfWork.ServicesRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                return new ApiErrorResult<ServiceResponseDTO>("Không tìm thấy dịch vụ.");
            }
            var response = _mapper.Map<ServiceResponseDTO>(service);
            return new ApiSuccessResult<ServiceResponseDTO>(response, "Lấy chi tiết dịch vụ thành công.");
        }

        public async Task<ApiResult<ServiceResponseDTO>> UpdateServiceAsync(Guid serviceId, ServiceUpdateRequestDTO request)
        {
            var service = await _unitOfWork.ServicesRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                return new ApiErrorResult<ServiceResponseDTO>("Không tìm thấy dịch vụ cần cập nhật.");
            }
            _mapper.Map(request, service);
            _unitOfWork.ServicesRepository.Update(service);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ServiceResponseDTO>(service);
            return new ApiSuccessResult<ServiceResponseDTO>(response, "Cập nhật dịch vụ thành công.");
        }
    }
}
