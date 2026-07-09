using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class CustomerQuizAnswer
    {
        public Guid CustomerQuizAnswerId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid QuizQuestionId { get; set; }
        public Guid QuizOptionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual Customer Customer { get; set; } = null!;
        public virtual QuizQuestion QuizQuestion { get; set; } = null!;
        public virtual QuizOption QuizOption { get; set; } = null!;
    }
}
