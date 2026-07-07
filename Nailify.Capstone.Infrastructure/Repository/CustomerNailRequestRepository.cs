using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class CustomerNailRequestRepository : GenericRepository<CustomerNailRequest>, ICustomerNailRequestRepository
    {
        public CustomerNailRequestRepository(NailifyDbContext context) : base(context)
        {
        }
        public async Task<PagedList<CustomerNailRequest>> GetPagedCustomerNailRequestsAsync(
            int pageNumber, int pageSize, Guid? salonId = null, CustomerNailStatus? status = null, Guid? customerId = null, Guid? approvedArtistId = null)
        {
            var query = FindByCondition(x => true)
                       .Include(x => x.CustomerNail)
                       .Include(x => x.Salon)
                       .Include(x => x.ApprovedArtist)
                       .AsQueryable();
            if (salonId.HasValue)
                query = query.Where(x => x.SalonId == salonId.Value);
            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);
            if (customerId.HasValue)
                query = query.Where(x => x.CustomerNail.UserId == customerId.Value);
            if (approvedArtistId.HasValue)
                query = query.Where(x => x.ApprovedArtistId == approvedArtistId.Value);
            query = query.OrderByDescending(x => x.CreatedAt);
            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<CustomerNailRequest>(items, totalItems, pageNumber, pageSize);
        }
        public async Task<CustomerNailRequest?> GetCustomerNailRequestDetailAsync(Guid requestId)
        {
            return await FindByCondition(r => r.CustomerNailRequestId == requestId)
                .Include(r => r.CustomerNail)
                    .ThenInclude(cn => cn.CustomerNailComponents)
                        .ThenInclude(c => c.Component)
                .Include(r => r.Salon)
                .Include(r => r.ApprovedArtist)
                    .ThenInclude(a => a.Account)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerNailRequest?> GetApprovedRequestAsync(int customerNailId, Guid salonId)
        {
            return await FindByCondition(r =>
                r.CustomerNailId == customerNailId &&
                r.SalonId == salonId &&
                r.Status == CustomerNailStatus.Approved || r.Status == CustomerNailStatus.Quoted)
                .FirstOrDefaultAsync();
        }


        public async Task<CustomerNailRequest?> GetAnyApprovedRequestAsync(int customerNailId)
        {
            return await FindByCondition(r =>
                r.CustomerNailId == customerNailId &&
                r.Status == CustomerNailStatus.Approved)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
