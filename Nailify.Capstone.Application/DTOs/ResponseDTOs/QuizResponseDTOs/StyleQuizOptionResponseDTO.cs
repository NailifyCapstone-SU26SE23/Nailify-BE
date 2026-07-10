using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.QuizResponseDTOs
{
    public class StyleQuizOptionResponseDTO : IMapFrom<QuizOption>
    {
        public Guid QuizOptionId { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<QuizOption, StyleQuizOptionResponseDTO>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.OptionValue));
        }
    }
}
