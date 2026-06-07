using AutoMapper;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    // Admin/Manager cập nhật hồ sơ khách hàng
    public class CustomerProfileUpdateRequest : CustomerSelfProfileUpdateRequest
    {
        public string Status { get; set; } = "Active";
        public int LoyaltyPoint { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerProfileUpdateRequest, User>().IgnoreAllNonExisting();
            profile.CreateMap<CustomerProfileUpdateRequest, Customer>().IgnoreAllNonExisting();
        }
    }
}
