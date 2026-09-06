using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class BookingSkillMatchingService : IBookingSkillMatchingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingSkillMatchingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<bool> HasRequiredSkillsAsync(NailArtist candidate, Booking booking, Guid? originalArtistId = null)
        {
            return HasRequiredSkillsAsync(candidate, booking.BookingItems, originalArtistId);
        }

        public async Task<bool> HasRequiredSkillsAsync(NailArtist candidate, IEnumerable<BookingItem> bookingItems, Guid? originalArtistId = null)
        {
            var candidateSkills = candidate.NailArtistSkills?.ToDictionary(x => x.SkillTypeId, x => x.Level)
                                  ?? new Dictionary<Guid, int>();

            foreach (var item in bookingItems)
            {
                if (item.NailVariantId.HasValue)
                {
                    var requiredSkills = await _unitOfWork.NailRequiredSkillRepository.GetSkillsByDesignIdAsync(item.NailVariantId.Value);
                    foreach (var required in requiredSkills)
                    {
                        if (!candidateSkills.TryGetValue(required.SkillTypeId, out var candidateLevel)
                            || candidateLevel < required.RequiredLevel)
                        {
                            return false;
                        }
                    }
                }

                if (item.CustomerNailRequestId.HasValue && originalArtistId.HasValue)
                {
                    var originalArtistSkills = await _unitOfWork.NailArtistSkillRepository.GetSkillsByArtistIdAsync(originalArtistId.Value);
                    foreach (var originalSkill in originalArtistSkills)
                    {
                        if (!candidateSkills.TryGetValue(originalSkill.SkillTypeId, out var candidateLevel)
                            || candidateLevel < originalSkill.Level)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
