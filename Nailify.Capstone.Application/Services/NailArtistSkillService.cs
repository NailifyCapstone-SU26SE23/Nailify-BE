using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistSkillRequestDTOs;
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
    public class NailArtistSkillService : INailArtistSkillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailArtistSkillService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<List<NailArtistSkillResponseDTO>>> AssignSkillAsync(Guid artistId, List<AssignSkillRequest> requests)
        {
            var artistExists = await _unitOfWork.NailArtistRepository.ExistsAsync(x => x.NailArtistId == artistId);
            if (!artistExists)
            {
                return new ApiErrorResult<List<NailArtistSkillResponseDTO>>("Không tìm thấy thợ nail.");
            }

            foreach(var req in requests)
            {
               var skillType = await _unitOfWork.SkillTypeRepository.GetByIdAsync(req.SkillTypeId);
                if(skillType == null || skillType.Status == "InActive")
                {
                    return new ApiErrorResult<List<NailArtistSkillResponseDTO>>($"Loại kỹ năng với ID {req.SkillTypeId} không tồn tại hoặc đã bị vô hiệu hóa.");
                }
                var existing = await _unitOfWork.NailArtistSkillRepository.GetByArtistAndSkillAsync(artistId, req.SkillTypeId);
                if(existing != null)
                {
                    existing.Level = req.Level;
                    _unitOfWork.NailArtistSkillRepository.Update(existing);
                }

                else
                {
                    var newSkill = new NailArtistSkill
                    {
                        NailArtistSkillId = Guid.NewGuid(),
                        NailArtistId = artistId,
                        SkillTypeId = req.SkillTypeId,
                        Level = req.Level
                    };
                    await _unitOfWork.NailArtistSkillRepository.CreateAsync(newSkill);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedSkills = await _unitOfWork.NailArtistSkillRepository.GetSkillsByArtistIdAsync(artistId);
            var response = _mapper.Map<List<NailArtistSkillResponseDTO>>(updatedSkills);
            return new ApiSuccessResult<List<NailArtistSkillResponseDTO>>(response, "Gán kỹ năng cho thợ nail thành công.");

        }

        public async Task<ApiResult<bool>> DeleteSkillAsync(Guid artistId, Guid skillTypeId)
        {
            var artistExists = await _unitOfWork.NailArtistRepository.ExistsAsync(x => x.NailArtistId == artistId);
            if(!artistExists)
            {
                return new ApiErrorResult<bool>("Không tìm thấy thợ nail.");
            }

            var skill = await _unitOfWork.NailArtistSkillRepository.GetByArtistAndSkillAsync(artistId, skillTypeId);
            if(skill == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy kỹ năng của thợ nail.");
            }

            _unitOfWork.NailArtistSkillRepository.Delete(skill);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa kỹ năng của thợ nail thành công.");    
        }

        public async Task<ApiResult<List<NailArtistSkillResponseDTO>>> GetSkillsByArtistIdAsync(Guid artistId)
        {
            var existing = await _unitOfWork.NailArtistRepository.ExistsAsync(x => x.NailArtistId == artistId);
            if (!existing)
            {
                return new ApiErrorResult<List<NailArtistSkillResponseDTO>>("Không tìm thấy thợ nail.");
            }

            var skills = await _unitOfWork.NailArtistSkillRepository.GetSkillsByArtistIdAsync(artistId);
            var response = _mapper.Map<List<NailArtistSkillResponseDTO>>(skills);
            return new ApiSuccessResult<List<NailArtistSkillResponseDTO>>(response, "Lấy danh sách kỹ năng của thợ nail thành công.");
        }

        public async Task<ApiResult<NailArtistSkillResponseDTO>> UpdateSkillAsync(Guid artistId, Guid skillTypeId, UpdateSkillLevelRequest request)
        {
            var artistExists = await _unitOfWork.NailArtistRepository.ExistsAsync(x => x.NailArtistId == artistId);
            if(!artistExists)
            {
                return new ApiErrorResult<NailArtistSkillResponseDTO>("Không tìm thấy thợ nail.");
            }

            var skill = await _unitOfWork.NailArtistSkillRepository.GetByArtistAndSkillAsync(artistId, skillTypeId);
            if(skill == null)
            {
                return new ApiErrorResult<NailArtistSkillResponseDTO>("Không tìm thấy kỹ năng của thợ nail.");
            }

            skill.Level = request.RequiredLevel;
            _unitOfWork.NailArtistSkillRepository.Update(skill);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<NailArtistSkillResponseDTO>(skill);
            return new ApiSuccessResult<NailArtistSkillResponseDTO>(response, "Cập nhật kỹ năng của thợ nail thành công.");
        }
    }
}
