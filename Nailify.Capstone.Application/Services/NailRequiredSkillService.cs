using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailRequiredSkillRequestDTOs;
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
    public class NailRequiredSkillService : INailRequiredSkillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailRequiredSkillService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<List<NailRequiredSkillResponseDTO>>> AssignRequiredSkillsAsync(int designId, List<AssignRequiredSkillRequest> requests)
        {
            var designExist = await _unitOfWork.NailDesignRepository.ExistsAsync(x => x.NailDesignId == designId);
            if (!designExist)
            {
                return new ApiErrorResult<List<NailRequiredSkillResponseDTO>>("Không tìm thấy thiết kế nail.");
            }

            foreach (var req in requests)
            {
                var skillType = await _unitOfWork.SkillTypeRepository.GetByIdAsync(req.SkillTypeId);
                if (skillType == null || skillType.Status == "InActive")
                {
                    return new ApiErrorResult<List<NailRequiredSkillResponseDTO>>($"Loại kỹ năng với ID {req.SkillTypeId} không tồn tại hoặc đã bị vô hiệu hóa.");
                }

                var existing = await _unitOfWork.NailRequiredSkillRepository.GetByNailDesignAndSkillAsync(designId, req.SkillTypeId);
                if (existing != null)
                {
                    _mapper.Map(req, existing);
                    _unitOfWork.NailRequiredSkillRepository.Update(existing);
                }
                else
                {
                    var newRequiredSkill = _mapper.Map<NailRequiredSkill>(req);
                    newRequiredSkill.NailRequiredSkillId = Guid.NewGuid();
                    newRequiredSkill.NailDesignId = designId;
                    await _unitOfWork.NailRequiredSkillRepository.CreateAsync(newRequiredSkill);
                }
            }
            await _unitOfWork.SaveChangesAsync();

            var updatedSkills = await _unitOfWork.NailRequiredSkillRepository.GetSkillsByDesignIdAsync(designId);
            var response= _mapper.Map<List<NailRequiredSkillResponseDTO>>(updatedSkills);
            return new ApiSuccessResult<List<NailRequiredSkillResponseDTO>>(response, "Gán kỹ năng yêu cầu cho thiết kế nail thành công.");
        }

        public async Task<ApiResult<bool>> DeleteRequiredSkillAsync(int designId, Guid skillId)
        {
            var designExists = await _unitOfWork.NailDesignRepository.ExistsAsync(x => x.NailDesignId == designId);
            if (!designExists)
            {
                return new ApiErrorResult<bool>("Không tìm thấy thiết kế nail.");
            }

            var requiredSkill = await _unitOfWork.NailRequiredSkillRepository.GetByNailDesignAndSkillAsync(designId, skillId);
            if (requiredSkill == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy kỹ năng yêu cầu cho thiết kế nail.");
            }
            _unitOfWork.NailRequiredSkillRepository.Delete(requiredSkill);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa kỹ năng yêu cầu khỏi thiết kế nail thành công.");
        }

        public async Task<ApiResult<List<NailRequiredSkillResponseDTO>>> GetRequiredSkillsByDesignIdAsync(int nailId)
        {
            var designExist = await _unitOfWork.NailDesignRepository.ExistsAsync(x => x.NailDesignId == nailId);
            if (!designExist)
            {
                return new ApiErrorResult<List<NailRequiredSkillResponseDTO>>("Không tìm thấy thiết kế nail.");
            }

            var skills = await _unitOfWork.NailRequiredSkillRepository.GetSkillsByDesignIdAsync(nailId);
            var response = _mapper.Map<List<NailRequiredSkillResponseDTO>>(skills);
            return new ApiSuccessResult<List<NailRequiredSkillResponseDTO>>(response, "Lấy danh sách kỹ năng yêu cầu cho thiết kế nail thành công.");
        }

        public async Task<ApiResult<NailRequiredSkillResponseDTO>> UpdateRequiredSkillLevelAsync(int designId, Guid skillId, UpdateRequiredSkillLevelRequest request)
        {
            var designExist = await _unitOfWork.NailDesignRepository.ExistsAsync(x => x.NailDesignId == designId);
            if(!designExist)
            {
                return new ApiErrorResult<NailRequiredSkillResponseDTO>("Không tìm thấy thiết kế nail.");
            }
            var requiredSkill = await _unitOfWork.NailRequiredSkillRepository.GetByNailDesignAndSkillAsync(designId, skillId);
            if (requiredSkill == null)
            {
                return new ApiErrorResult<NailRequiredSkillResponseDTO>("Kỹ năng yêu cầu không tồn tại.");
            }
            _mapper.Map(request, requiredSkill);
            _unitOfWork.NailRequiredSkillRepository.Update(requiredSkill);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<NailRequiredSkillResponseDTO>(requiredSkill);
            return new ApiSuccessResult<NailRequiredSkillResponseDTO>(response, "Cập nhật mức độ kỹ năng yêu cầu thành công.");
        }
    }
}
