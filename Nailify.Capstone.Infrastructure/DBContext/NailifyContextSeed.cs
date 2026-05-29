using Microsoft.Extensions.Logging;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.DBContext
{
    public class NailifyContextSeed
    {
        public static async Task SeedProductAsync(NailifyDbContext nailifyDbContext, ILogger logger)
        {

            if (!nailifyDbContext.Users.Any())
            {
                nailifyDbContext.Users.Add(new User
                {
                    Email = "admin@stemotion.com",
                    Password = "123456",
                    FirstName = "System",
                    LastName = "Admin",
                    Phone = "0966340303",
                    AvatarUrl = "default-avatar.png", // Thêm avatarUrl để tránh lỗi NOT NULL constraint
                    Status = "Active",
                });
                await nailifyDbContext.SaveChangesAsync();
                logger.LogInformation("Seed tài khoản Admin mặc định thành công.");
            }
        }
    }
}
