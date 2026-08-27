using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs
{
    public class SubmitQuizAnswersRequestDto
    {
        public List<Guid> SelectedOptionIds { get; set; } = new();
    }
}
