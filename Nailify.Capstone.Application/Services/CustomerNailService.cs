using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
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
        public async Task<ApiResult<PagedList<CustomerNailDto>>> GetPagedCustomerNailsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, bool? isPublic = null, Guid? salonId = null, CustomerNailStatus? status = null)
        {
            var pagedResult = await _unitOfWork.CustomerNailRepository.GetPagedCustomerNailsAsync(pageNumber, pageSize, userId, name, isPublic,salonId,status);
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

            var validationError = await ValidateReferencesAsync(request.NailShapeId, request.NailSurfaceId);
            if (validationError != null)
            {
                return new ApiErrorResult<CustomerNailDto>(validationError);
            }

            _mapper.Map(request, customerNail);
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                customerNail.ImageUrl = imageUrl;
            }

            customerNail.Price = await CalculateCustomerNailPriceAsync(request.NailShapeId, request.NailSurfaceId, id);
            customerNail.Duration = await CalculateCustomerNailDurationAsync(request.NailShapeId, request.NailSurfaceId, id);
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

            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(customerNailId);
            if (customerNail == null || customerNailDetail == null)
            {
                return;
            }

            customerNail.Price = await CalculateCustomerNailPriceAsync(customerNail.NailShapeId, customerNail.NailSurfaceId, customerNailId);
            customerNail.Duration = (customerNailDetail.NailShape?.Duration ?? 0)
              + (customerNailDetail.NailSurface?.Duration ?? 0)
              + customerNailDetail.CustomerNailComponents.Sum(nailComponent => nailComponent.Component?.Duration ?? 0);
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateCustomerNailPriceAsync(int? nailShapeId, int? nailSurfaceId, int? customerNailId)
        {
            var nailShape = nailShapeId.HasValue ? await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId.Value) : null;
            var nailSurface = nailSurfaceId.HasValue ? await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId.Value) : null;
            var componentPrice = 0m;

            if (customerNailId.HasValue)
            {
                var customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customerNailId.Value);
                componentPrice = customerNail?.CustomerNailComponents.Sum(component =>
                    (component.Component?.Price ?? 0m) + (component.CustomerComponent?.Price ?? 0m)) ?? 0m;
            }

            return (nailShape?.Price ?? 0m) + (nailSurface?.Price ?? 0m) + componentPrice;
        }

        private async Task<int?> CalculateCustomerNailDurationAsync(int? nailShapeId, int? nailSurfaceId, int? customerNailId = null)
        {
            var nailShape = nailShapeId.HasValue
                ? await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId.Value)
                : null;
            var nailSurface = nailSurfaceId.HasValue
                ? await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId.Value)
                : null;
            var componentDuration = 0;

            if (customerNailId.HasValue)
            {
                var nailComponent = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customerNailId.Value);
                componentDuration = nailComponent?.CustomerNailComponents.Sum(nailComponent => nailComponent.Component?.Duration ?? 0) ?? 0;
            }

            return (nailShape?.Duration ?? 0) + (nailSurface?.Duration ?? 0) + componentDuration;
        }

        private async Task<string?> ValidateReferencesAsync(int? nailShapeId, int? nailSurfaceId)
        {
            if (!nailShapeId.HasValue || await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId.Value) == null)
            {
                return "Không tìm thấy dáng móng.";
            }

            if (nailSurfaceId.HasValue && await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId.Value) == null)
            {
                return "Không tìm thấy bề mặt móng.";
            }

            return null;
        }

        public async Task<ApiResult<CustomerNailDto>> SubmitReviewAsync(int id, Guid customerId, Guid salonId)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if(customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(customerNail.UserId != customerId)
            {
                return new ApiErrorResult<CustomerNailDto>("Bạn không có quyền gửi mẫu nail này để xem xét.");
            }
            if(customerNail.Status != CustomerNailStatus.Draft)
            {
                return new ApiErrorResult<CustomerNailDto>($"Không thể gửi yêu cầu duyệt mẫu khi mẫu móng ở trạng thái '{customerNail.Status}'.");
            }
            var salon = await _unitOfWork.SalonRepository.GetByIdAsync(salonId);
            if (salon == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy chi nhánh/salon được chọn.");
            }
            customerNail.SalonId = salonId;
            customerNail.Status = CustomerNailStatus.PendingReview;
            customerNail.RejectReason = null;
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();

            var updatedNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            var response = _mapper.Map<CustomerNailDto>(updatedNail);
            return new ApiSuccessResult<CustomerNailDto>(response, "Gửi yêu cầu duyệt báo giá mẫu nail thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> AssignReviewerAsync(int id, Guid managerUserId, AssignArtistRequestDTO request)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if(customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(customerNail.Status != CustomerNailStatus.PendingReview && customerNail.Status != CustomerNailStatus.Assigned)
            {
                return new ApiErrorResult<CustomerNailDto>("Yêu cầu duyệt mẫu móng không ở trạng thái hợp lệ để phân thợ.");
            }
            var manager = await _unitOfWork.UserRepository.GetByIdAsync(managerUserId);
            if (manager == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy tài khoản quản lý.");
            }
            if (customerNail.SalonId != manager.SalonId)
            {
                return new ApiErrorResult<CustomerNailDto>("Bạn không có quyền chỉ định thợ cho mẫu móng ở chi nhánh khác.");
            }
            //var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(request.StaffArtistId);
            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistWithProfileAsync(request.StaffArtistId);
            if (artist == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy thợ nail.");
            }
            if (artist.Account?.SalonId != customerNail.SalonId)
            {
                return new ApiErrorResult<CustomerNailDto>("Thợ làm móng được chỉ định không thuộc chi nhánh cần thẩm định mẫu nail.");
            }
            customerNail.Status = CustomerNailStatus.Assigned;
            customerNail.ApprovedArtistId = request.StaffArtistId;
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            var response = _mapper.Map<CustomerNailDto>(updatedNail);
            return new ApiSuccessResult<CustomerNailDto>(response, "Chỉ định thợ thẩm định mẫu nail thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> ArtistQuoteAsync(int id, Guid artistAccountId, ArtistQuoteRequestDTO request)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if(customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(customerNail.Status != CustomerNailStatus.Assigned)
            {
                return new ApiErrorResult<CustomerNailDto>("Mẫu móng không ở trạng thái chờ Thợ Báo Giá.");
            }
            var artist = await _unitOfWork.NailArtistRepository.GetNailArtistByAccountIdAsync(artistAccountId);
            if (artist == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Tài khoản của bạn không được cấu hình là thợ nail hoặc thợ nail không hoạt động.");
            }
            if(customerNail.ApprovedArtistId != artist.NailArtistId)
            {
                return new ApiErrorResult<CustomerNailDto>("Bạn không có quyền báo giá mẫu nail này.");
            }
            customerNail.Price = request.QuotedPrice;
            customerNail.Duration = request.QuotedDuration;
            customerNail.Status = CustomerNailStatus.Reviewed;
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            var response = _mapper.Map<CustomerNailDto>(updatedNail);
            return new ApiSuccessResult<CustomerNailDto>(response, "Thợ nail đề xuất báo giá thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> ManagerApproveQuoteAsync(int id, Guid managerUserId, ManagerApproveQuoteRequestDTO request)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if(customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(customerNail.Status != CustomerNailStatus.Reviewed)
            {
                return new ApiErrorResult<CustomerNailDto>("Mẫu móng không ở trạng thái chờ Quản Lý Duyệt Báo Giá.");
            }
            var manager = await _unitOfWork.UserRepository.GetByIdAsync(managerUserId);
            if (manager == null || customerNail.SalonId != manager.SalonId)
            {
                return new ApiErrorResult<CustomerNailDto>("Bạn không có quyền chốt báo giá mẫu móng của chi nhánh khác.");
            }

            customerNail.Price = request.FinalPrice;
            customerNail.Duration = request.FinalDuration;
            customerNail.Status = CustomerNailStatus.Quoted;
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            var response = _mapper.Map<CustomerNailDto>(updatedNail);
            return new ApiSuccessResult<CustomerNailDto>(response, "Quản lý chốt giá gửi khách hàng thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> ManagerRejectRequestAsync(int id, Guid managerUserId, RejectRequestDTO request)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if(customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if(customerNail.Status != CustomerNailStatus.PendingReview && customerNail.Status != CustomerNailStatus.Assigned)
            {
                return new ApiErrorResult<CustomerNailDto>("Yêu cầu duyệt mẫu không ở trạng thái có thể từ chối.");
            }
            var manager = await _unitOfWork.UserRepository.GetByIdAsync(managerUserId);
            if (manager == null || customerNail.SalonId != manager.SalonId)
            {
                return new ApiErrorResult<CustomerNailDto>("Bạn không có quyền từ chối yêu cầu duyệt mẫu móng của chi nhánh khác.");
            }
            customerNail.Status = CustomerNailStatus.Rejected;
            customerNail.RejectReason = request.Reason;
            customerNail.Price = null;
            customerNail.Duration = null;
            customerNail.ApprovedArtistId = null;
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            var response = _mapper.Map<CustomerNailDto>(updatedNail);
            return new ApiSuccessResult<CustomerNailDto>(response, "Yêu cầu duyệt mẫu nail đã bị từ chối.");
        }

        public async Task<ApiResult<CustomerNailDto>> CustomerRespondQuoteAsync(int id, Guid customerId, CustomerRespondQuoteRequest request)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if (customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy mẫu nail tùy chỉnh.");
            }
            if (customerNail.UserId != customerId)
            {
                return new ApiErrorResult<CustomerNailDto>("Bạn không có quyền phản hồi mẫu nail này.");
            }
            if (customerNail.Status != CustomerNailStatus.Quoted)
            {
                return new ApiErrorResult<CustomerNailDto>("Mẫu móng không ở trạng thái Chờ Khách Xác Nhận.");
            }
            if (request.IsAccepted)
            {
                customerNail.Status = CustomerNailStatus.Approved;
                customerNail.RejectReason = null;
            }
            else
            {
                customerNail.Status = CustomerNailStatus.Rejected;
                customerNail.RejectReason = request.RejectReason ?? "Khách hàng từ chối báo giá.";
                customerNail.Price = null;
                customerNail.Duration = null;
                customerNail.ApprovedArtistId = null;
            }

            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();
            var updatedNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            string message = request.IsAccepted ? "Đồng ý báo giá thành công. Bạn đã có thể đặt lịch làm mẫu này." : "Từ chối báo giá thành công.";
            var response = _mapper.Map<CustomerNailDto>(updatedNail);
            return new ApiSuccessResult<CustomerNailDto>(response, message);
        }
        #endregion Additional Operations
    }
}
