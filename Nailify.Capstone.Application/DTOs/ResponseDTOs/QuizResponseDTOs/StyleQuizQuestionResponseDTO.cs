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
    public class StyleQuizQuestionResponseDTO : IMapFrom<QuizQuestion>
    {
        public Guid QuizQuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string Type { get; set; } = "single";
        public string CategoryKey { get; set; } = string.Empty;
        public List<StyleQuizOptionResponseDTO> Options { get; set; } = new();
        public void Mapping(Profile profile)
        {
            profile.CreateMap<QuizQuestion, StyleQuizQuestionResponseDTO>()
                .ForMember(dest => dest.CategoryKey, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString().ToLower()))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.QuizOptions));
        }
    }
}
