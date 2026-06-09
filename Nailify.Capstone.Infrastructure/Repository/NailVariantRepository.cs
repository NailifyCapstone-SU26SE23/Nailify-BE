using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class NailVariantRepository : GenericRepository<NailVariant>, INailVariantRepository
    {
        public NailVariantRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<List<NailVariant>> GetAllNailVariantsAsync()
        {
            return await BuildNailVariantQuery().ToListAsync();
        }

        public async Task<PagedList<NailVariant>> GetPagedNailVariantsAsync(int pageNumber, int pageSize, int? nailDesignId = null, string? name = null)
        {
            var query = BuildNailVariantQuery();
            if (nailDesignId.HasValue)
            {
                query = query.Where(nv => nv.NailDesignId == nailDesignId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(nv => nv.Name.ToLower().Contains(normalizedName));
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<NailVariant>(items, count, pageNumber, pageSize);
        }

        public async Task<NailVariant?> GetNailVariantDetailAsync(int nailVariantId)
        {
            return await BuildNailVariantQuery()
                .FirstOrDefaultAsync(nv => nv.NailVariantId == nailVariantId);
        }

        public async Task<List<NailVariant>> GetNailVariantsByIdsAsync(IEnumerable<int> nailVariantIds)
        {
            var ids = nailVariantIds
                .Where(nailVariantId => nailVariantId > 0)
                .Distinct()
                .ToList();

            return await _dbSet
                .Where(nv => ids.Contains(nv.NailVariantId))
                .ToListAsync();
        }

        public async Task<List<NailVariant>> GetNailVariantsByDesignIdAsync(int nailDesignId)
        {
            return await _dbSet
                .Where(nv => nv.NailDesignId == nailDesignId)
                .ToListAsync();
        }

        private IQueryable<NailVariant> BuildNailVariantQuery()
        {
            return _dbSet
                .Include(nv => nv.NailShape)
                .Include(nv => nv.NailSurface)
                .Include(nv => nv.NailComponents)
                .ThenInclude(nc => nc.Component);
        }

        public List<int> GetDistinctVariantIdsAsync(IEnumerable<BookingItem> items)
        {
            return items?
                .Where(x => x.NailVariantId.HasValue)
                .Select(x => x.NailVariantId!.Value)
                .Distinct()
                .ToList() ?? new List<int>();
        }

        public async Task<List<NailVariant>> GetNailVariantsCapableByArtistAsync(Guid artistId)
        {
            var artistSkills = await _context.NailArtistSkills
                .Where(nas => nas.NailArtistId == artistId)
                .ToListAsync();

            var artistSkillDict = artistSkills.ToDictionary(nas => nas.SkillTypeId, nas => nas.Level);

            var allVariants = await BuildNailVariantQuery()
                .Include(nv => nv.NailRequiredSkills)
                .ToListAsync();

            var capableVariants = new List<NailVariant>();
            foreach (var variant in allVariants)
            {
                bool isCapable = true;
                foreach (var reqSkill in variant.NailRequiredSkills)
                {
                    if (!artistSkillDict.TryGetValue(reqSkill.SkillTypeId, out var level) || level < reqSkill.RequiredLevel)
                    {
                        isCapable = false;
                        break;
                    }
                }

                if (isCapable)
                {
                    capableVariants.Add(variant);
                }
            }

            return capableVariants;
        }
    }
}
