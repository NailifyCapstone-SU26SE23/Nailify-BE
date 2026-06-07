using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailShapeRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class NailShapeService : INailShapeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailShapeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<NailShapeDto>>> GetPagedNailShapesAsync(int pageNumber, int pageSize, string? name = null)
        {
            var pagedResult = await _unitOfWork.NailShapeRepository.GetPagedNailShapesAsync(pageNumber, pageSize, name);
            var mappedItems = _mapper.Map<List<NailShapeDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<NailShapeDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<NailShapeDto>>(resultPagedList, "Lấy danh sách dáng móng thành công.");
        }

        public async Task<ApiResult<NailShapeDto>> GetNailShapeByIdAsync(int id)
        {
            var shape = await _unitOfWork.NailShapeRepository.GetByIdAsync(id);
            if (shape == null)
            {
                return new ApiErrorResult<NailShapeDto>("Không tìm thấy dáng móng.");
            }

            return new ApiSuccessResult<NailShapeDto>(_mapper.Map<NailShapeDto>(shape), "Lấy thông tin dáng móng thành công.");
        }

        public async Task<ApiResult<NailShapeDto>> CreateNailShapeAsync(NailShapeCreateRequest request, string? imageUrl = null)
        {
            var shape = _mapper.Map<NailShape>(request);
            shape.ImageUrl = imageUrl ?? string.Empty;
            await _unitOfWork.NailShapeRepository.CreateAsync(shape);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<NailShapeDto>(_mapper.Map<NailShapeDto>(shape), "Tạo dáng móng thành công.");
        }

        public async Task<ApiResult<NailShapeDto>> UpdateNailShapeAsync(int id, NailShapeUpdateRequest request, string? imageUrl = null)
        {
            var shape = await _unitOfWork.NailShapeRepository.GetByIdAsync(id);
            if (shape == null)
            {
                return new ApiErrorResult<NailShapeDto>("Không tìm thấy dáng móng.");
            }

            _mapper.Map(request, shape);
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                shape.ImageUrl = imageUrl;
            }

            _unitOfWork.NailShapeRepository.Update(shape);
            await _unitOfWork.SaveChangesAsync();
            await RecalculateAffectedNailVariantsAsync(id);

            return new ApiSuccessResult<NailShapeDto>(_mapper.Map<NailShapeDto>(shape), "Cập nhật dáng móng thành công.");
        }

        public async Task<ApiResult<bool>> DeleteNailShapeAsync(int id)
        {
            var shape = await _unitOfWork.NailShapeRepository.GetByIdAsync(id);
            if (shape == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy dáng móng.");
            }

            _unitOfWork.NailShapeRepository.Delete(shape);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa dáng móng thành công.");
        }

        private async Task RecalculateAffectedNailVariantsAsync(int nailShapeId)
        {
            var variants = await _unitOfWork.NailVariantRepository.GetAllNailVariantsAsync();
            var affectedVariants = variants
                .Where(variant => variant.NailShapeId == nailShapeId)
                .ToList();

            foreach (var variant in affectedVariants)
            {
                variant.Price = (variant.NailShape?.Price ?? 0m)
                    + (variant.NailSurface?.Price ?? 0m)
                    + variant.NailComponents.Sum(nailComponent => nailComponent.Component.Price);
                variant.Duration = (variant.NailShape?.Duration ?? 0)
                    + (variant.NailSurface?.Duration ?? 0)
                    + variant.NailComponents.Sum(nailComponent => nailComponent.Component.Duration ?? 0);

                _unitOfWork.NailVariantRepository.Update(variant);
            }

            await _unitOfWork.SaveChangesAsync();
            foreach (var nailDesignId in affectedVariants.Select(variant => variant.NailDesignId).Distinct())
            {
                await UpdateNailDesignPriceRangeAsync(nailDesignId);
            }
        }

        private async Task UpdateNailDesignPriceRangeAsync(int nailDesignId)
        {
            var nailDesign = await _unitOfWork.NailDesignRepository.GetByIdAsync(nailDesignId);
            if (nailDesign == null)
            {
                return;
            }

            var variants = await _unitOfWork.NailVariantRepository.GetNailVariantsByDesignIdAsync(nailDesignId);
            nailDesign.MinPrice = variants.Any() ? variants.Min(variant => variant.Price) : 0m;
            nailDesign.MaxPrice = variants.Any() ? variants.Max(variant => variant.Price) : 0m;

            _unitOfWork.NailDesignRepository.Update(nailDesign);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
