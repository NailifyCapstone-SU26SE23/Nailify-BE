using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class QuizOption
    {
        public Guid QuizOptionId { get; set; }
        public Guid QuizQuestionId { get; set; }
        public string OptionValue { get; set; } = string.Empty; // e.g. "Pink", "Minimal", "Almond"
        public string Label { get; set; } = string.Empty; // e.g. "Màu hồng", "Móng hạnh nhân"
        public string? Description { get; set; }
        public virtual QuizQuestion QuizQuestion { get; set; } = null!;
        public virtual ICollection<CustomerQuizAnswer> CustomerQuizAnswers { get; set; } = new List<CustomerQuizAnswer>();
    }
}
