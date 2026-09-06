using AutoMapper;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    // Admin update bất kỳ user - kế thừa từ ProfileUpdateRequest
    public class UserUpdateRequest : ProfileUpdateRequest
    {
        public string Status { get; set; } = "Active";
        public Guid? SalonId { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<UserUpdateRequest, User>().IgnoreAllNonExisting();
        }
    }
}
