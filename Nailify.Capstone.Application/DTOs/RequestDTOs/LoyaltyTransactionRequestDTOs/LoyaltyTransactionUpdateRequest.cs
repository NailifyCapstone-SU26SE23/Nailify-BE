using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTransactionRequestDTOs
{
    public class LoyaltyTransactionUpdateRequest
    {
        public int Points { get; set; }
        public LoyaltyTransactionType TransactionType { get; set; } = LoyaltyTransactionType.Adjusted;
    }
}
