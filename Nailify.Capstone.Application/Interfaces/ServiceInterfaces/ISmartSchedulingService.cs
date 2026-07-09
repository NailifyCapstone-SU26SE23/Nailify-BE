using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ISmartSchedulingService
    {
        Task<ApiResult<List<SmartSlotDto>>> GetSmartSlotAsync(Guid salonId, DateTime date, List<BookingProcedure> procedures);
    }
}
