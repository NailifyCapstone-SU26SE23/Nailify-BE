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
    }
}
