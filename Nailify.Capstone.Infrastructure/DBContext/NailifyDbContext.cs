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
        public DbSet<CategoryType> CategoryTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<NailDesign> NailDesigns { get; set; }
        public DbSet<NailCategory> NailCategories { get; set; }
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NailCategory>()
                .HasKey(nc => nc.NailCategoryId);

            modelBuilder.Entity<NailCategory>()
                .HasIndex(nc => new { nc.NailDesignId, nc.CategoryId })
                .IsUnique();

            modelBuilder.Entity<NailCategory>()
                .HasOne(nc => nc.NailDesign)
                .WithMany(nd => nd.NailCategories)
                .HasForeignKey(nc => nc.NailDesignId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NailCategory>()
                .HasOne(nc => nc.Category)
                .WithMany(c => c.NailCategories)
                .HasForeignKey(nc => nc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NailDesign>()
                .Property(nd => nd.Price)
                .HasPrecision(18, 2);
        }
    }
}
