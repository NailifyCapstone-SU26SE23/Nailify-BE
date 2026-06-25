using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTierRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class LoyaltyTierService : ILoyaltyTierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoyaltyTierService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<ApiResult<List<LoyaltyTierDto>>> GetAllAsync()
        {
            var tiers = _unitOfWork.LoyaltyTierRepository.FindAll()
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.MinLifetimePoints)
                .ToList();
            return Task.FromResult<ApiResult<List<LoyaltyTierDto>>>(
                new ApiSuccessResult<List<LoyaltyTierDto>>(
                    _mapper.Map<List<LoyaltyTierDto>>(tiers),
                    "Lấy danh sách hạng thành viên thành công."));
        }

        public async Task<ApiResult<LoyaltyTierDto>> GetByIdAsync(int id)
        {
            var tier = await _unitOfWork.LoyaltyTierRepository.GetByIdAsync(id);
            return tier == null
                ? new ApiErrorResult<LoyaltyTierDto>("Không tìm thấy hạng thành viên.")
                : new ApiSuccessResult<LoyaltyTierDto>(_mapper.Map<LoyaltyTierDto>(tier), "Lấy hạng thành viên thành công.");
        }

        public async Task<ApiResult<UserLoyaltyDto>> GetMyLoyaltyAsync(Guid userId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiErrorResult<UserLoyaltyDto>("Không tìm thấy hồ sơ khách hàng.");
            }

            LoyaltyTier? tier = null;
            if (customer.LoyaltyTierId.HasValue)
            {
                tier = await _unitOfWork.LoyaltyTierRepository.GetByIdAsync(customer.LoyaltyTierId.Value);
            }

            tier ??= _unitOfWork.LoyaltyTierRepository.FindAll()
                .FirstOrDefault(t => t.SortOrder == 1);

            if (tier == null)
            {
                return new ApiErrorResult<UserLoyaltyDto>("Không tìm thấy hạng thành viên mặc định.");
            }

            return new ApiSuccessResult<UserLoyaltyDto>(new UserLoyaltyDto
            {
                LoyaltyPoint = customer.LoyaltyPoint,
                LifetimePoints = customer.LifetimePoints,
                LoyaltyTier = _mapper.Map<LoyaltyTierDto>(tier)
            }, "Lấy thông tin thành viên thành công.");
        }

        public async Task<ApiResult<LoyaltyTierDto>> CreateAsync(LoyaltyTierRequest request, string? imageUrl = null)
        {
            var validationError = await ValidateAsync(request);
            if (validationError != null) return new ApiErrorResult<LoyaltyTierDto>(validationError);

            var tier = _mapper.Map<LoyaltyTier>(request);
            tier.ImageUrl = imageUrl;
            await _unitOfWork.LoyaltyTierRepository.CreateAsync(tier);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<LoyaltyTierDto>(_mapper.Map<LoyaltyTierDto>(tier), "Tạo hạng thành viên thành công.");
        }

        public async Task<ApiResult<LoyaltyTierDto>> UpdateAsync(int id, LoyaltyTierRequest request, string? imageUrl = null)
        {
            var tier = await _unitOfWork.LoyaltyTierRepository.GetByIdAsync(id);
            if (tier == null) return new ApiErrorResult<LoyaltyTierDto>("Không tìm thấy hạng thành viên.");

            var validationError = await ValidateAsync(request, id);
            if (validationError != null) return new ApiErrorResult<LoyaltyTierDto>(validationError);

            _mapper.Map(request, tier);
            if (imageUrl != null)
            {
                tier.ImageUrl = imageUrl;
            }
            _unitOfWork.LoyaltyTierRepository.Update(tier);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<LoyaltyTierDto>(_mapper.Map<LoyaltyTierDto>(tier), "Cập nhật hạng thành viên thành công.");
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            var tier = await _unitOfWork.LoyaltyTierRepository.GetByIdAsync(id);
            if (tier == null) return new ApiErrorResult<bool>("Không tìm thấy hạng thành viên.");

            _unitOfWork.LoyaltyTierRepository.Delete(tier);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa hạng thành viên thành công.");
        }

        private async Task<string?> ValidateAsync(LoyaltyTierRequest request, int? excludedId = null)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return "Tên hạng thành viên không được để trống.";
            if (request.MinLifetimePoints < 0 || request.MaxLifetimePoints < 0) return "Điểm hạng không được âm.";
            if (request.MinLifetimePoints.HasValue && request.MaxLifetimePoints.HasValue &&
                request.MinLifetimePoints > request.MaxLifetimePoints) return "Điểm tối thiểu không được lớn hơn điểm tối đa.";
            if (request.DiscountRate < 0 || request.DiscountRate > 1) return "Tỷ lệ giảm giá phải nằm trong khoảng 0 đến 1.";

            var duplicate = await _unitOfWork.LoyaltyTierRepository.ExistsAsync(t =>
                t.LoyaltyTierId != excludedId && t.Name.ToLower() == request.Name.Trim().ToLower());
            return duplicate ? "Tên hạng thành viên đã tồn tại." : null;
        }
    }
}
