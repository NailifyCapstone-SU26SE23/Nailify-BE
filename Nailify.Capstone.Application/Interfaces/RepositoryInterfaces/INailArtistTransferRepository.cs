using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface INailArtistTransferRepository : IGenericRepository<StaffTransfer>
    {
        /// <summary>
        /// Transfer đang hiệu lực của thợ tại 1 ngày (Scheduled và StartDate <= date <= EndDate)
        /// </summary>
        /// <param name="artistId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<StaffTransfer?> GetActiveTransferByArtistAndDateAsync(Guid artistId, DateTime date);
        /// <summary>
        /// Các transfer tăng cường vào salon tại 1 ngày
        /// </summary>
        /// <param name="salonId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<List<StaffTransfer>> GetTransfersIntoSalonByDateAsync(Guid salonId, DateTime date);
        /// Danh sách ArtistId bị điều RA KHỎI salon tại 1 ngày (để ẩn khỏi salon gốc)
        Task<List<NailArtist>> GetTransferredOutArtistIdsAsync(Guid salonId, DateTime date);
        /// Kiểm tra thợ đã có transfer Scheduled trùng khoảng ngày chưa
        Task<bool> HasOverlappingTransferAsync(Guid artistId, DateTime startDate, DateTime endDate);
        /// Danh sách transfer phân trang cho manager
        Task<PagedList<StaffTransfer>> GetPagedTransferAsync(int pageNumber, int pageSize, Guid? salonId, Guid? artistId, NailArtistTransferStatus? status);
    }
}
