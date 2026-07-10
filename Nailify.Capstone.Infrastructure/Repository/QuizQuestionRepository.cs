using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class QuizQuestionRepository : GenericRepository<QuizQuestion>, IQuizQuestionRepository
    {
        public QuizQuestionRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<QuizQuestion>> GetActiveQuestionsWithOptionsAsync()
        => await FindByCondition(q => q.IsActive)
                         .Include(q => q.QuizOptions)
                         .OrderBy(q => q.QuizQuestionId)
                         .ToListAsync();

        public async Task<QuizQuestion?> GetQuestionWithOptionsAsync(Guid questionId)
        => await FindByCondition(q => q.QuizQuestionId == questionId, trackChanges: true)
                         .Include(q => q.QuizOptions)
                         .FirstOrDefaultAsync();
    }
}
