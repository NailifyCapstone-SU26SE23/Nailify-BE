using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs
{
    public class CreateQuizQuestionRequestDTO : IMapFrom<QuizQuestion>
    {
        public string QuestionText { get; set; } = string.Empty;
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateQuizQuestionRequestDTO, QuizQuestion>();
        }
    }
}
