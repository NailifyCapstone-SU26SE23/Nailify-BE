using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ProcedureResponseDTOs;
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
    public class ProcedureService : IProcedureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProcedureService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<bool>> AssignProceduresToVariantAsync(int nailVariantId, List<AssignProcedureRequestDTO> request)
        {
            var variantExists = await _unitOfWork.NailVariantRepository.ExistsAsync(v => v.NailVariantId == nailVariantId);
            if (!variantExists)
            {
                return new ApiErrorResult<bool>("Không tìm thấy biến thể móng (NailVariant).");
            }
            var oldNailProcs = _unitOfWork.NailProcedureRepository
                                                .FindByCondition(np => np.NailVariantId == nailVariantId)
                                                .ToList();
            foreach (var oldNp in oldNailProcs)
            {
                _unitOfWork.NailProcedureRepository.Delete(oldNp);
            }
            foreach (var assign in request)
            {
                var step = new NailProcedure
                {
                    NailVariantId = nailVariantId,
                    ProcedureId = assign.ProcedureId,
                    StepOrder = assign.StepOrder,
                    Status = "Active"
                };
                await _unitOfWork.NailProcedureRepository.CreateAsync(step);
            }
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Cấu hình quy trình cho mẫu nail thành công.");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> CreateProcedureAsync(CreateProcedureRequestDTO request)
        {
            var procedure = _mapper.Map<Procedure>(request);
            procedure.Status = "Active";
            await _unitOfWork.ProcedureRepository.CreateAsync(procedure);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Tạo bước quy trình mới thành công.");
        }

        public async Task<ApiResult<bool>> DeleteProcedureAsync(Guid procedureId)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy bước quy trình.");
            }
            _unitOfWork.ProcedureRepository.Delete(procedure);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa bước quy trình thành công.");
        }

        public async Task<ApiResult<PagedList<ProcedureResponseDTO>>> GetAllProceduresAsync(PagingRequestParameters parameters)
        {
            var pagedProcedures = await _unitOfWork.ProcedureRepository.GetPagedAsync(
                           parameters.PageIndex,
                           parameters.PageSize
                       );
            var mappedItems = _mapper.Map<List<ProcedureResponseDTO>>(pagedProcedures.Items);
            var response = new PagedList<ProcedureResponseDTO>(
                mappedItems,
                pagedProcedures.MetaData.TotalItems,
                parameters.PageIndex,
                parameters.PageSize
            );
            return new ApiSuccessResult<PagedList<ProcedureResponseDTO>>(response, "Lấy danh sách các bước quy trình chuẩn phân trang thành công.");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> GetProcedureByIdAsync(Guid procedureId)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<ProcedureResponseDTO>("Không tìm thấy bước quy trình.");
            }
            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Lấy chi tiết bước quy trình thành công.");
        }

        public async  Task<ApiResult<List<ProcedureResponseDTO>>> GetProceduresByVariantIdAsync(int nailVariantId)
        {
            var nailProcs = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(nailVariantId);
            var procedures = nailProcs.Select(np => np.Procedure).ToList();
            var response = _mapper.Map<List<ProcedureResponseDTO>>(procedures);
            return new ApiSuccessResult<List<ProcedureResponseDTO>>(response, "Lấy danh sách quy trình của NailVariant thành công.");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> UpdateProcedureAsync(Guid procedureId, UpdateProcedureRequestDTO request)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<ProcedureResponseDTO>("Không tìm thấy bước quy trình cần cập nhật.");
            }
            _mapper.Map(request, procedure);
            _unitOfWork.ProcedureRepository.Update(procedure);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Cập nhật bước quy trình thành công.");
        }
    }
}
