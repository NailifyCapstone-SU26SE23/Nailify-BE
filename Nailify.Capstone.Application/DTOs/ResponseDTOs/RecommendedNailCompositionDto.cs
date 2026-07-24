namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class RecommendedNailCompositionDto
    {
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public List<int> ComponentIds { get; set; } = new List<int>();
        public NailShapeDto? NailShape { get; set; }
        public NailSurfaceDto? NailSurface { get; set; }
        public List<ComponentDto> Components { get; set; } = new List<ComponentDto>();
        public List<string> Colors { get; set; } = new List<string>();
    }
}
