using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CategoryTypeRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class CategoryTypeService : ICategoryTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<CategoryTypeDto>>> GetPagedCategoryTypesAsync(
            int pageNumber,
            int pageSize,
            string? name = null)
        {
            var pagedResult = await _unitOfWork.CategoryTypeRepository.GetPagedCategoryTypesAsync(
                pageNumber,
                pageSize,
                name);
            var mappedItems = _mapper.Map<List<CategoryTypeDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<CategoryTypeDto>(
                mappedItems,
                pagedResult.MetaData.TotalItems,
                pageNumber,
                pageSize
            );

            return new ApiSuccessResult<PagedList<CategoryTypeDto>>(resultPagedList, "Lấy danh sách loại danh mục thành công.");
        }

        public async Task<ApiResult<CategoryTypeDto>> GetCategoryTypeByIdAsync(int id)
        {
            var categoryType = await _unitOfWork.CategoryTypeRepository.GetCategoryTypeWithCategoriesAsync(id);
            if (categoryType == null || categoryType.Status == "InActive")
            {
                return new ApiErrorResult<CategoryTypeDto>("Không tìm thấy loại danh mục.");
            }

            var categoryTypeDto = _mapper.Map<CategoryTypeDto>(categoryType);
            return new ApiSuccessResult<CategoryTypeDto>(categoryTypeDto, "Lấy thông tin loại danh mục thành công.");
        }

        public async Task<ApiResult<CategoryTypeDto>> CreateCategoryTypeAsync(CategoryTypeCreateRequest request)
        {
            var categoryType = _mapper.Map<CategoryType>(request);
            categoryType.Status = "Active";

            await _unitOfWork.CategoryTypeRepository.CreateAsync(categoryType);
            await _unitOfWork.SaveChangesAsync();

            var categoryTypeDto = _mapper.Map<CategoryTypeDto>(categoryType);
            return new ApiSuccessResult<CategoryTypeDto>(categoryTypeDto, "Tạo loại danh mục thành công.");
        }

        public async Task<ApiResult<CategoryTypeDto>> UpdateCategoryTypeAsync(int id, CategoryTypeUpdateRequest request)
        {
            var categoryType = await _unitOfWork.CategoryTypeRepository.GetByIdAsync(id);
            if (categoryType == null)
            {
                return new ApiErrorResult<CategoryTypeDto>("Không tìm thấy loại danh mục.");
            }

            _mapper.Map(request, categoryType);
            _unitOfWork.CategoryTypeRepository.Update(categoryType);
            await _unitOfWork.SaveChangesAsync();

            var categoryTypeDto = _mapper.Map<CategoryTypeDto>(categoryType);
            return new ApiSuccessResult<CategoryTypeDto>(categoryTypeDto, "Cập nhật loại danh mục thành công.");
        }

        public async Task<ApiResult<bool>> DeleteCategoryTypeAsync(int id)
        {
            var categoryType = await _unitOfWork.CategoryTypeRepository.GetByIdAsync(id);
            if (categoryType == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy loại danh mục.");
            }

            _unitOfWork.CategoryTypeRepository.Delete(categoryType);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa loại danh mục thành công.");
        }
    }
}
