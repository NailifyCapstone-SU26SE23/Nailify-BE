using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IQuizQuestionRepository : IGenericRepository<QuizQuestion>
    {
        Task<List<QuizQuestion>> GetActiveQuestionsWithOptionsAsync();
        Task<QuizQuestion?> GetQuestionWithOptionsAsync(Guid questionId);
    }
}
