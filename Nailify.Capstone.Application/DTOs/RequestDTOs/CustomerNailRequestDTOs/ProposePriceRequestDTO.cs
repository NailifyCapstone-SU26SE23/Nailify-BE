using System;
using System.ComponentModel.DataAnnotations;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs
{
    public class ProposePriceRequestDTO
    {
        [Required]
        public decimal ProposedPrice { get; set; }
        public int? ProposedDuration { get; set; }
    }
}
