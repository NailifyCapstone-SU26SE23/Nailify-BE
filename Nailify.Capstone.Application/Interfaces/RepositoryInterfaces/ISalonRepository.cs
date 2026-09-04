using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ISalonRepository : IGenericRepository<Salon>
    {
        Task<Salon?> GetSalonWithOperatingHoursAsync(Guid salonId);
        Task<PagedList<Salon>> GetPagedSalonsAsync(SalonRequestParameters parameters);
        //Task<PagedList<Salon>> GetPagedSalonsAdminAsync(SalonRequestParameters parameters);
    }
}
