using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
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
        public static async Task SeedProductAsync(NailifyDbContext nailifyDbContext, ILogger logger, IPasswordHasher passwordHasher)
        {

            if (!nailifyDbContext.Users.Any(u => u.Email == "admin@nailify.com"))
            {
                nailifyDbContext.Users.Add(new User
                {
                    Email = "admin@nailify.com",
                    Password = passwordHasher.HashPassword("123456"),
                    FirstName = "System",
                    LastName = "Admin",
                    Phone = "0966340303",
                    AvatarUrl = "default-avatar.png",
                    Role = Nailify.Capstone.Domain.Enums.UserRole.Admin,
                    Status = "Active",
                });
                await nailifyDbContext.SaveChangesAsync();
                logger.LogInformation("Seed tài khoản Admin mặc định thành công.");
            }
        }
    }
}
