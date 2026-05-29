using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Infrastructure.DBContext
{
    public class NailifyDbContext : DbContext
    {
        public NailifyDbContext()
        {
        }
        public NailifyDbContext(DbContextOptions<NailifyDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public static string GetConnectionString(string connectionStringName)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString(connectionStringName);
            return connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(GetConnectionString("DefaultConnection"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }
    }
}
