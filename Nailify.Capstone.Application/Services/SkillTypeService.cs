using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SkillTypeRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SkillTypeResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class SkillTypeService : ISkillTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SkillTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ApiResult<SkillTypeResponseDTO>> CreateSkillTypeAsync(SkillTypeCreateRequest request)
        {
            var existing = await _unitOfWork.SkillTypeRepository   
                                            .ExistsAsync(
                                            x => x.Name.ToLower() == request.Name.ToLower() 
                                            && x.Status == "Active"
                                            );
            if (existing) 
            {
                return new ApiErrorResult<SkillTypeResponseDTO>("Tên loại kỹ năng này đã tồn tại.");
            }
            var skillType = _mapper.Map<SkillType>(request);
            await _unitOfWork.SkillTypeRepository.CreateAsync(skillType);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<SkillTypeResponseDTO>(skillType);
            return new ApiSuccessResult<SkillTypeResponseDTO>(response, "Tên loại kỹ năng này đã tồn tại.");
        }

        public async Task<ApiResult<bool>> DeleteSkillTYpeAsync(Guid skillTypeId)
        {
            var skillType = await _unitOfWork.SkillTypeRepository.GetByIdAsync(skillTypeId);
            if(skillType == null || skillType.Status != "Active")
            {
                return new ApiErrorResult<bool>("Loại kỹ năng không tồn tại.");
            }

            _unitOfWork.SkillTypeRepository.Delete(skillType);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa loại kỹ năng thành công.");
        }

        public async Task<ApiResult<PagedList<SkillTypeResponseDTO>>> GetPagedSkillTypesAsync(int pageNumber, int pageSize, string? name = null)
        {
            var pagedResult = await _unitOfWork.SkillTypeRepository
                                               .GetPagedAsync(
                                                            pageNumber,
                                                            pageSize,
                                                            x => x.Status == "Active" 
                                                            && (string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name.ToLower())
                                                            ));

            var mapping = _mapper.Map<List<SkillTypeResponseDTO>>(pagedResult.Items);

            var response = new PagedList<SkillTypeResponseDTO>(
             mapping,
             pagedResult.MetaData.TotalItems,
             pagedResult.MetaData.CurrentPage,
             pagedResult.MetaData.PageSize
            );
            return new ApiSuccessResult<PagedList<SkillTypeResponseDTO>>(response, "Lấy danh sách loại kĩ năng thành công");
        }

        public async Task<ApiResult<SkillTypeResponseDTO>> GetSkillTypeByIdAsync(Guid skillTypeId)
        {
            var skillType = await _unitOfWork.SkillTypeRepository.GetByIdAsync(skillTypeId);
            if(skillType == null || skillType.Status != "InActive")
            {
                return new ApiErrorResult<SkillTypeResponseDTO>("Loại kỹ năng không tồn tại.");
            }
            var response = _mapper.Map<SkillTypeResponseDTO>(skillType);
            return new ApiSuccessResult<SkillTypeResponseDTO>(response, "Lấy loại kỹ năng thành công.");
        }

        public async Task<ApiResult<SkillTypeResponseDTO>> UpdateSkillTypeAsync(Guid skillTypeId, SkillTypeUpdateRequest request)
        {
            var skillType = await _unitOfWork.SkillTypeRepository.GetByIdAsync(skillTypeId);
            if(skillType == null || skillType.Status != "Active")
            {
                return new ApiErrorResult<SkillTypeResponseDTO>("Loại kỹ năng không tồn tại.");
            }
            var existing = await _unitOfWork.SkillTypeRepository.ExistsAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.SkillTypeId != skillTypeId && x.Status == "Active");
            if (existing)
            {
                return new ApiErrorResult<SkillTypeResponseDTO>("Tên loại kỹ năng này đã tồn tại.");
            }

            _mapper.Map(request, skillType);
            _unitOfWork.SkillTypeRepository.Update(skillType);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<SkillTypeResponseDTO>(skillType);
            return new ApiSuccessResult<SkillTypeResponseDTO>(response, "Cập nhật loại kỹ năng thành công.");
        }
    }
}
