using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class MatchedCharacteristicDTO
    {
        public string Category { get; set; } = string.Empty; // Color, Style, Occasion, Shape, SkinTone, HandShape, Complexity
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsMatchingPreference { get; set; }
    }
}
