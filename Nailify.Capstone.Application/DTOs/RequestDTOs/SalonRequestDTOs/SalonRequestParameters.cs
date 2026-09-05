using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs
{
    public class SalonRequestParameters
    {
        private const int maxPageSize = 50;
        private int _pageNumber = 1;
        private int _pageSize = 10;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 0 && value <= maxPageSize) ? value : maxPageSize;
        }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public SalonStatusFilter? Status { get; set; }
        public string? OrderBy { get; set; }
    }
}
