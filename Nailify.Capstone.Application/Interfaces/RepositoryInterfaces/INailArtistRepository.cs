using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailArtistRepository : IGenericRepository<NailArtist>
    {
        Task<IEnumerable<NailArtist>> GetNailArtistsBySalonIdAsync(Guid salonId);
        Task<NailArtist?> GetNailArtistWithProfileAsync(Guid artistId);
    }
}
