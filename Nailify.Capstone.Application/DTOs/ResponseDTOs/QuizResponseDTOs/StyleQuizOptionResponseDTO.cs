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
        public List<string> Values { get; set; } = new();
        public string Label { get; set; } = string.Empty;
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<QuizOption, StyleQuizOptionResponseDTO>()
                   .ForMember(dest => dest.Values, opt => opt.MapFrom(src => ParseOptionValues(src.OptionValue)));

        }
        private static List<string> ParseOptionValues(string optionValueJson)
        {
            if (string.IsNullOrEmpty(optionValueJson)) return new List<string>();
            try
            {
                if (optionValueJson.TrimStart().StartsWith("["))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<string>>(optionValueJson) ?? new List<string>();
                }
                return new List<string> { optionValueJson };
            }
            catch
            {
                return new List<string> { optionValueJson };
            }
        }
    }
}
