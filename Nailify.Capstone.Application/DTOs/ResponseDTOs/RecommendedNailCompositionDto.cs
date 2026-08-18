namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class RecommendedNailCompositionDto
    {
        public int? NailShapeId { get; set; }
        public int? NailSurfaceId { get; set; }
        public NailShapeDto? NailShape { get; set; }
        public NailSurfaceDto? NailSurface { get; set; }
        public List<string> Colors { get; set; } = new List<string>();
    }
}
