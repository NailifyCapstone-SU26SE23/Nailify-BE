using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailSurfaceRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class NailSurfaceService : INailSurfaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailSurfaceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<NailSurfaceDto>>> GetPagedNailSurfacesAsync(int pageNumber, int pageSize, string? name = null)
        {
            var pagedResult = await _unitOfWork.NailSurfaceRepository.GetPagedNailSurfacesAsync(pageNumber, pageSize, name);
            var mappedItems = _mapper.Map<List<NailSurfaceDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<NailSurfaceDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<NailSurfaceDto>>(resultPagedList, "Lấy danh sách bề mặt móng thành công.");
        }

        public async Task<ApiResult<NailSurfaceDto>> GetNailSurfaceByIdAsync(int id)
        {
            var surface = await _unitOfWork.NailSurfaceRepository.GetByIdAsync(id);
            if (surface == null)
            {
                return new ApiErrorResult<NailSurfaceDto>("Không tìm thấy bề mặt móng.");
            }

            return new ApiSuccessResult<NailSurfaceDto>(_mapper.Map<NailSurfaceDto>(surface), "Lấy thông tin bề mặt móng thành công.");
        }

        public async Task<ApiResult<NailSurfaceDto>> CreateNailSurfaceAsync(NailSurfaceCreateRequest request)
        {
            var surface = _mapper.Map<NailSurface>(request);
            await _unitOfWork.NailSurfaceRepository.CreateAsync(surface);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<NailSurfaceDto>(_mapper.Map<NailSurfaceDto>(surface), "Tạo bề mặt móng thành công.");
        }

        public async Task<ApiResult<NailSurfaceDto>> UpdateNailSurfaceAsync(int id, NailSurfaceUpdateRequest request)
        {
            var surface = await _unitOfWork.NailSurfaceRepository.GetByIdAsync(id);
            if (surface == null)
            {
                return new ApiErrorResult<NailSurfaceDto>("Không tìm thấy bề mặt móng.");
            }

            _mapper.Map(request, surface);
            _unitOfWork.NailSurfaceRepository.Update(surface);
            await _unitOfWork.SaveChangesAsync();
            await RecalculateAffectedNailVariantsAsync(id);

            return new ApiSuccessResult<NailSurfaceDto>(_mapper.Map<NailSurfaceDto>(surface), "Cập nhật bề mặt móng thành công.");
        }

        public async Task<ApiResult<bool>> DeleteNailSurfaceAsync(int id)
        {
            var surface = await _unitOfWork.NailSurfaceRepository.GetByIdAsync(id);
            if (surface == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy bề mặt móng.");
            }

            _unitOfWork.NailSurfaceRepository.Delete(surface);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa bề mặt móng thành công.");
        }

        private async Task RecalculateAffectedNailVariantsAsync(int nailSurfaceId)
        {
            var variants = await _unitOfWork.NailVariantRepository.GetAllNailVariantsAsync();
            var affectedVariants = variants
                .Where(variant => variant.NailSurfaceId == nailSurfaceId)
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
