using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs
{
    public class CreateQuizOptionRequestDTO : IMapFrom<QuizOption>
    {
        public List<string> OptionValues { get; set; } = new();
        public string Label { get; set; } = string.Empty;
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateQuizOptionRequestDTO, QuizOption>()
                   .ForMember(dest => dest.OptionValue,
                    opt => opt.MapFrom(src => System.Text.Json.JsonSerializer.Serialize(src.OptionValues, (System.Text.Json.JsonSerializerOptions?)null)));
        }
    }
}
