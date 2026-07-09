using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs
{
    public class CreateQuizQuestionRequestDTO : IMapFrom<QuizQuestion>
    {
        public string QuestionText { get; set; } = string.Empty;
        public string Type { get; set; } = "single";
        public string Category { get; set; } = "Color";
        public List<CreateQuizOptionRequestDTO> Options { get; set; } = new();
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateQuizQuestionRequestDTO, QuizQuestion>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<QuizQuestionType>(src.Type, true)))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => Enum.Parse<QuizCategory>(src.Category, true)))
                .ForMember(dest => dest.QuizOptions, opt => opt.MapFrom(src => src.Options));
        }
    }
}
