using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class ModelTrainingResultDto
    {
        public DateTime TrainedAt { get; set; }
        public int TotalSamples { get; set; }   // số cặp (user, variant) dùng train
        public int UniqueUsers { get; set; }
        public int UniqueVariants { get; set; }
        public double RmseScore { get; set; }   // Root Mean Squared Error (thấp = tốt)
        public string ModelPath { get; set; } = string.Empty;
    }
}
