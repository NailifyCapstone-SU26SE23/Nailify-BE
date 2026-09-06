using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System.Text.Json.Serialization;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    public class UserCreateRequest : IMapFrom<User>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string? AvatarUrl { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }
        public Guid? SalonId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserCreateRequest, User>().IgnoreAllNonExisting();
        }
    }
}

