using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class LoyaltyTransactionDto : IMapFrom<LoyaltyTransaction>
    {
        public int LoyaltyTransactionId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? BookingId { get; set; }
        public int Points { get; set; }
        public LoyaltyTransactionType TransactionType { get; set; }
        public int? LoyaltyTierIdAtTime { get; set; }
        public DateTime CreatedAt { get; set; }

        public void Mapping(Profile profile) => profile.CreateMap<LoyaltyTransaction, LoyaltyTransactionDto>();
    }
}
