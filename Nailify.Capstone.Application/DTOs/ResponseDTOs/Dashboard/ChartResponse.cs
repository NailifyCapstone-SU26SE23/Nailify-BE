using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.Dashboard
{
    public class ChartResponse<T>
    {
        public List<string> Labels { get; set; } = new();
        public List<ChartDataset<T>> Datasets { get; set; } = new();
    }

    public class ChartDataset<T>
    {
        public string Label { get; set; } = string.Empty;
        public List<T> Data { get; set; } = new();
    }
}
