using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ProcedureResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

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
            var variantExists = await _unitOfWork.NailVariantRepository.ExistsAsync(v => v.NailVariantId == nailVariantId && v.Status == "Active");
            if (!variantExists)
            {
                return new ApiErrorResult<bool>("Khong tim thay bien the mong (NailVariant).");
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
                var validationError = await ValidateProcedureAsync(assign.ProcedureId, assign.StepOrder);
                if (validationError != null)
                {
                    return new ApiErrorResult<bool>(validationError);
                }

                await _unitOfWork.NailProcedureRepository.CreateAsync(new NailProcedure
                {
                    NailVariantId = nailVariantId,
                    ProcedureId = assign.ProcedureId,
                    StepOrder = assign.StepOrder,
                    Status = "Active"
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Cau hinh quy trinh cho mau nail thanh cong.");
        }

        public async Task<ApiResult<bool>> AssignProceduresToCustomerNailAsync(int customerNailId, List<CustomerNailProcedureRequestDTO> request)
        {
            if (!await _unitOfWork.CustomerNailRepository.ExistsAsync(cn => cn.CustomerNailId == customerNailId && cn.Status == "Active"))
            {
                return new ApiErrorResult<bool>("Khong tim thay mau mong custom.");
            }

            var oldNailProcedures = _unitOfWork.NailProcedureRepository
                .FindByCondition(np => np.CustomerNailId == customerNailId)
                .ToList();

            foreach (var oldProcedure in oldNailProcedures)
            {
                _unitOfWork.NailProcedureRepository.Delete(oldProcedure);
            }

            foreach (var assign in request)
            {
                var validationError = await ValidateProcedureAsync(assign.ProcedureId, assign.StepOrder);
                if (validationError != null)
                {
                    return new ApiErrorResult<bool>(validationError);
                }

                await _unitOfWork.NailProcedureRepository.CreateAsync(new NailProcedure
                {
                    CustomerNailId = customerNailId,
                    ProcedureId = assign.ProcedureId,
                    StepOrder = assign.StepOrder,
                    Status = "Active"
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Cau hinh quy trinh cho mau mong custom thanh cong.");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> CreateProcedureAsync(CreateProcedureRequestDTO request)
        {
            var procedure = _mapper.Map<Procedure>(request);
            procedure.Status = "Active";
            await _unitOfWork.ProcedureRepository.CreateAsync(procedure);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Tao buoc quy trinh moi thanh cong.");
        }

        public async Task<ApiResult<NailProcedureResponseDTO>> CreateCustomerNailProcedureAsync(int customerNailId, CustomerNailProcedureRequestDTO request)
        {
            if (!await _unitOfWork.CustomerNailRepository.ExistsAsync(cn => cn.CustomerNailId == customerNailId && cn.Status == "Active"))
            {
                return new ApiErrorResult<NailProcedureResponseDTO>("Khong tim thay mau mong custom.");
            }

            var validationError = await ValidateProcedureAsync(request.ProcedureId, request.StepOrder);
            if (validationError != null)
            {
                return new ApiErrorResult<NailProcedureResponseDTO>(validationError);
            }

            var nailProcedure = new NailProcedure
            {
                CustomerNailId = customerNailId,
                ProcedureId = request.ProcedureId,
                StepOrder = request.StepOrder,
                Status = "Active"
            };

            await _unitOfWork.NailProcedureRepository.CreateAsync(nailProcedure);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.NailProcedureRepository.GetNailProcedureWithProcedureAsync(nailProcedure.NailProcedureId);
            return new ApiSuccessResult<NailProcedureResponseDTO>(MapNailProcedure(created!), "Them quy trinh cho mau mong custom thanh cong.");
        }

        public async Task<ApiResult<bool>> DeleteProcedureAsync(Guid procedureId)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<bool>("Khong tim thay buoc quy trinh.");
            }

            _unitOfWork.ProcedureRepository.Delete(procedure);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xoa buoc quy trinh thanh cong.");
        }

        public async Task<ApiResult<bool>> DeleteCustomerNailProcedureAsync(Guid nailProcedureId)
        {
            var nailProcedure = await _unitOfWork.NailProcedureRepository.GetByIdAsync(nailProcedureId);
            if (nailProcedure == null || !nailProcedure.CustomerNailId.HasValue)
            {
                return new ApiErrorResult<bool>("Khong tim thay quy trinh cua mau mong custom.");
            }

            _unitOfWork.NailProcedureRepository.Delete(nailProcedure);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xoa quy trinh cua mau mong custom thanh cong.");
        }

        public async Task<ApiResult<PagedList<ProcedureResponseDTO>>> GetAllProceduresAsync(PagingRequestParameters parameters)
        {
            var pagedProcedures = await _unitOfWork.ProcedureRepository.GetPagedAsync(parameters.PageIndex, parameters.PageSize);
            var mappedItems = _mapper.Map<List<ProcedureResponseDTO>>(pagedProcedures.Items);
            var response = new PagedList<ProcedureResponseDTO>(
                mappedItems,
                pagedProcedures.MetaData.TotalItems,
                parameters.PageIndex,
                parameters.PageSize);

            return new ApiSuccessResult<PagedList<ProcedureResponseDTO>>(response, "Lay danh sach cac buoc quy trinh phan trang thanh cong.");
        }

        public async Task<ApiResult<List<NailProcedureResponseDTO>>> GetNailProceduresByCustomerNailIdAsync(int customerNailId)
        {
            if (!await _unitOfWork.CustomerNailRepository.ExistsAsync(cn => cn.CustomerNailId == customerNailId && cn.Status == "Active"))
            {
                return new ApiErrorResult<List<NailProcedureResponseDTO>>("Khong tim thay mau mong custom.");
            }

            var nailProcedures = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByCustomerNailIdAsync(customerNailId);
            return new ApiSuccessResult<List<NailProcedureResponseDTO>>(
                nailProcedures.Select(MapNailProcedure).ToList(),
                "Lay danh sach quy trinh cua mau mong custom thanh cong.");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> GetProcedureByIdAsync(Guid procedureId)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<ProcedureResponseDTO>("Khong tim thay buoc quy trinh.");
            }

            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Lay chi tiet buoc quy trinh thanh cong.");
        }

        public async Task<ApiResult<List<ProcedureResponseDTO>>> GetProceduresByVariantIdAsync(int nailVariantId)
        {
            var nailProcs = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(nailVariantId);
            var procedures = nailProcs.Select(np => np.Procedure).ToList();
            var response = _mapper.Map<List<ProcedureResponseDTO>>(procedures);
            return new ApiSuccessResult<List<ProcedureResponseDTO>>(response, "Lay danh sach quy trinh cua NailVariant thanh cong.");
        }

        public async Task<ApiResult<NailProcedureResponseDTO>> UpdateCustomerNailProcedureAsync(Guid nailProcedureId, CustomerNailProcedureRequestDTO request)
        {
            var nailProcedure = await _unitOfWork.NailProcedureRepository.GetByIdAsync(nailProcedureId);
            if (nailProcedure == null || !nailProcedure.CustomerNailId.HasValue)
            {
                return new ApiErrorResult<NailProcedureResponseDTO>("Khong tim thay quy trinh cua mau mong custom.");
            }

            var validationError = await ValidateProcedureAsync(request.ProcedureId, request.StepOrder);
            if (validationError != null)
            {
                return new ApiErrorResult<NailProcedureResponseDTO>(validationError);
            }

            nailProcedure.ProcedureId = request.ProcedureId;
            nailProcedure.StepOrder = request.StepOrder;
            _unitOfWork.NailProcedureRepository.Update(nailProcedure);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.NailProcedureRepository.GetNailProcedureWithProcedureAsync(nailProcedureId);
            return new ApiSuccessResult<NailProcedureResponseDTO>(MapNailProcedure(updated!), "Cap nhat quy trinh cua mau mong custom thanh cong.");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> UpdateProcedureAsync(Guid procedureId, UpdateProcedureRequestDTO request)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<ProcedureResponseDTO>("Khong tim thay buoc quy trinh can cap nhat.");
            }

            _mapper.Map(request, procedure);
            _unitOfWork.ProcedureRepository.Update(procedure);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Cap nhat buoc quy trinh thanh cong.");
        }

        private static NailProcedureResponseDTO MapNailProcedure(NailProcedure nailProcedure)
        {
            return new NailProcedureResponseDTO
            {
                NailProcedureId = nailProcedure.NailProcedureId,
                NailVariantId = nailProcedure.NailVariantId,
                CustomerNailId = nailProcedure.CustomerNailId,
                ProcedureId = nailProcedure.ProcedureId,
                ProcedureName = nailProcedure.Procedure?.Name ?? string.Empty,
                ProcedureDescription = nailProcedure.Procedure?.Description,
                ProcedureDuration = nailProcedure.Procedure?.Duration,
                StepOrder = nailProcedure.StepOrder,
                Status = nailProcedure.Status
            };
        }

        private async Task<string?> ValidateProcedureAsync(Guid procedureId, int stepOrder)
        {
            if (stepOrder <= 0)
            {
                return "Thu tu buoc quy trinh phai lon hon 0.";
            }

            if (!await _unitOfWork.ProcedureRepository.ExistsAsync(procedure => procedure.ProcedureId == procedureId && procedure.Status == "Active"))
            {
                return "Khong tim thay buoc quy trinh.";
            }

            return null;
        }
    }
}
