using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IQuizOptionRepository : IGenericRepository<QuizOption>
    {
        Task<List<QuizOption>> GetOptionsWithQuestionsAsync(List<Guid> optionIds);
    }
}
