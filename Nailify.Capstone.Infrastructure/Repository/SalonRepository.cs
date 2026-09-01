using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class SalonRepository : GenericRepository<Salon>, ISalonRepository
    {
        public SalonRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<Salon?> GetSalonWithOperatingHoursAsync(Guid salonId)
        {
            return await _dbSet.Include(x => x.OperatingHours)
                .FirstOrDefaultAsync(x => x.SalonId == salonId);
        }

        public async Task<PagedList<Salon>> GetPagedSalonsAsync(SalonRequestParameters parameters)
        {
            var query = _context.Salons
                .Include(s => s.OperatingHours)
                .Where(s => s.Status == "Open")
                .AsQueryable();

            // Lọc theo Tên
            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                var nameFilter = parameters.Name.Trim().ToLower();
                query = query.Where(s => s.Name != null && s.Name.ToLower().Contains(nameFilter));
            }

            // Lọc theo Địa chỉ
            if (!string.IsNullOrWhiteSpace(parameters.Address))
            {
                var addressFilter = parameters.Address.Trim().ToLower();
                query = query.Where(s => s.Address != null && s.Address.ToLower().Contains(addressFilter));
            }

            // Sắp xếp theo khoảng cách (Gần nhất trước) nếu có truyền tọa độ
            if (parameters.Latitude.HasValue && parameters.Longitude.HasValue)
            {
                double latVal = parameters.Latitude.Value;
                double lonVal = parameters.Longitude.Value;
                double cosFactor = Math.Cos(latVal * Math.PI / 180.0);
                double factor = 12392.1424; // 111.32 * 111.32

                query = query.OrderBy(s => 
                    ((s.Latitude - latVal) * (s.Latitude - latVal) + 
                     (s.Longitude - lonVal) * (s.Longitude - lonVal) * cosFactor * cosFactor) * factor
                );
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageIndex - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedList<Salon>(items, totalItems, parameters.PageIndex, parameters.PageSize);
        }

        public async Task<PagedList<Salon>> GetPagedSalonsAdminAsync(SalonRequestParameters parameters)
        {
            var query = _context.Salons
                .Include(s => s.OperatingHours)
                .AsQueryable();

            // Lọc theo Tên
            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                var nameFilter = parameters.Name.Trim().ToLower();
                query = query.Where(s => s.Name != null && s.Name.ToLower().Contains(nameFilter));
            }

            // Lọc theo Địa chỉ
            if (!string.IsNullOrWhiteSpace(parameters.Address))
            {
                var addressFilter = parameters.Address.Trim().ToLower();
                query = query.Where(s => s.Address != null && s.Address.ToLower().Contains(addressFilter));
            }

            // Sắp xếp theo khoảng cách (Gần nhất trước) nếu có truyền tọa độ
            if (parameters.Latitude.HasValue && parameters.Longitude.HasValue)
            {
                double latVal = parameters.Latitude.Value;
                double lonVal = parameters.Longitude.Value;
                double cosFactor = Math.Cos(latVal * Math.PI / 180.0);
                double factor = 12392.1424; // 111.32 * 111.32

                query = query.OrderBy(s =>
                    ((s.Latitude - latVal) * (s.Latitude - latVal) +
                     (s.Longitude - lonVal) * (s.Longitude - lonVal) * cosFactor * cosFactor) * factor
                );
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageIndex - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedList<Salon>(items, totalItems, parameters.PageIndex, parameters.PageSize);
        }
    }
}
