using Nailify.Capstone.Application.Interfaces.MappingInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.ServiceResponseDTOs
{
    public class ServiceResponseDTO : IMapFrom<Nailify.Capstone.Domain.Entities.Services>
    {
        public Guid ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }
}
