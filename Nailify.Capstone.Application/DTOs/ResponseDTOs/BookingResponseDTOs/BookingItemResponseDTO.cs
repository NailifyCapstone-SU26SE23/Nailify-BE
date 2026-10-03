using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class BookingItemResponseDTO : IMapFrom<BookingItem>
    {
        public Guid BookingItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int? NailVariantId { get; set; }
        public string NailVariantName { get; set; } = string.Empty;
        public string NailVariantImageUrl { get; set; } = string.Empty;
        public int? ShapeMethodConfigId { get; set; }
        public string ShapeMethodConfigName { get; set; } = string.Empty;
        public Guid? CustomerNailRequestId { get; set; }
        public int? CustomerNailId { get; set; }
        public string CustomerNailName { get; set; } = string.Empty;
        public string CustomerNailImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? EstimatedStartTime { get; set; }
        public string? EstimatedEndTime { get; set; }
        public string? ActualStartTime { get; set; }
        public string? ActualEndTime { get; set; }
        public string TimeRangeDisplay { get; set; } = string.Empty;
        public string FullItemSummary { get; set; } = string.Empty;
        public void Mapping(Profile profile)
        {
            profile.CreateMap<BookingItem, BookingItemResponseDTO>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : ""))
                .ForMember(dest => dest.NailVariantName, opt => opt.MapFrom(src => src.NailVariant != null ? src.NailVariant.Name : ""))
                .ForMember(dest => dest.NailVariantImageUrl, opt => opt.MapFrom(src => src.NailVariant != null ? src.NailVariant.ImageUrl : ""))
                .ForMember(dest => dest.ShapeMethodConfigName, opt => opt.MapFrom(src => src.ShapeMethodConfig != null ? src.ShapeMethodConfig.Name : ""))
                .ForMember(dest => dest.CustomerNailId, opt => opt.MapFrom(src => src.CustomerNailRequest != null ? src.CustomerNailRequest.CustomerNailId : (int?)null))
                   .ForMember(dest => dest.CustomerNailName, opt => opt.MapFrom(src => src.CustomerNailRequest != null && src.CustomerNailRequest.CustomerNail != null ? src.CustomerNailRequest.CustomerNail.Name : ""))
                .ForMember(dest => dest.CustomerNailImageUrl, opt => opt.MapFrom(src => src.CustomerNailRequest != null && src.CustomerNailRequest.CustomerNail != null ? src.CustomerNailRequest.CustomerNail.ImageUrl : ""))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => GetItemName(src)))
                .ForMember(dest => dest.EstimatedStartTime, opt => opt.MapFrom(src => FormatTime(GetEstimatedStartTime(src))))
                .ForMember(dest => dest.EstimatedEndTime, opt => opt.MapFrom(src => FormatTime(GetEstimatedEndTime(src))))
                .ForMember(dest => dest.ActualStartTime, opt => opt.MapFrom(src => FormatTime(GetActualStartTime(src))))
                .ForMember(dest => dest.ActualEndTime, opt => opt.MapFrom(src => FormatTime(GetActualEndTime(src))))
                .ForMember(dest => dest.TimeRangeDisplay, opt => opt.MapFrom(src => GetTimeRangeDisplay(src)))
                .ForMember(dest => dest.FullItemSummary, opt => opt.MapFrom(src => GetFullItemSummary(src)));
        }
        private static string? FormatTime(TimeSpan? time)
        {
            return time.HasValue ? time.Value.ToString(@"hh\:mm") : null;
        }

        private static string GetItemName(BookingItem item)
        {
            if (item.NailVariant != null && !string.IsNullOrWhiteSpace(item.NailVariant.Name))
                return item.NailVariant.Name;
            if (item.Service != null && !string.IsNullOrWhiteSpace(item.Service.Name))
                return item.Service.Name;
            if (item.CustomerNailRequest != null && item.CustomerNailRequest.CustomerNail != null && !string.IsNullOrWhiteSpace(item.CustomerNailRequest.CustomerNail.Name))
                return item.CustomerNailRequest.CustomerNail.Name;
            if (item.ShapeMethodConfig != null && !string.IsNullOrWhiteSpace(item.ShapeMethodConfig.Name))
                return item.ShapeMethodConfig.Name;

            return "Dịch vụ/Mẫu móng";
        }

        private static TimeSpan? GetEstimatedStartTime(BookingItem item)
        {
            if (item.BookingProcedures == null || !item.BookingProcedures.Any()) return null;
            var values = item.BookingProcedures.Where(p => p.EstimatedStartTime.HasValue).Select(p => p.EstimatedStartTime!.Value).ToList();
            return values.Any() ? values.Min() : null;
        }

        private static TimeSpan? GetEstimatedEndTime(BookingItem item)
        {
            if (item.BookingProcedures == null || !item.BookingProcedures.Any()) return null;
            var values = item.BookingProcedures.Where(p => p.EstimatedEndTime.HasValue).Select(p => p.EstimatedEndTime!.Value).ToList();
            return values.Any() ? values.Max() : null;
        }

        private static TimeSpan? GetActualStartTime(BookingItem item)
        {
            if (item.BookingProcedures == null || !item.BookingProcedures.Any()) return null;
            var actualStartValues = item.BookingProcedures.Where(p => p.ActualStartTime.HasValue).Select(p => p.ActualStartTime!.Value.TimeOfDay).ToList();
            return actualStartValues.Any() ? actualStartValues.Min() : null;
        }

        private static TimeSpan? GetActualEndTime(BookingItem item)
        {
            if (item.BookingProcedures == null || !item.BookingProcedures.Any()) return null;
            var actualEndValues = item.BookingProcedures.Where(p => p.ActualEndTime.HasValue).Select(p => p.ActualEndTime!.Value.TimeOfDay).ToList();
            return actualEndValues.Any() ? actualEndValues.Max() : null;
        }

        private static string GetTimeRangeDisplay(BookingItem item)
        {
            var start = GetActualStartTime(item) ?? GetEstimatedStartTime(item);
            var end = GetActualEndTime(item) ?? GetEstimatedEndTime(item);

            if (start.HasValue && end.HasValue)
            {
                return $"{start.Value:hh\\:mm} - {end.Value:hh\\:mm}";
            }
            return string.Empty;
        }

        private static string GetFullItemSummary(BookingItem item)
        {
            var name = GetItemName(item);
            var timeRange = GetTimeRangeDisplay(item);
            return string.IsNullOrWhiteSpace(timeRange) ? name : $"{name} ({timeRange})";
        }
    }
}
