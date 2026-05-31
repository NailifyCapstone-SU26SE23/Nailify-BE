using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class NailDesignService : INailDesignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailDesignService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<List<NailDesignDto>>> GetAllNailDesignsAsync()
        {
            var designs = await _unitOfWork.NailDesignRepository.GetActiveNailDesignsAsync();
            var designDtos = _mapper.Map<List<NailDesignDto>>(designs);

            return new ApiSuccessResult<List<NailDesignDto>>(designDtos, "Lấy danh sách mẫu nail thành công.");
        }

        public async Task<ApiResult<PagedList<NailDesignDto>>> GetPagedNailDesignsAsync(int pageNumber, int pageSize)
        {
            var pagedResult = await _unitOfWork.NailDesignRepository.GetPagedActiveNailDesignsAsync(pageNumber, pageSize);
            var mappedItems = _mapper.Map<List<NailDesignDto>>(pagedResult);
            var resultPagedList = new PagedList<NailDesignDto>(
                mappedItems,
                pagedResult.GetMetaData().TotalItems,
                pageNumber,
                pageSize
            );

            return new ApiSuccessResult<PagedList<NailDesignDto>>(resultPagedList, "Lấy danh sách mẫu nail phân trang thành công.");
        }

        public async Task<ApiResult<NailDesignDto>> GetNailDesignByIdAsync(int id)
        {
            var design = await _unitOfWork.NailDesignRepository.GetNailDesignWithCategoriesAsync(id);
            if (design == null || design.Status == "InActive")
            {
                return new ApiErrorResult<NailDesignDto>("Không tìm thấy mẫu nail.");
            }

            var designDto = _mapper.Map<NailDesignDto>(design);
            return new ApiSuccessResult<NailDesignDto>(designDto, "Lấy thông tin mẫu nail thành công.");
        }

        public async Task<ApiResult<NailDesignDto>> CreateNailDesignAsync(NailDesignCreateRequest request)
        {
            var invalidCategoryIds = await GetInvalidCategoryIdsAsync(request.CategoryIds);
            if (invalidCategoryIds.Any())
            {
                return new ApiErrorResult<NailDesignDto>($"Không tìm thấy danh mục: {string.Join(", ", invalidCategoryIds)}.");
            }

            var design = _mapper.Map<NailDesign>(request);
            design.Status = "Active";
            design.NailCategories = request.CategoryIds
                .Distinct()
                .Select(categoryId => new NailCategory { CategoryId = categoryId })
                .ToList();

            await _unitOfWork.NailDesignRepository.CreateAsync(design);
            await _unitOfWork.SaveChangesAsync();

            var createdDesign = await _unitOfWork.NailDesignRepository.GetNailDesignWithCategoriesAsync(design.NailDesignId);
            var designDto = _mapper.Map<NailDesignDto>(createdDesign);

            return new ApiSuccessResult<NailDesignDto>(designDto, "Tạo mẫu nail thành công.");
        }

        public async Task<ApiResult<NailDesignDto>> UpdateNailDesignAsync(NailDesignUpdateRequest request)
        {
            var existingDesign = await _unitOfWork.NailDesignRepository.GetNailDesignWithCategoriesAsync(request.NailDesignId);
            if (existingDesign == null || existingDesign.Status == "InActive")
            {
                return new ApiErrorResult<NailDesignDto>("Không tìm thấy mẫu nail.");
            }

            var invalidCategoryIds = await GetInvalidCategoryIdsAsync(request.CategoryIds);
            if (invalidCategoryIds.Any())
            {
                return new ApiErrorResult<NailDesignDto>($"Không tìm thấy danh mục: {string.Join(", ", invalidCategoryIds)}.");
            }

            _mapper.Map(request, existingDesign);
            existingDesign.NailCategories.Clear();

            foreach (var categoryId in request.CategoryIds.Distinct())
            {
                existingDesign.NailCategories.Add(new NailCategory
                {
                    NailDesignId = existingDesign.NailDesignId,
                    CategoryId = categoryId
                });
            }

            _unitOfWork.NailDesignRepository.Update(existingDesign);
            await _unitOfWork.SaveChangesAsync();

            var updatedDesign = await _unitOfWork.NailDesignRepository.GetNailDesignWithCategoriesAsync(request.NailDesignId);
            var designDto = _mapper.Map<NailDesignDto>(updatedDesign);

            return new ApiSuccessResult<NailDesignDto>(designDto, "Cập nhật mẫu nail thành công.");
        }

        public async Task<ApiResult<bool>> DeleteNailDesignAsync(int id)
        {
            var design = await _unitOfWork.NailDesignRepository.GetByIdAsync(id);
            if (design == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy mẫu nail.");
            }

            _unitOfWork.NailDesignRepository.Delete(design);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa mẫu nail thành công.");
        }

        public async Task<ApiResult<List<NailDesignDto>>> GetNailDesignsByCategoryAsync(int categoryId)
        {
            var designs = await _unitOfWork.NailDesignRepository.GetNailDesignsByCategoryAsync(categoryId);
            var designDtos = _mapper.Map<List<NailDesignDto>>(designs);

            return new ApiSuccessResult<List<NailDesignDto>>(designDtos, "Lấy danh sách mẫu nail theo danh mục thành công.");
        }

        private async Task<List<int>> GetInvalidCategoryIdsAsync(IEnumerable<int> categoryIds)
        {
            var invalidCategoryIds = new List<int>();

            foreach (var categoryId in categoryIds.Distinct())
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (category == null || category.Status == "InActive")
                {
                    invalidCategoryIds.Add(categoryId);
                }
            }

            return invalidCategoryIds;
        }
    }
}
