
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class WalletDepositWebhookHandler : IPaymentWebhookHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WalletDepositWebhookHandler> _logger;
        public WalletDepositWebhookHandler(
            IUnitOfWork unitOfWork,
            ILogger<WalletDepositWebhookHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public bool CanHandle(Transaction transaction)
        {
            return transaction.PaymentType == PaymentType.WalletDeposit
                    || transaction.WalletId.HasValue;
        }

        public async Task HandlePaidTransactionAsync(Transaction transaction, PaymentWebhookDto webhookDto)
        {
            if (!transaction.WalletId.HasValue)
            {
                _logger.LogError("WalletDeposit transaction {OrderCode} missing WalletId.", transaction.OrderCode);
                return;
            }
            // Lock row ví bằng Pessimistic Row Lock 
            var wallet = await _unitOfWork.CustomerWalletRepository.GetByWalletIdForUpdateAsync(transaction.WalletId.Value);

            if (wallet == null)
            {
                _logger.LogError("Wallet {WalletId} not found for deposit order {OrderCode}.", transaction.WalletId, transaction.OrderCode);
                return;
            }

            var balanceBefore = wallet.Balance;
            wallet.Balance += transaction.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            var walletTx = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                Amount = transaction.Amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = wallet.Balance,
                Type = WalletTransactionType.Deposit,
                Status = WalletTransactionStatus.Completed,
                ReferenceId = transaction.OrderCode,
                ReferenceType = WalletReferenceType.PayOs,
                Description = $"Nạp tiền vào ví qua PayOS (Mã GD: {transaction.OrderCode})",
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.CustomerWalletRepository.Update(wallet);
            await _unitOfWork.WalletTransactionRepository.CreateAsync(walletTx);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully credited {Amount} VND to Wallet {WalletId}. New Balance: {Balance}",
                transaction.Amount, wallet.WalletId, wallet.Balance);
        }
    }
}

