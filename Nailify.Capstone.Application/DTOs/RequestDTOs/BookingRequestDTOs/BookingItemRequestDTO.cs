using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class BookingItemRequestDTO : IMapFrom<BookingItem>
    {
        public int? NailVariantId { get; set; }
        public Guid? ServiceId { get; set; }
        public int Quantity { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BookingItemRequestDTO, BookingItem>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.BookingItemId, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }
}
