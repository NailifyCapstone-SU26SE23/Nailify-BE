using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.CustomerNailRequestResponseDTO;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.Services
{
    public class CustomerNailService : ICustomerNailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #region Constructor
        public CustomerNailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion Constructor
        #region CRUD Operations
        public async Task<ApiResult<PagedList<CustomerNailDto>>> GetPagedCustomerNailsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, bool? isPublic = null)
        {
            var pagedResult = await _unitOfWork.CustomerNailRepository.GetPagedCustomerNailsAsync(pageNumber, pageSize, userId, name, isPublic);
            var mappedItems = _mapper.Map<List<CustomerNailDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<CustomerNailDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<CustomerNailDto>>(resultPagedList, "Lấy danh sách móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> GetCustomerNailByIdAsync(int id)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            if (customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy móng tùy chỉnh.");
            }

            return new ApiSuccessResult<CustomerNailDto>(_mapper.Map<CustomerNailDto>(customerNail), "Lấy thông tin móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> CreateCustomerNailAsync(CustomerNailCreateRequest request, string? imageUrl = null, Guid? userId = null)
        {
            if (!userId.HasValue || userId.Value == Guid.Empty || await _unitOfWork.UserRepository.GetByIdAsync(userId.Value) == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy người dùng.");
            }

            var customerNail = _mapper.Map<CustomerNail>(request);
            customerNail.UserId = userId.Value;
            customerNail.ImageUrl = imageUrl ?? string.Empty;
            customerNail.CreatedAt = DateTime.UtcNow;
            customerNail.Price = await CalculateCustomerNailPriceAsync(request.NailShapeId, request.NailSurfaceId, null);
            customerNail.Duration = await CalculateCustomerNailDurationAsync(request.NailShapeId, request.NailSurfaceId);
            await _unitOfWork.CustomerNailRepository.CreateAsync(customerNail);
            await _unitOfWork.SaveChangesAsync();

            var createdCustomerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customerNail.CustomerNailId);
            return new ApiSuccessResult<CustomerNailDto>(_mapper.Map<CustomerNailDto>(createdCustomerNail), "Tạo móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> UpdateCustomerNailAsync(int id, CustomerNailUpdateRequest request, string? imageUrl = null)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if (customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy móng tùy chỉnh.");
            }

            var hasChanges = false;

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                customerNail.Name = request.Name;
                hasChanges = true;
            }

            if (request.NailShapeId.HasValue)
            {
                customerNail.NailShapeId = request.NailShapeId;
                hasChanges = true;
            }

            if (request.NailSurfaceId.HasValue)
            {
                customerNail.NailSurfaceId = request.NailSurfaceId;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(request.CustomColor))
            {
                customerNail.CustomColor = request.CustomColor;
                hasChanges = true;
            }

            if (request.IsPublic.HasValue)
            {
                customerNail.IsPublic = request.IsPublic.Value;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                customerNail.ImageUrl = imageUrl;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return new ApiErrorResult<CustomerNailDto>("Khong co thong tin nao de cap nhat.");
            }

            customerNail.Price = await CalculateCustomerNailPriceAsync(customerNail.NailShapeId, customerNail.NailSurfaceId, id);
            customerNail.Duration = await CalculateCustomerNailDurationAsync(customerNail.NailShapeId, customerNail.NailSurfaceId, id);
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();

            var updatedCustomerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            return new ApiSuccessResult<CustomerNailDto>(_mapper.Map<CustomerNailDto>(updatedCustomerNail), "Cập nhật móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<bool>> DeleteCustomerNailAsync(int id)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if (customerNail == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy móng tùy chỉnh.");
            }

            _unitOfWork.CustomerNailRepository.Delete(customerNail);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa móng tùy chỉnh thành công.");
        }
        #endregion CRUD Operations
        #region Additional Operations
        public async Task RecalculateCustomerNailPriceAsync(int customerNailId)
        {
            var customerNailDetail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customerNailId);

            if (customerNailDetail == null)
            {
                return;
            }

            var customerNail = customerNailDetail;
            customerNail.Price = await CalculateCustomerNailPriceAsync(customerNail.NailShapeId, customerNail.NailSurfaceId, customerNailId);
            customerNail.Duration = (customerNailDetail.NailSurface?.Duration ?? 0)
              + customerNailDetail.CustomerNailComponents.Sum(nailComponent => nailComponent.Component?.Duration ?? 0);
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateCustomerNailPriceAsync(int? nailShapeId, int? nailSurfaceId, int? customerNailId)
        {
            var nailSurface = nailSurfaceId.HasValue ? await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId.Value) : null;
            var componentPrice = 0m;

            if (customerNailId.HasValue)
            {
                var customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customerNailId.Value);
                componentPrice = customerNail?.CustomerNailComponents.Sum(component =>
                    ((component.Component?.Price ?? 0m) + (component.CustomerComponent?.Price ?? 0m))
                    * GetFingerPriceMultiplier(component.FingerIndex)) ?? 0m;
            }

            return (nailSurface?.Price ?? 0m) + componentPrice;
        }

        private static int GetFingerPriceMultiplier(int fingerIndex)
        {
            return fingerIndex == -1 ? 5 : 1;
        }

        private async Task<int?> CalculateCustomerNailDurationAsync(int? nailShapeId, int? nailSurfaceId, int? customerNailId = null)
        {
            var nailSurface = nailSurfaceId.HasValue
                ? await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId.Value)
                : null;
            var componentDuration = 0;

            if (customerNailId.HasValue)
            {
                var nailComponent = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customerNailId.Value);
                componentDuration = nailComponent?.CustomerNailComponents.Sum(nailComponent => nailComponent.Component?.Duration ?? 0) ?? 0;
            }

            return (nailSurface?.Duration ?? 0) + componentDuration;
        }

        public async Task<ApiResult<CustomerNailRequestResponseDTO>> SubmitReviewAsync(CustomerNailRequestCreateRequest requestDto, Guid customerId)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(requestDto.CustomerNailId);
            if(customerNail == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(customerNail.UserId != customerId)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Bạn không có quyền gửi mẫu nail này để xem xét.");
            }
            var salon = await _unitOfWork.SalonRepository.GetByIdAsync(requestDto.SalonId);
            if (salon == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy chi nhánh/salon được chọn.");
            }
            var hasPendingRequest = await _unitOfWork.CustomerNailRequestRepository.ExistsAsync(
                                           x => x.CustomerNailId == requestDto.CustomerNailId
                                           && x.SalonId == requestDto.SalonId
                                           && (x.Status == CustomerNailStatus.PendingReview || x.Status == CustomerNailStatus.Assigned)
                                           );
            if (hasPendingRequest) 
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Mẫu móng này đang trong quá trình duyệt tại chi nhánh này.");
            }
            var request = _mapper.Map<CustomerNailRequest>(requestDto);
            await _unitOfWork.CustomerNailRequestRepository.CreateAsync(request);
            await _unitOfWork.SaveChangesAsync();

            var updatedNail = await _unitOfWork.CustomerNailRequestRepository.GetCustomerNailRequestDetailAsync(request.CustomerNailRequestId);
            var response = _mapper.Map<CustomerNailRequestResponseDTO>(updatedNail);
            return new ApiSuccessResult<CustomerNailRequestResponseDTO>(response, "Gửi yêu cầu duyệt báo giá mẫu nail thành công.");
        }

        public async Task<ApiResult<CustomerNailRequestResponseDTO>> AssignReviewerAsync(Guid id, Guid managerUserId, AssignArtistRequestDTO request)
        {
            var nailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(id);
            if(nailRequest == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if (nailRequest.Status != CustomerNailStatus.PendingReview && nailRequest.Status != CustomerNailStatus.Assigned)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Yêu cầu duyệt mẫu móng không ở trạng thái hợp lệ để phân thợ.");
            }
            var manager = await _unitOfWork.UserRepository.GetByIdAsync(managerUserId);
            if (manager == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy tài khoản quản lý.");
            }
           
            if (nailRequest.SalonId != manager.SalonId)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Bạn không có quyền chỉ định thợ cho mẫu móng ở chi nhánh khác.");
            }
            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.StaffArtistId);
            if (artist == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy thợ nail.");
            }
            if (artist.Account?.SalonId != nailRequest.SalonId)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Thợ làm móng được chỉ định không thuộc chi nhánh cần thẩm định mẫu nail.");
            }
            nailRequest.Status = CustomerNailStatus.Assigned;
            nailRequest.ApprovedArtistId = request.StaffArtistId;
            nailRequest.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CustomerNailRequestRepository.Update(nailRequest);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRequestRepository.GetCustomerNailRequestDetailAsync(id);
            var response = _mapper.Map<CustomerNailRequestResponseDTO>(updatedNail);
            return new ApiSuccessResult<CustomerNailRequestResponseDTO>(response, "Chỉ định thợ thẩm định mẫu nail thành công.");
        }

        public async Task<ApiResult<CustomerNailRequestResponseDTO>> ArtistQuoteAsync(Guid id, Guid artistAccountId, ArtistQuoteRequestDTO request)
        {
            var nailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(id);
            if(nailRequest == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(nailRequest.Status != CustomerNailStatus.Assigned)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Mẫu móng không ở trạng thái chờ Thợ Báo Giá.");
            }
            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistByAccountIdAsync(artistAccountId);
            if (artist == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Tài khoản của bạn không được cấu hình là thợ nail hoặc thợ nail không hoạt động.");
            }
            if(nailRequest.ApprovedArtistId != artist.NailArtistId)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Bạn không có quyền báo giá mẫu nail này.");
            }
            if (request.QuotedPrice < 0 || request.QuotedDuration < 0)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Quote price and duration cannot be negative.");
            }

            nailRequest.Price = request.QuotedPrice;
            nailRequest.Duration = request.QuotedDuration;
            nailRequest.Status = CustomerNailStatus.Reviewed;
            nailRequest.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CustomerNailRequestRepository.Update(nailRequest);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRequestRepository.GetCustomerNailRequestDetailAsync(id);
            var response = _mapper.Map<CustomerNailRequestResponseDTO>(updatedNail);
            return new ApiSuccessResult<CustomerNailRequestResponseDTO>(response, "Thợ nail đề xuất báo giá thành công.");
        }

        public async Task<ApiResult<CustomerNailRequestResponseDTO>> ManagerApproveQuoteAsync(Guid id, Guid managerUserId, ManagerApproveQuoteRequestDTO request)
        {
            var nailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(id);
            if(nailRequest == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(nailRequest.Status != CustomerNailStatus.Reviewed)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Mẫu móng không ở trạng thái chờ Quản Lý Duyệt Báo Giá.");
            }
            var manager = await _unitOfWork.UserRepository.GetByIdAsync(managerUserId);
            if (manager == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy tài khoản quản lý.");
            }
            if (nailRequest.SalonId != manager.SalonId)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Bạn không có quyền chốt báo giá mẫu móng của chi nhánh khác.");
            }

            if (request.FinalPrice < 0 || request.FinalDuration < 0)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Final quote price and duration cannot be negative.");
            }

            nailRequest.Price = request.FinalPrice;
            nailRequest.Duration = request.FinalDuration;
            nailRequest.Status = CustomerNailStatus.Quoted;
            nailRequest.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CustomerNailRequestRepository.Update(nailRequest);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRequestRepository.GetCustomerNailRequestDetailAsync(id);
            var response = _mapper.Map<CustomerNailRequestResponseDTO>(updatedNail);
            return new ApiSuccessResult<CustomerNailRequestResponseDTO>(response, "Quản lý chốt giá gửi khách hàng thành công.");
        }

        public async Task<ApiResult<CustomerNailRequestResponseDTO>> ManagerRejectRequestAsync(Guid id, Guid managerUserId, RejectRequestDTO request)
        {
            var nailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(id);
            if(nailRequest == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(nailRequest.Status != CustomerNailStatus.PendingReview && nailRequest.Status != CustomerNailStatus.Assigned)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Yêu cầu duyệt mẫu không ở trạng thái có thể từ chối.");
            }
            var manager = await _unitOfWork.UserRepository.GetByIdAsync(managerUserId);
            if (manager == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy tài khoản quản lý.");
            }
            if (nailRequest.SalonId != manager.SalonId)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Bạn không có quyền từ chối yêu cầu duyệt mẫu móng của chi nhánh khác.");
            }
            nailRequest.Status = CustomerNailStatus.Rejected;
            nailRequest.RejectReason = request.Reason;
            nailRequest.Price = null;
            nailRequest.Duration = null;
            nailRequest.ApprovedArtistId = null;
            nailRequest.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CustomerNailRequestRepository.Update(nailRequest);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRequestRepository.GetCustomerNailRequestDetailAsync(id);
            var response = _mapper.Map<CustomerNailRequestResponseDTO>(updatedNail);
            return new ApiSuccessResult<CustomerNailRequestResponseDTO>(response, "Yêu cầu duyệt mẫu nail đã bị từ chối.");
        }

        public async Task<ApiResult<CustomerNailRequestResponseDTO>> CustomerRespondQuoteAsync(Guid id, Guid customerId, CustomerRespondQuoteRequest request)
        {
            var nailRequest = await _unitOfWork.CustomerNailRequestRepository.GetByIdAsync(id);
            if (nailRequest == null)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Không tìm thấy yêu cầu duyệt.");
            }
            var customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(nailRequest.CustomerNailId);
            if (customerNail == null || customerNail.UserId != customerId)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Bạn không có quyền phản hồi mẫu nail này.");
            }
            if (nailRequest.Status != CustomerNailStatus.Quoted)
            {
                return new ApiErrorResult<CustomerNailRequestResponseDTO>("Mẫu móng không ở trạng thái Chờ Khách Xác Nhận.");
            }
            if (request.IsAccepted)
            {
                nailRequest.Status = CustomerNailStatus.Approved;
                nailRequest.RejectReason = null;
            }
            else
            {
                nailRequest.Status = CustomerNailStatus.Rejected;
                nailRequest.RejectReason = request.RejectReason ?? "Khách hàng từ chối báo giá.";
                nailRequest.Price = null;
                nailRequest.Duration = null;
                nailRequest.ApprovedArtistId = null;
            }
            nailRequest.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CustomerNailRequestRepository.Update(nailRequest);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRequestRepository.GetCustomerNailRequestDetailAsync(id);
            string message = request.IsAccepted ? "Đồng ý báo giá thành công. Bạn đã có thể đặt lịch làm mẫu này." : "Từ chối báo giá thành công.";
            var response = _mapper.Map<CustomerNailRequestResponseDTO>(updatedNail);
            return new ApiSuccessResult<CustomerNailRequestResponseDTO>(response, message);
        }

        public async Task<ApiResult<PagedList<CustomerNailRequestResponseDTO>>> GetPagedCustomerNailRequestsAsync(
            int pageNumber, int pageSize, Guid? salonId = null, CustomerNailStatus? status = null, Guid? customerId = null, Guid? approvedArtistId = null)
        {
            var pagedRequests = await _unitOfWork.CustomerNailRequestRepository.GetPagedCustomerNailRequestsAsync(
                pageNumber, pageSize, salonId, status, customerId, approvedArtistId);

            var mappedItems = _mapper.Map<List<CustomerNailRequestResponseDTO>>(pagedRequests.Items);
            var resultPagedList = new PagedList<CustomerNailRequestResponseDTO>(
                mappedItems, pagedRequests.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<CustomerNailRequestResponseDTO>>(resultPagedList, "Lấy danh sách yêu cầu duyệt mẫu móng thành công.");
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
        #endregion Additional Operations
    }
}
