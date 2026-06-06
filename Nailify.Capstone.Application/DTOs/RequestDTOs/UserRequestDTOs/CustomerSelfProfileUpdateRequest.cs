using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    public class CustomerSelfProfileUpdateRequest : IMapFrom<User>, IMapFrom<Customer>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }

        public string? SkinTone { get; set; }
        public string? Occupation { get; set; }
        public string? NailCondition { get; set; }
        public string? PersonaId { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerSelfProfileUpdateRequest, User>().IgnoreAllNonExisting();
            profile.CreateMap<CustomerSelfProfileUpdateRequest, Customer>().IgnoreAllNonExisting();
        }
    }
}
