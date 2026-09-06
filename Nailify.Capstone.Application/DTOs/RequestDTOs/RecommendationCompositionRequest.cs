namespace Nailify.Capstone.Application.DTOs.RequestDTOs
{
    public class RecommendationCompositionRequest
    {
        public string? SkinTone { get; set; }
        public string? SkinShade { get; set; }
        public string? HandShape { get; set; }
        public string? Occupation { get; set; }
        public string? NailCondition { get; set; }
        public List<string> PreferredColors { get; set; } = new List<string>();
        public List<string> PreferredStyles { get; set; } = new List<string>();
        public List<string> PreferredOccasions { get; set; } = new List<string>();
        public int? PreferredNailShapeId { get; set; }
        public string? PreferredComplexity { get; set; }
    }
}
