using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalletRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IWalletService
    {
        Task<ApiResult<CustomerWalletSummaryDto>> GetWalletSummaryAsync(Guid customerId);
        Task<ApiResult<string>> RequestDepositAsync(Guid customerId, decimal amount);
        Task<ApiResult<WithdrawalRequestResponseDto>> RequestWithdrawalAsync(Guid customerId, CreateWithdrawalRequestDto request);
        Task<ApiResult<string>> ConvertMoneyToPointsAsync(Guid customerId, decimal moneyAmount);
        Task<ApiResult<PagedList<WalletTransactionResponseDto>>> GetTransactionHistoryAsync(Guid customerId, int pageNumber, int pageSize);
        Task<ApiResult<WalletTransactionResponseDto>> GetWalletTransactionByIdAsync(Guid walletTransactionId);
        Task<ApiResult<PagedList<WithdrawalRequestResponseDto>>> GetPendingWithdrawalsAsync(int pageNumber, int pageSize);
        Task<ApiResult<PagedList<WithdrawalRequestResponseDto>>> GetAllWithdrawalsAsync(WithdrawalStatus? status, int pageNumber, int pageSize);
        Task<ApiResult<WithdrawalRequestResponseDto>> GetWithdrawalByIdAsync(Guid requestId);
        Task<ApiResult<PagedList<WalletTransactionResponseDto>>> GetSystemTransactionHistoryAsync(WalletTransactionType? type, WalletTransactionStatus? status, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);
        Task<ApiResult<WithdrawalRequestResponseDto>> ApproveWithdrawalAsync(Guid adminId, Guid requestId, ApproveWithdrawalDto dto);
        Task<ApiResult<WithdrawalRequestResponseDto>> RejectWithdrawalAsync(Guid adminId, Guid requestId, RejectWithdrawalDto dto);
        Task<ApiResult<SystemWalletSummaryDto>> GetSystemWalletSummaryAsync();
    }
}
