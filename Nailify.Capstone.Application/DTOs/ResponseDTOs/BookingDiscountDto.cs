using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class BookingDiscountDto : IMapFrom<BookingDiscount>
    {
        public int BookingDiscountId { get; set; }
        public Guid BookingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public bool IsAutoApplied { get; set; }
        public DateTime AppliedDate { get; set; }
        public int? PromotionId { get; set; }
        public int? LoyaltyTierId { get; set; }
        public int? LoyaltyTransactionId { get; set; }

        public void Mapping(Profile profile) => profile.CreateMap<BookingDiscount, BookingDiscountDto>();
    }
}
