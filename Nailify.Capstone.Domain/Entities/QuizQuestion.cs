using Microsoft.VisualBasic.FileIO;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class QuizQuestion
    {
        public Guid QuizQuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public QuizQuestionType Type { get; set; } = QuizQuestionType.Single;
        public QuizCategory Category { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<QuizOption> QuizOptions { get; set; } = new List<QuizOption>();
        public virtual ICollection<CustomerQuizAnswer> CustomerQuizAnswers { get; set; } = new List<CustomerQuizAnswer>();
    }
}
