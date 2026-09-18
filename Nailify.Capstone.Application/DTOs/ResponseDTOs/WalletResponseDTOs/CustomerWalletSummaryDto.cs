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
    public class CustomerWalletSummaryDto : IMapFrom<CustomerWallet>
    {
        public Guid WalletId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenBalance { get; set; }
        public decimal AvailableBalance => Balance - FrozenBalance;
        public WalletStatus Status { get; set; }
        public int LoyaltyPoints { get; set; }
        public int LifetimePoints { get; set; }
        public string? LoyaltyTierName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
