using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs
{
    public class SalonResponseDTO : IMapFrom<Salon>
    {
        public Guid SalonId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public List<SalonOperatingHourResponseDTO.SalonOperatingHourResponseDTO> OperatingHours { get; set; } = new();

    }
}
