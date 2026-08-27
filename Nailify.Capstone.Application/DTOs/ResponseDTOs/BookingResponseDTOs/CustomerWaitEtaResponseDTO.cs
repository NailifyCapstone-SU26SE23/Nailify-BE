using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class CustomerWaitEtaResponseDTO : IMapFrom<Booking>
    {
        public Guid BookingId { get; set; }
        // Developers 
        public string StatusMessage { get; set; } = string.Empty;
        public int EstimatedWaitMinutes { get; set; }
        /*
        // Cờ xác nhận đền bù
        public bool IsCompensationApplied { get; set; }
        // Loại đền bù
        public string CompensationType { get; set; } = string.Empty;
        */
        // Thông điệp hiển thị cho khách hàng
        public string DisplayMessage { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Booking, CustomerWaitEtaResponseDTO>()
                   .IgnoreAllNonExisting();
        }
    }
}
