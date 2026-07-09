using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IDurationPredictionService
    {
        Task<ApiResult<ModelTrainingResultDto>> TrainModelAsync();
        float PredictDuration(float stepsCount, float requiredComplexity, float artistSpeed, float baseDuration);
    }
}
