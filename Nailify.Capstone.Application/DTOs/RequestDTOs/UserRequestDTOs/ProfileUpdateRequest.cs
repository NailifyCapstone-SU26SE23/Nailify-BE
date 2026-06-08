using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    public class ProfileUpdateRequest : IMapFrom<User>
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<ProfileUpdateRequest, User>().IgnoreAllNonExisting();
        }
    }
}
