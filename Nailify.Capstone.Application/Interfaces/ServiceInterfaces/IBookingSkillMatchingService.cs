using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingSkillMatchingService
    {
        Task<bool> HasRequiredSkillsAsync(NailArtist candidate, Booking booking, Guid? originalArtistId = null);
        Task<bool> HasRequiredSkillsAsync(NailArtist candidate, IEnumerable<BookingItem> bookingItems, Guid? originalArtistId = null);
    }
}
