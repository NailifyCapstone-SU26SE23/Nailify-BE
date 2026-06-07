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
    public class NailArtistRepository : GenericRepository<NailArtist>, INailArtistRepository
    {
        public NailArtistRepository(NailifyDbContext context) : base(context) { }

        public async Task<IEnumerable<NailArtist>> GetNailArtistsBySalonIdAsync(Guid salonId)
        {
            return await FindByCondition(na => na.SalonId == salonId)
                .Include(na => na.Account)
                .ToListAsync();
        }

        public async Task<NailArtist?> GetNailArtistWithProfileAsync(Guid artistId)
        {
            return await FindByCondition(na => na.NailArtistId == artistId)
                .Include(na => na.Account)
                .FirstOrDefaultAsync();
        }
    }
}
