using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<CategoryDto>>> GetPagedCategoriesAsync(
            int pageNumber,
            int pageSize,
            string? name = null,
            int? categoryTypeId = null)
        {
            var pagedResult = await _unitOfWork.CategoryRepository.GetPagedCategoriesAsync(
                pageNumber,
                pageSize,
                name,
                categoryTypeId);
            var mappedItems = _mapper.Map<List<CategoryDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<CategoryDto>(
                mappedItems,
                pagedResult.MetaData.TotalItems,
                pageNumber,
                pageSize
            );

            return new ApiSuccessResult<PagedList<CategoryDto>>(resultPagedList, "Lấy danh sách danh mục thành công.");
        }

        public async Task<ApiResult<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetCategoryWithDesignsAsync(id);
            if (category == null || category.Status == "InActive")
            {
                return new ApiErrorResult<CategoryDto>("Không tìm thấy danh mục.");
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return new ApiSuccessResult<CategoryDto>(categoryDto, "Lấy thông tin danh mục thành công.");
        }

        public async Task<ApiResult<CategoryDto>> CreateCategoryAsync(CategoryCreateRequest request)
        {
            var categoryType = await _unitOfWork.CategoryTypeRepository.GetByIdAsync(request.CategoryTypeId);
            if (categoryType == null || categoryType.Status == "InActive")
            {
                return new ApiErrorResult<CategoryDto>("Không tìm thấy loại danh mục.");
            }

            var category = _mapper.Map<Category>(request);
            category.Status = "Active";

            await _unitOfWork.CategoryRepository.CreateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            category.CategoryType = categoryType;
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return new ApiSuccessResult<CategoryDto>(categoryDto, "Tạo danh mục thành công.");
        }

        public async Task<ApiResult<CategoryDto>> UpdateCategoryAsync(int id, CategoryUpdateRequest request)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return new ApiErrorResult<CategoryDto>("Không tìm thấy danh mục.");
            }

            var categoryType = await _unitOfWork.CategoryTypeRepository.GetByIdAsync(request.CategoryTypeId);
            if (categoryType == null || categoryType.Status == "InActive")
            {
                return new ApiErrorResult<CategoryDto>("Không tìm thấy loại danh mục.");
            }

            _mapper.Map(request, category);
            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            category.CategoryType = categoryType;
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return new ApiSuccessResult<CategoryDto>(categoryDto, "Cập nhật danh mục thành công.");
        }

        public async Task<ApiResult<bool>> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy danh mục.");
            }

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa danh mục thành công.");
        }

    }
}
