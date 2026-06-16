using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class BookingProcedureResponseDTO : IMapFrom<BookingProcedure>
    {
        public Guid BookingProcedureId { get; set; }
        public Guid BookingItemId { get; set; }
        public Guid? ProcedureId { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int StepOrder { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public Guid? CompletedById { get; set; }
        public string? CompletedByName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BookingProcedure, BookingProcedureResponseDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CompletedByName, opt => opt.MapFrom(x =>
                    x.CompletedBy != null && x.CompletedBy.Account != null
                    ? $"{x.CompletedBy.Account.FirstName} {x.CompletedBy.Account.LastName}"
                    : null));
        }
    }
}
