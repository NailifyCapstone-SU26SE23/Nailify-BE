using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class ComponentDto : IMapFrom<Component>
    {
        public int ComponentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public ComponentType ComponentType { get; set; }
        public decimal Price { get; set; }
        public int? Duration { get; set; }
    }
}
