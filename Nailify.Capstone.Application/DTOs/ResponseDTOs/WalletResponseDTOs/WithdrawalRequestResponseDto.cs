using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs
{
    public class WithdrawalRequestResponseDto : IMapFrom<WithdrawalRequest>
    {
        public Guid WithdrawalRequestId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public string BankCode { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string AccountHolderName { get; set; } = null!;
        public WithdrawalStatus Status { get; set; }
        public string? TransactionReference { get; set; }
        public string? AdminNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
