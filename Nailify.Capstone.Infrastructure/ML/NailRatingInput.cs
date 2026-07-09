using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.ML
{
    public class NailRatingInput
    {
        [LoadColumn(0)] public float UserId { get; set; }        // encoded Guid
        [LoadColumn(1)] public float NailVariantId { get; set; } // int cast to float
        [LoadColumn(2)] public float Label { get; set; }         // rating 1.0–5.0
    }
    public class NailRatingPrediction
    {
        public float Label { get; set; }
        public float Score { get; set; }
    }
}
