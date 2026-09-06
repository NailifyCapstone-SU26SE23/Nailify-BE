using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ChairRequestDTOs
{
    public class ChairUpdateRequest : IMapFrom<Chair>
    {
        [Required(ErrorMessage = "Tên ghế là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "Tên ghế không được vượt quá 100 ký tự.")]
        public string ChairName { get; set; } = null!;

        [Required(ErrorMessage = "Trạng thái ghế là bắt buộc.")]
        [RegularExpression("^(Active|Maintenance|Inactive)$", ErrorMessage = "Trạng thái phải là Active, Maintenance hoặc Inactive.")]
        public string Status { get; set; } = "Active";

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ChairUpdateRequest, Chair>()
                   .ForMember(dest => dest.ChairId, opt => opt.Ignore())
                   .ForMember(dest => dest.SalonId, opt => opt.Ignore())
                   .ForMember(dest => dest.Salon, opt => opt.Ignore())
                   .ForMember(dest => dest.Bookings, opt => opt.Ignore());
        }
    }
}
