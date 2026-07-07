using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.FavoriteNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class FavoriteNailService : IFavoriteNailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriteNailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<FavoriteNailDto>>> GetPagedAsync(Guid userId, int pageNumber, int pageSize)
        {
            var favorites = await _unitOfWork.FavoriteNailRepository.GetPagedByUserAsync(userId, pageNumber, pageSize);
            var response = new PagedList<FavoriteNailDto>(
                _mapper.Map<List<FavoriteNailDto>>(favorites.Items),
                favorites.MetaData.TotalItems,
                pageNumber,
                pageSize);
            return new ApiSuccessResult<PagedList<FavoriteNailDto>>(response, "Lấy danh sách nail yêu thích thành công.");
        }

        public async Task<ApiResult<FavoriteNailDto>> GetByIdAsync(Guid userId, int id)
        {
            var favorite = await _unitOfWork.FavoriteNailRepository.GetByIdForUserAsync(id, userId);
            return favorite == null
                ? new ApiErrorResult<FavoriteNailDto>("Không tìm thấy nail yêu thích.")
                : new ApiSuccessResult<FavoriteNailDto>(_mapper.Map<FavoriteNailDto>(favorite), "Lấy nail yêu thích thành công.");
        }

        public async Task<ApiResult<FavoriteNailDto>> CreateAsync(Guid userId, FavoriteNailRequest request)
        {
            var resolved = await ResolveReferencesAsync(request);
            if (!resolved.IsSucceeded) return new ApiErrorResult<FavoriteNailDto>(resolved.Message);

            var duplicate = await _unitOfWork.FavoriteNailRepository.ExistsAsync(f =>
                f.UserId == userId &&
                ((resolved.NailVariantId != null && f.NailVariantId == resolved.NailVariantId) ||
                 (resolved.NailVariantId == null && f.NailVariantId == null && f.NailDesignId == resolved.NailDesignId)));
            if (duplicate) return new ApiErrorResult<FavoriteNailDto>("Nail này đã có trong danh sách yêu thích.");

            var favorite = new FavoriteNail
            {
                UserId = userId,
                NailDesignId = resolved.NailDesignId,
                NailVariantId = resolved.NailVariantId
            };
            await _unitOfWork.FavoriteNailRepository.CreateAsync(favorite);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.FavoriteNailRepository.GetByIdForUserAsync(favorite.FavoriteNailId, userId);
            return new ApiSuccessResult<FavoriteNailDto>(_mapper.Map<FavoriteNailDto>(created), "Thêm nail yêu thích thành công.");
        }

        public async Task<ApiResult<FavoriteNailDto>> UpdateAsync(Guid userId, int id, FavoriteNailRequest request)
        {
            var favorite = await _unitOfWork.FavoriteNailRepository.GetTrackedByIdForUserAsync(id, userId);
            if (favorite == null) return new ApiErrorResult<FavoriteNailDto>("Không tìm thấy nail yêu thích.");

            var resolved = await ResolveReferencesAsync(request);
            if (!resolved.IsSucceeded) return new ApiErrorResult<FavoriteNailDto>(resolved.Message);

            var duplicate = await _unitOfWork.FavoriteNailRepository.ExistsAsync(f =>
                f.FavoriteNailId != id && f.UserId == userId &&
                ((resolved.NailVariantId != null && f.NailVariantId == resolved.NailVariantId) ||
                 (resolved.NailVariantId == null && f.NailVariantId == null && f.NailDesignId == resolved.NailDesignId)));
            if (duplicate) return new ApiErrorResult<FavoriteNailDto>("Nail này đã có trong danh sách yêu thích.");

            favorite.NailDesignId = resolved.NailDesignId;
            favorite.NailVariantId = resolved.NailVariantId;
            _unitOfWork.FavoriteNailRepository.Update(favorite);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.FavoriteNailRepository.GetByIdForUserAsync(id, userId);
            return new ApiSuccessResult<FavoriteNailDto>(_mapper.Map<FavoriteNailDto>(updated), "Cập nhật nail yêu thích thành công.");
        }

        public async Task<ApiResult<bool>> DeleteAsync(Guid userId, int id)
        {
            var favorite = await _unitOfWork.FavoriteNailRepository.GetTrackedByIdForUserAsync(id, userId);
            if (favorite == null) return new ApiErrorResult<bool>("Không tìm thấy nail yêu thích.");

            _unitOfWork.FavoriteNailRepository.Delete(favorite);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa nail yêu thích thành công.");
        }

        private async Task<(bool IsSucceeded, string Message, int? NailDesignId, int? NailVariantId)> ResolveReferencesAsync(FavoriteNailRequest request)
        {
            if (request.NailDesignId == null && request.NailVariantId == null)
                return (false, "NailDesignId và NailVariantId không thể đồng thời để trống.", null, null);

            if (request.NailVariantId != null)
            {
                var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(request.NailVariantId.Value);
                if (variant == null) return (false, "Không tìm thấy biến thể nail.", null, null);
                if (request.NailDesignId != null && request.NailDesignId != variant.NailDesignId)
                    return (false, "NailVariant không thuộc NailDesign đã chọn.", null, null);
                return (true, string.Empty, variant.NailDesignId, variant.NailVariantId);
            }

            var design = await _unitOfWork.NailDesignRepository.GetByIdAsync(request.NailDesignId!.Value);
            return design == null
                ? (false, "Không tìm thấy mẫu nail.", null, null)
                : (true, string.Empty, design.NailDesignId, null);
        }
    }
}
