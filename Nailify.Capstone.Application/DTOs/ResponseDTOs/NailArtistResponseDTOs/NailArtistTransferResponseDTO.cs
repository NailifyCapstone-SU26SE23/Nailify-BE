using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs
{
    public class NailArtistTransferResponseDTO : IMapFrom<StaffTransfer>
    {
        public Guid StaffTransferId { get; set; }
        public Guid NailArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public Guid FromSalonId { get; set; }
        public string FromSalonName { get; set; } = string.Empty;
        public Guid ToSalonId { get; set; }
        public string ToSalonName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public NailArtistTransferStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<StaffTransfer, NailArtistTransferResponseDTO>()
                .ForMember(d => d.ArtistName, opt => opt.MapFrom(s => s.NailArtist != null && s.NailArtist.Account != null
                    ? s.NailArtist.Account.FirstName + " " + s.NailArtist.Account.LastName : string.Empty))
                .ForMember(d => d.FromSalonName, opt => opt.MapFrom(s => s.FromSalon != null ? s.FromSalon.Name : string.Empty))
                .ForMember(d => d.ToSalonName, opt => opt.MapFrom(s => s.ToSalon != null ? s.ToSalon.Name : string.Empty))
                .IgnoreAllNonExisting();
        }
    }
}
