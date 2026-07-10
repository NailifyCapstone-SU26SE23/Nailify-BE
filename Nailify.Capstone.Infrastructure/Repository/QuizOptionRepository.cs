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
    public class QuizOptionRepository : GenericRepository<QuizOption>, IQuizOptionRepository
    {
        public QuizOptionRepository(NailifyDbContext context) : base(context)
        { }
        public async Task<List<QuizOption>> GetOptionsWithQuestionsAsync(List<Guid> optionIds)
         => await FindByCondition(o => optionIds.Contains(o.QuizOptionId))
                .Include(o => o.QuizQuestion)
                .ToListAsync();
    }
}

