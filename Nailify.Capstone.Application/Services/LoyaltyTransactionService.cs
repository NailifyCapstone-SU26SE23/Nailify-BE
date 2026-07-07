using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTransactionRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.Services
{
    public class LoyaltyTransactionService : ILoyaltyTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoyaltyTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<LoyaltyTransactionDto>>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Guid? userId = null)
        {
            var transactions = await _unitOfWork.LoyaltyTransactionRepository
                .GetPagedAsync(pageNumber, pageSize, userId);
            var response = new PagedList<LoyaltyTransactionDto>(
                _mapper.Map<List<LoyaltyTransactionDto>>(transactions.Items),
                transactions.MetaData.TotalItems,
                pageNumber,
                pageSize);

            return new ApiSuccessResult<PagedList<LoyaltyTransactionDto>>(
                response,
                "Lấy lịch sử điểm thành công.");
        }

        public async Task<ApiResult<LoyaltyTransactionDto>> GetByIdAsync(int id)
        {
            var transaction = await _unitOfWork.LoyaltyTransactionRepository.GetByIdAsync(id);
            return transaction == null
                ? new ApiErrorResult<LoyaltyTransactionDto>("Không tìm thấy giao dịch điểm.")
                : new ApiSuccessResult<LoyaltyTransactionDto>(_mapper.Map<LoyaltyTransactionDto>(transaction), "Lấy giao dịch điểm thành công.");
        }

        public async Task<ApiResult<LoyaltyTransactionDto>> UpdateAsync(int id, LoyaltyTransactionUpdateRequest request)
        {
            var transaction = await _unitOfWork.LoyaltyTransactionRepository.GetByIdAsync(id);
            if (transaction == null) return new ApiErrorResult<LoyaltyTransactionDto>("Không tìm thấy giao dịch điểm.");
            if (request.TransactionType != LoyaltyTransactionType.Adjusted)
                return new ApiErrorResult<LoyaltyTransactionDto>("Giao dịch chỉnh sửa phải có loại Adjusted.");

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(transaction.CustomerId);
            if (customer == null) return new ApiErrorResult<LoyaltyTransactionDto>("Không tìm thấy khách hàng.");

            var pointDifference = request.Points - transaction.Points;
            if (customer.LoyaltyPoint + pointDifference < 0)
                return new ApiErrorResult<LoyaltyTransactionDto>("Điểm hiện tại của khách hàng không thể nhỏ hơn 0.");

            customer.LoyaltyPoint += pointDifference;
            if (pointDifference > 0) customer.LifetimePoints += pointDifference;
            transaction.Points = request.Points;
            transaction.TransactionType = LoyaltyTransactionType.Adjusted;

            _unitOfWork.CustomerRepository.Update(customer);
            _unitOfWork.LoyaltyTransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<LoyaltyTransactionDto>(_mapper.Map<LoyaltyTransactionDto>(transaction), "Cập nhật giao dịch điểm thành công.");
        }
    }
}
