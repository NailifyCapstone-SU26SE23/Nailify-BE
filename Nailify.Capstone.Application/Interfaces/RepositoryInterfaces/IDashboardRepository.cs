using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetActiveSalonsCountAsync();
        Task<decimal> GetPlatformRevenueAsync(DateTime start, DateTime end);
        Task<int> GetRegisteredCustomersCountAsync();
        Task<int> GetActiveStaffCountAsync();
        Task<double?> GetPlatformAverageRatingAsync();
        Task<int> GetActivePromotionsCountAsync(DateTime now);
        Task<List<(DateTime Date, decimal Total)>> GetRevenueTrendAsync(DateTime start, DateTime end);
        Task<List<(DateTime Date, int Count)>> GetCustomerGrowthAsync(DateTime start, DateTime end);
        Task<List<Booking>> GetPaidBookingsForPeriodAsync(DateTime start, DateTime end);
        Task<List<Transaction>> GetPaidTransactionsForBookingsAsync(IEnumerable<Guid> bookingIds);
        Task<List<BookingRating>> GetSalonRatingsForPeriodAsync(DateTime start, DateTime end);
        Task<List<Booking>> GetNailArtistBookingsForPeriodAsync(Guid artistId, DateTime start, DateTime end);
        Task<double?> GetNailArtistAverageRatingAsync(Guid artistId, DateTime start, DateTime end);
        Task<decimal> GetNailArtistEarningsAsync(IEnumerable<Guid> bookingIds);
        Task<List<(DateTime Date, decimal Total)>> GetNailArtistEarningsTrendAsync(IEnumerable<Guid> bookingIds, DateTime start, DateTime end);
        Task<List<NailArtistBreak>> GetNailArtistBreaksForPeriodAsync(Guid artistId, DateTime start, DateTime end);
        Task<List<NailArtistSkill>> GetNailArtistSkillsAsync(Guid artistId);
        Task<List<BookingRating>> GetNailArtistRecentFeedbackAsync(Guid artistId, int count = 5);

        // Receptionist Dashboard
        Task<List<WalkInQueue>> GetWalkInQueueForDayAsync(Guid salonId, DateTime startOfDay, DateTime endOfDay);
        Task<List<BookingWaitlist>> GetWaitlistForDayAsync(Guid salonId, DateTime startOfDay, DateTime endOfDay);
        Task<List<Booking>> GetTodaysBookingsForSalonAsync(Guid salonId, DateTime startOfDay, DateTime endOfDay);
        Task<List<NailArtist>> GetActiveStaffForSalonAsync(Guid salonId);
        Task<List<Guid>> GetArtistIdsOnBreakAsync(Guid salonId, DateTime date, TimeSpan timeOfDay);
        Task<List<Chair>> GetChairsForSalonAsync(Guid salonId);
        Task<List<NailArtistBreak>> GetSalonBreaksForDayAsync(Guid salonId, DateTime startOfDay, DateTime endOfDay);

        // Salon Manager Dashboard
        Task<List<Booking>> GetSalonBookingsForPeriodAsync(Guid salonId, DateTime start, DateTime end);
        Task<int> GetSalonActiveStaffCountAsync(Guid salonId);
        Task<List<NailArtistBreak>> GetSalonBreaksForPeriodAsync(Guid salonId, DateTime start, DateTime end);
    }
}
