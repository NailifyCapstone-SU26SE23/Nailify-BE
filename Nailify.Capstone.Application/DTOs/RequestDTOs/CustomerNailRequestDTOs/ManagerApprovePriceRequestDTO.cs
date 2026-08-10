using System;
using System.ComponentModel.DataAnnotations;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs
{
    public class ManagerApprovePriceRequestDTO
    {
        [Required]
        public bool IsApproved { get; set; } // true: Approve Staff's proposed price, false: Manager overrides
        public decimal? OverridePrice { get; set; } // Required if IsApproved == false
        public int? OverrideDuration { get; set; }
    }
}
