using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ProcedureResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

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
                return new ApiErrorResult<bool>("Không tìm thấy biến thể móng");
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
            return new ApiSuccessResult<bool>(true, "Cấu hình quy trình cho mẫu nail thành công");
        }

        public async Task<ApiResult<bool>> AssignProceduresToCustomerNailAsync(int customerNailId, List<CustomerNailProcedureRequestDTO> request)
        {
            if (!await _unitOfWork.CustomerNailRepository.ExistsAsync(cn => cn.CustomerNailId == customerNailId && cn.Status == "Active"))
            {
                return new ApiErrorResult<bool>("Không tìm thấy móng custom.");
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
            return new ApiSuccessResult<bool>(true, "Cấu hình quy trình cho mẫu móng thành công");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> CreateProcedureAsync(CreateProcedureRequestDTO request)
        {
            var procedure = _mapper.Map<Procedure>(request);
            procedure.Status = "Active";
            await _unitOfWork.ProcedureRepository.CreateAsync(procedure);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Tạo bước quy trình thành công");
        }

        public async Task<ApiResult<NailProcedureResponseDTO>> CreateCustomerNailProcedureAsync(int customerNailId, CustomerNailProcedureRequestDTO request)
        {
            if (!await _unitOfWork.CustomerNailRepository.ExistsAsync(cn => cn.CustomerNailId == customerNailId && cn.Status == "Active"))
            {
                return new ApiErrorResult<NailProcedureResponseDTO>("Không tìm thấy móng custom");
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
            return new ApiSuccessResult<NailProcedureResponseDTO>(MapNailProcedure(created!), "Thêm quy trình cho mẫu móng thành công.");
        }

        public async Task<ApiResult<bool>> DeleteProcedureAsync(Guid procedureId)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy bước quy trình");
            }

            _unitOfWork.ProcedureRepository.Delete(procedure);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa bước quy trình thành công");
        }

        public async Task<ApiResult<bool>> DeleteCustomerNailProcedureAsync(Guid nailProcedureId)
        {
            var nailProcedure = await _unitOfWork.NailProcedureRepository.GetByIdAsync(nailProcedureId);
            if (nailProcedure == null || !nailProcedure.CustomerNailId.HasValue)
            {
                return new ApiErrorResult<bool>("Không tìm thấy quy trình của mẫu móng custom");
            }

            _unitOfWork.NailProcedureRepository.Delete(nailProcedure);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa quy trình của mẫu móng thành công.");
        }

        public async Task<ApiResult<PagedList<ProcedureResponseDTO>>> GetAllProceduresAsync(ProcedurePagingParameters parameters)
        {
            System.Linq.Expressions.Expression<Func<Procedure, bool>>? predicate = null;
            if (parameters.ProcedureType.HasValue)
            {
                var procType = parameters.ProcedureType.Value;
                predicate = p => p.ProcedureType == procType;
            }
            var status = (parameters.Status == null || parameters.Status == ActiveStatusFilter.All) ? null : parameters.Status.ToString();


            var pagedProcedures = await _unitOfWork.ProcedureRepository.GetPagedAsync(
                parameters.PageIndex,
                parameters.PageSize,
                predicate,
                status,
                parameters.OrderBy);

            var mappedItems = _mapper.Map<List<ProcedureResponseDTO>>(pagedProcedures.Items);
            var response = new PagedList<ProcedureResponseDTO>(
                mappedItems,
                pagedProcedures.MetaData.TotalItems,
                parameters.PageIndex,
                parameters.PageSize);

            return new ApiSuccessResult<PagedList<ProcedureResponseDTO>>(response, "Lấy danh sách các bước quy trình thành công.");
        }

        public async Task<ApiResult<List<NailProcedureResponseDTO>>> GetNailProceduresByCustomerNailIdAsync(int customerNailId)
        {
            if (!await _unitOfWork.CustomerNailRepository.ExistsAsync(cn => cn.CustomerNailId == customerNailId && cn.Status == "Active"))
            {
                return new ApiErrorResult<List<NailProcedureResponseDTO>>("Không tìm thấy móng custom");
            }

            var nailProcedures = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByCustomerNailIdAsync(customerNailId);
            return new ApiSuccessResult<List<NailProcedureResponseDTO>>(
                nailProcedures.Select(MapNailProcedure).ToList(),
                "Lấy danh sách các bước quy trình thành công.");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> GetProcedureByIdAsync(Guid procedureId)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<ProcedureResponseDTO>("Không tìm thấy các bước quy trình.");
            }

            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Lấy chi tiết các bước quy trình thành công.");
        }

        public async Task<ApiResult<List<ProcedureResponseDTO>>> GetProceduresByVariantIdAsync(int nailVariantId)
        {
            var nailProcs = await _unitOfWork.NailProcedureRepository.GetActiveProceduresByVariantIdAsync(nailVariantId);
            var procedures = nailProcs.Select(np => np.Procedure).ToList();
            var response = _mapper.Map<List<ProcedureResponseDTO>>(procedures);
            return new ApiSuccessResult<List<ProcedureResponseDTO>>(response, "Lấy chi tiết các bước quy trình thành công.");
        }

        public async Task<ApiResult<NailProcedureResponseDTO>> UpdateCustomerNailProcedureAsync(Guid nailProcedureId, CustomerNailProcedureRequestDTO request)
        {
            var nailProcedure = await _unitOfWork.NailProcedureRepository.GetByIdAsync(nailProcedureId);
            if (nailProcedure == null || !nailProcedure.CustomerNailId.HasValue)
            {
                return new ApiErrorResult<NailProcedureResponseDTO>("Không tìm thấy các bước quy trình.");
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
            return new ApiSuccessResult<NailProcedureResponseDTO>(MapNailProcedure(updated!), "Cập nhật quy trình thành công.");
        }

        public async Task<ApiResult<ProcedureResponseDTO>> UpdateProcedureAsync(Guid procedureId, UpdateProcedureRequestDTO request)
        {
            var procedure = await _unitOfWork.ProcedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return new ApiErrorResult<ProcedureResponseDTO>("Không tìm thấy các bước quy trình.");
            }

            _mapper.Map(request, procedure);
            _unitOfWork.ProcedureRepository.Update(procedure);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ProcedureResponseDTO>(procedure);
            return new ApiSuccessResult<ProcedureResponseDTO>(response, "Cập nhật quy trình thành công.");
        }

        private static NailProcedureResponseDTO MapNailProcedure(NailProcedure nailProcedure)
        {
            return new NailProcedureResponseDTO
            {
                NailProcedureId = nailProcedure.NailProcedureId,
                NailVariantId = nailProcedure.NailVariantId,
                CustomerNailId = nailProcedure.CustomerNailId,
                ProcedureId = nailProcedure.ProcedureId,
                Name = nailProcedure.Name,
                EstimatedMinutes = nailProcedure.EstimatedMinutes,
                Price = nailProcedure.Price,
                Note = nailProcedure.Note,
                IsCustomStep = nailProcedure.IsCustomStep,
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
                return "Thứ tự các bước phải lớn hơn 0";
            }

            if (!await _unitOfWork.ProcedureRepository.ExistsAsync(procedure => procedure.ProcedureId == procedureId && procedure.Status == "Active"))
            {
                return "Không tìm thấy các bước quy trình.";
            }

            return null;
        }
    }
}
