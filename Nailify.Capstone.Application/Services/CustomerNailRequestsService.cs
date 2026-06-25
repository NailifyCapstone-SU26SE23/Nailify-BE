using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.CustomerNailRequestResponseDTO;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class CustomerNailRequestsService : ICustomerNailRequestsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerNailRequestsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<CustomerNailRequestResponseDTO>>> GetPagedCustomerNailRequestsAsync(
            int pageNumber, 
            int pageSize, 
            Guid? salonId = null, 
            CustomerNailStatus? status = null, 
            Guid? customerId = null, 
            Guid? approvedArtistId = null)
        {
            var pagedRequests = await _unitOfWork.CustomerNailRequestRepository.GetPagedCustomerNailRequestsAsync(
                pageNumber, pageSize, salonId, status, customerId, approvedArtistId);

            var mappedItems = _mapper.Map<List<CustomerNailRequestResponseDTO>>(pagedRequests.Items);
            var resultPagedList = new PagedList<CustomerNailRequestResponseDTO>(
                mappedItems, pagedRequests.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<CustomerNailRequestResponseDTO>>(
                resultPagedList, "Lấy danh sách yêu cầu duyệt mẫu móng thành công.");
        }

        public async Task<ApiResult<CustomerNailRequestResponseDTO>> GetCustomerNailRequestByIdAsync(Guid requestId)
        {
            var nailRequest = await _unitOfWork.CustomerNailRequestRepository.GetCustomerNailRequestDetailAsync(requestId);
            if (nailRequest == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy yêu cầu duyệt mẫu móng.");
            }
            var response = _mapper.Map<CustomerNailRequestResponseDTO>(nailRequest);
            return new ApiSuccessResult<CustomerNailRequestResponseDTO>(response, "Lấy chi tiết yêu cầu duyệt mẫu móng thành công.");
        }
    }
}
