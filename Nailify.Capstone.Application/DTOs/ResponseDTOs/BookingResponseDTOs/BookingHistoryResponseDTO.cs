using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class BookingHistoryResponseDTO : IMapFrom<BookingHistory>
    {
        public Guid BookingHistoryId { get; set; }
        public Guid BookingId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Payload { get; set; }
        public Guid? ActorId { get; set; }
        public string? ActorName { get; set; }
        public DateTime CreatedAt { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BookingHistory, BookingHistoryResponseDTO>()
                .ForMember(
                    dest => dest.ActorName,
                    opt => opt.MapFrom(src => src.Actor == null ? null : src.Actor.FirstName + " " + src.Actor.LastName));
        }
    }
}
