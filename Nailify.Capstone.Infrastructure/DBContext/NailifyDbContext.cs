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
        public DbSet<NailDesignImage> NailDesignImages { get; set; }
        public DbSet<SalonOperatingHour> SalonOperatingHours { get; set; }
        public DbSet<Salon> Salons { get; set; }
        public DbSet<NailArtist> NailArtists { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
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

            modelBuilder.Entity<NailDesignImage>()
                .HasKey(ndi => ndi.NailDesignImageId);

            modelBuilder.Entity<NailDesignImage>()
                .HasOne(ndi => ndi.NailDesign)
                .WithMany(nd => nd.NailDesignImages)
                .HasForeignKey(ndi => ndi.NailDesignId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Salon>()
                .HasKey(s => s.SalonId);

            modelBuilder.Entity<SalonOperatingHour>()
                .HasKey(soh => soh.OperatingHourId);

            modelBuilder.Entity<SalonOperatingHour>()
                .HasOne(soh => soh.Salon)
                .WithMany(s => s.OperatingHours)
                .HasForeignKey(soh => soh.SalonId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<NailArtist>()
                .HasKey(na => na.NailArtistId);

            modelBuilder.Entity<Schedule>()
                .HasKey(s => s.ScheduleId);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.NailArtist)
                .WithMany(na => na.Schedules)
                .HasForeignKey(s => s.NailArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NailArtist>()
                        .HasOne(na => na.Account)
                        .WithOne() // Một User Account chỉ làm một thợ nail (hoặc có thể dùng WithMany tùy thiết kế)
                        .HasForeignKey<NailArtist>(na => na.AccountId)
                        .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Nhiều-1 giữa NailArtist và Salon
            modelBuilder.Entity<NailArtist>()
                        .HasOne(na => na.Salon)
                        .WithMany(s => s.NailArtists)
                        .HasForeignKey(na => na.SalonId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.UserId);

                entity.Property(c => c.LoyaltyPoint).HasDefaultValue(0);
                entity.Property(c => c.SkinTone).HasDefaultValue(string.Empty).HasMaxLength(100);
                entity.Property(c => c.Occupation).HasDefaultValue(string.Empty).HasMaxLength(250);
                entity.Property(c => c.NailCondition).HasDefaultValue(string.Empty).HasMaxLength(500);
                entity.Property(c => c.PersonaId).HasDefaultValue(string.Empty).HasMaxLength(100);

                entity.HasOne(c => c.User)
                      .WithOne() // Hoặc .WithOne(u => u.Customer) nếu khai báo Customer trong lớp User
                      .HasForeignKey<Customer>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
