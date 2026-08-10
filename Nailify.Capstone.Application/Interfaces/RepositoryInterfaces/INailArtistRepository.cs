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
        Task<List<NailArtist>> GetSuggestedArtistsAsync(Guid salonId, List<int> nailVariantIds);
        Task<NailArtist?> GetNailArtistByAccountIdAsync(Guid accountId);
        Task<NailArtist?> GetArtistWithLockAsync(Guid artistId);
        Task<List<NailArtist>> GetArtistsWithSkillsBySalonIdAsync(Guid salonId);
        /// <summary>
        /// Lấy danh sách tất cả các Thợ ứng viên tiềm năng tại cùng Salon có thể thay thế cho thợ bị nghỉ đột xuất
        /// </summary>
        /// <param name="salonId"></param>
        /// <param name="excludingArtistId"></param>
        /// <returns></returns>
        Task<List<NailArtist>> GetActiveArtistsWithSchedulesAndSkillsBySalonAsync(Guid salonId, Guid excludingArtistId);
        
        /// <summary>
        /// Tìm 1 Thợ phụ rảnh tại Salon (ConcurrentCapacity > 0) để có thể assign thay thế đè ca
        /// </summary>
        Task<NailArtist?> GetAvailableAlternativeArtistAsync(Guid salonId, Guid excludeArtistId, DateTime date, TimeSpan startTime, int durationMinutes);
    }
}
