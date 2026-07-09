using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.ML
{
    public class DurationPredictionInput
    {
        [LoadColumn(0)]
        public float StepsCount { get; set; }
        // Tổng điểm yêu cầu các kỹ năng chuyên môn (Precision, Color, Form, Material, Design)
        [LoadColumn(1)]
        public float RequiredComplexity { get; set; }
        [LoadColumn(2)]
        public float ArtistSpeed { get; set; }
        [LoadColumn(3)]
        public float BaseDuration { get; set; }
        [LoadColumn(4)]
        public float ActualDuration { get; set; }
    }
    public class DurationPredictionOutput
    {
        [ColumnName("Score")]
        public float PredictedDuration { get; set; }
    }
}
