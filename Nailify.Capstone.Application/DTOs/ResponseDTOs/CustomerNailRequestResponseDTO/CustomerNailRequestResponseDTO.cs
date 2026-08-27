using AutoMapper;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.CustomerNailRequestResponseDTO
{
    public class CustomerNailRequestResponseDTO : IMapFrom<CustomerNailRequest>
    {
        public Guid CustomerNailRequestId { get; set; }
        public int CustomerNailId { get; set; }
        public Guid SalonId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectReason { get; set; }
        public Guid? ApprovedArtistId { get; set; }
        public decimal? Price { get; set; }
        public int? Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? ArtistFullName { get; set; }
        public string? SalonName { get; set; }
        public bool IsCustomerRequest { get; set; }
        public CustomerNailDto? CustomerNail { get; set; }
        public SalonResponseDTO? Salon { get; set; }
        public NailArtistResponseDTO? ApprovedArtist { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerNailRequest, CustomerNailRequestResponseDTO>()
                   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                   .ForMember(dest => dest.ArtistFullName, opt => opt.MapFrom(src =>
                       src.ApprovedArtist != null && src.ApprovedArtist.Account != null
                       ? $"{src.ApprovedArtist.Account.FirstName} {src.ApprovedArtist.Account.LastName}"
                       : null))
                   .ForMember(dest => dest.SalonName, opt => opt.MapFrom(src =>
                       src.Salon != null ? src.Salon.Name : null));
        }
    }
}
