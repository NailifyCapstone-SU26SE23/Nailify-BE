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
    public class WalletTransactionResponseDto : IMapFrom<WalletTransaction>
    {
        public Guid WalletTransactionId { get; set; }
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public WalletTransactionType Type { get; set; }
        public WalletTransactionStatus Status { get; set; }
        public string? ReferenceId { get; set; }
        public WalletReferenceType? ReferenceType { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
