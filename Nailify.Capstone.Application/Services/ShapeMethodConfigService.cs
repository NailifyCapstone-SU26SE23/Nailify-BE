using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ShapeMethodConfigRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class ShapeMethodConfigService : IShapeMethodConfigService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShapeMethodConfigService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<ShapeMethodConfigDto>>> GetPagedShapeMethodConfigsAsync(int pageNumber, int pageSize, int? nailShapeId = null, string? name = null)
        {
            var pagedConfigs = await _unitOfWork.ShapeMethodConfigRepository.GetPagedShapeMethodConfigsAsync(pageNumber, pageSize, nailShapeId, name);
            var response = new PagedList<ShapeMethodConfigDto>(
                _mapper.Map<List<ShapeMethodConfigDto>>(pagedConfigs.Items),
                pagedConfigs.MetaData.TotalItems,
                pageNumber,
                pageSize);

            return new ApiSuccessResult<PagedList<ShapeMethodConfigDto>>(response, "Lấy danh sách cấu hình thành công.");
        }

        public async Task<ApiResult<ShapeMethodConfigDto>> GetShapeMethodConfigByIdAsync(int id)
        {
            var config = await _unitOfWork.ShapeMethodConfigRepository.GetByIdAsync(id);
            if (config == null)
            {
                return new ApiErrorResult<ShapeMethodConfigDto>("Không tìm thấy cấu hình.");
            }

            return new ApiSuccessResult<ShapeMethodConfigDto>(_mapper.Map<ShapeMethodConfigDto>(config), "Lấy cấu hình thành công.");
        }

        public async Task<ApiResult<List<ShapeMethodConfigDto>>> GetShapeMethodConfigsByNailShapeIdAsync(int nailShapeId)
        {
            if (!await _unitOfWork.NailShapeRepository.ExistsAsync(shape => shape.NailShapeId == nailShapeId && shape.Status == "Active"))
            {
                return new ApiErrorResult<List<ShapeMethodConfigDto>>("Không tìm thấy dáng móng.");
            }

            var configs = await _unitOfWork.ShapeMethodConfigRepository.GetActiveByNailShapeIdAsync(nailShapeId);
            return new ApiSuccessResult<List<ShapeMethodConfigDto>>(_mapper.Map<List<ShapeMethodConfigDto>>(configs), "Lấy cấu hình thành công.");
        }

        public async Task<ApiResult<ShapeMethodConfigDto>> CreateShapeMethodConfigAsync(ShapeMethodConfigCreateRequest request)
        {
            var validationError = await ValidateRequestAsync(request.NailShapeId, request.Price, request.Duration);
            if (validationError != null)
            {
                return new ApiErrorResult<ShapeMethodConfigDto>(validationError);
            }

            var config = _mapper.Map<ShapeMethodConfig>(request);
            config.Status = "Active";
            await _unitOfWork.ShapeMethodConfigRepository.CreateAsync(config);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<ShapeMethodConfigDto>(_mapper.Map<ShapeMethodConfigDto>(config), "Tạo cấu hình thành công.");
        }

        public async Task<ApiResult<ShapeMethodConfigDto>> UpdateShapeMethodConfigAsync(int id, ShapeMethodConfigUpdateRequest request)
        {
            var config = await _unitOfWork.ShapeMethodConfigRepository.GetByIdAsync(id);
            if (config == null)
            {
                return new ApiErrorResult<ShapeMethodConfigDto>("Không tìm thấy cấu hình.");
            }

            var validationError = await ValidateRequestAsync(request.NailShapeId, request.Price, request.Duration);
            if (validationError != null)
            {
                return new ApiErrorResult<ShapeMethodConfigDto>(validationError);
            }

            _mapper.Map(request, config);
            _unitOfWork.ShapeMethodConfigRepository.Update(config);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<ShapeMethodConfigDto>(_mapper.Map<ShapeMethodConfigDto>(config), "Cập nhật cấu hình thành công.");
        }

        public async Task<ApiResult<bool>> DeleteShapeMethodConfigAsync(int id)
        {
            var config = await _unitOfWork.ShapeMethodConfigRepository.GetByIdAsync(id);
            if (config == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy cấu hình.");
            }

            _unitOfWork.ShapeMethodConfigRepository.Delete(config);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa cấu hình thành công.");
        }

        private async Task<string?> ValidateRequestAsync(int nailShapeId, decimal price, int duration)
        {
            if (!await _unitOfWork.NailShapeRepository.ExistsAsync(shape => shape.NailShapeId == nailShapeId && shape.Status == "Active"))
            {
                return "Không tìm thấy dáng móng.";
            }

            if (price < 0)
            {
                return "Giá cấu hình phải lớn hơn 0.";
            }

            if (duration < 0)
            {
                return "Thời lượng phải lớn hơn 0.";
            }

            return null;
        }
    }
}
