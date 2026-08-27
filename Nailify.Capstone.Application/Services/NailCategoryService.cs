using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailCategoryRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class NailCategoryService : INailCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<List<NailCategoryDto>>> GetByNailDesignIdAsync(int nailDesignId)
        {
            if (!await _unitOfWork.NailDesignRepository.ExistsAsync(nd => nd.NailDesignId == nailDesignId && nd.Status == "Active"))
            {
                return new ApiErrorResult<List<NailCategoryDto>>("Không tìm thấy mẫu móng.");
            }

            var nailCategories = await _unitOfWork.NailCategoryRepository.GetByNailDesignIdAsync(nailDesignId);
            return new ApiSuccessResult<List<NailCategoryDto>>(
                _mapper.Map<List<NailCategoryDto>>(nailCategories),
                "Lấy danh sách danh mục của mẫu móng thành công.");
        }

        public async Task<ApiResult<List<NailCategoryDto>>> AssignCategoriesToNailDesignAsync(int nailDesignId, List<NailCategoryRequest> request)
        {
            if (!await _unitOfWork.NailDesignRepository.ExistsAsync(nd => nd.NailDesignId == nailDesignId && nd.Status == "Active"))
            {
                return new ApiErrorResult<List<NailCategoryDto>>("Không tìm thấy mẫu móng.");
            }

            var categoryIds = request.Select(item => item.CategoryId).Distinct().ToList();
            var invalidCategoryIds = await GetInvalidCategoryIdsAsync(categoryIds);
            if (invalidCategoryIds.Any())
            {
                return new ApiErrorResult<List<NailCategoryDto>>($"Không tìm thấy danh mục: {string.Join(", ", invalidCategoryIds)}.");
            }

            var oldNailCategories = await _unitOfWork.NailCategoryRepository.GetByNailDesignIdAsync(nailDesignId);
            foreach (var oldNailCategory in oldNailCategories)
            {
                _unitOfWork.NailCategoryRepository.Delete(oldNailCategory);
            }

            foreach (var categoryId in categoryIds)
            {
                await _unitOfWork.NailCategoryRepository.CreateAsync(new NailCategory
                {
                    NailDesignId = nailDesignId,
                    CategoryId = categoryId
                });
            }

            await _unitOfWork.SaveChangesAsync();
            var updatedNailCategories = await _unitOfWork.NailCategoryRepository.GetByNailDesignIdAsync(nailDesignId);
            return new ApiSuccessResult<List<NailCategoryDto>>(
                _mapper.Map<List<NailCategoryDto>>(updatedNailCategories),
                "Cấu hình danh mục cho mẫu móng thành công.");
        }

        public async Task<ApiResult<List<NailCategoryDto>>> DeleteByNailDesignIdAsync(int nailDesignId)
        {
            if (!await _unitOfWork.NailDesignRepository.ExistsAsync(nd => nd.NailDesignId == nailDesignId && nd.Status == "Active"))
            {
                return new ApiErrorResult<List<NailCategoryDto>>("Không tìm thấy mẫu móng.");
            }

            var nailCategories = await _unitOfWork.NailCategoryRepository.GetByNailDesignIdAsync(nailDesignId);
            foreach (var nailCategory in nailCategories)
            {
                _unitOfWork.NailCategoryRepository.Delete(nailCategory);
            }

            await _unitOfWork.SaveChangesAsync();
            var updatedNailCategories = await _unitOfWork.NailCategoryRepository.GetByNailDesignIdAsync(nailDesignId);
            return new ApiSuccessResult<List<NailCategoryDto>>(
                _mapper.Map<List<NailCategoryDto>>(updatedNailCategories),
                "Xóa danh mục của mẫu móng thành công.");
        }

        private async Task<List<int>> GetInvalidCategoryIdsAsync(IEnumerable<int> categoryIds)
        {
            var invalidCategoryIds = new List<int>();

            foreach (var categoryId in categoryIds)
            {
                if (!await _unitOfWork.CategoryRepository.ExistsAsync(category => category.CategoryId == categoryId && category.Status == "Active"))
                {
                    invalidCategoryIds.Add(categoryId);
                }
            }

            return invalidCategoryIds;
        }
    }
}
