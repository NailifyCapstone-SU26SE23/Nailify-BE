using Nailify.Capstone.Application.DTOs.ResponseDTOs.Dashboard;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<NailArtistDashboardDto> GetNailArtistDashboardDataAsync(Guid artistId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ReceptionistDashboardDto> GetReceptionistDashboardDataAsync(Guid salonId, DateTime? date = null);
        Task<SalonManagerDashboardDto> GetSalonDashboardDataAsync(Guid salonId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
