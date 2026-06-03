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
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<NailShape> NailShapes { get; set; }
        public DbSet<NailSurface> NailSurfaces { get; set; }
        public DbSet<NailVariant> NailVariants { get; set; }
        public DbSet<NailComponent> NailComponents { get; set; }
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

            modelBuilder.Entity<Component>()
                .HasKey(c => c.ComponentId);

            modelBuilder.Entity<Component>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NailShape>()
                .HasKey(ns => ns.NailShapeId);

            modelBuilder.Entity<NailShape>()
                .Property(ns => ns.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NailSurface>()
                .HasKey(ns => ns.NailSurfaceId);

            modelBuilder.Entity<NailSurface>()
                .Property(ns => ns.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NailVariant>()
                .HasKey(nv => nv.NailVariantId);

            modelBuilder.Entity<NailVariant>()
                .Property(nv => nv.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NailVariant>()
                .HasOne(nv => nv.NailDesign)
                .WithMany(nd => nd.NailVariants)
                .HasForeignKey(nv => nv.NailDesignId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NailVariant>()
                .HasOne(nv => nv.NailShape)
                .WithMany(ns => ns.NailVariants)
                .HasForeignKey(nv => nv.NailShapeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NailVariant>()
                .HasOne(nv => nv.NailSurface)
                .WithMany(ns => ns.NailVariants)
                .HasForeignKey(nv => nv.NailSurfaceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NailComponent>()
                .HasKey(nc => nc.NailComponentId);

            modelBuilder.Entity<NailComponent>()
                .Property(nc => nc.PosX)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NailComponent>()
                .Property(nc => nc.PosY)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NailComponent>()
                .Property(nc => nc.FingerIndex)
                .HasComment("-1 = whole hand, 0-9 = specific finger index");

            modelBuilder.Entity<NailComponent>()
                .HasOne(nc => nc.NailVariant)
                .WithMany(nv => nv.NailComponents)
                .HasForeignKey(nc => nc.NailVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NailComponent>()
                .HasOne(nc => nc.Component)
                .WithMany(c => c.NailComponents)
                .HasForeignKey(nc => nc.ComponentId)
                .OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}
