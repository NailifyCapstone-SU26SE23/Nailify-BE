using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.Extensions;

namespace Nailify.Capstone.Infrastructure.DBContext
{
    public class NailifyDbContext : DbContext
    {
        private readonly IMediator? _mediator;
        public NailifyDbContext()
        {
        }
        public NailifyDbContext(DbContextOptions<NailifyDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }
        #region initial DBSet
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
        public DbSet<Component> Components { get; set; }
        public DbSet<NailShape> NailShapes { get; set; }
        public DbSet<NailSurface> NailSurfaces { get; set; }
        public DbSet<NailVariant> NailVariants { get; set; }
        public DbSet<NailComponent> NailComponents { get; set; }
        public DbSet<CustomerComponent> CustomerComponents { get; set; }
        public DbSet<CustomerNail> CustomerNails { get; set; }
        public DbSet<CustomerNailComponent> CustomerNailComponents { get; set; }
        public DbSet<SkillType> SkillTypes { get; set; }
        public DbSet<NailArtistSkill> NailArtistSkills { get; set; }
        public DbSet<NailRequiredSkill> NailRequiredSkills { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingItem> BookingItems { get; set; }
        public DbSet<BookingHistory> BookingHistories { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<NailProcedure> NailProcedures { get; set; }
        public DbSet<BookingProcedure> BookingProcedures { get; set; }
        public DbSet<FavoriteNail> FavoriteNails { get; set; }
        public DbSet<LoyaltyTier> LoyaltyTiers { get; set; }
        public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }
        #endregion initial DBSet

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
                .Property(nd => nd.MinPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NailDesign>()
                .Property(nd => nd.MaxPrice)
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

            modelBuilder.Entity<CustomerComponent>()
                .HasKey(cc => cc.CustomerComponentId);

            modelBuilder.Entity<CustomerComponent>()
                .Property(cc => cc.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerComponent>()
                .HasOne(cc => cc.User)
                .WithMany()
                .HasForeignKey(cc => cc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerNail>()
                .HasKey(cn => cn.CustomerNailId);

            modelBuilder.Entity<CustomerNail>()
                .Property(cn => cn.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerNail>()
                .HasOne(cn => cn.User)
                .WithMany()
                .HasForeignKey(cn => cn.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerNail>()
                .HasOne(cn => cn.NailShape)
                .WithMany()
                .HasForeignKey(cn => cn.NailShapeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerNail>()
                .HasOne(cn => cn.NailSurface)
                .WithMany()
                .HasForeignKey(cn => cn.NailSurfaceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerNail>()
                .Property(cn => cn.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            modelBuilder.Entity<CustomerNail>()
                .HasOne(cn => cn.ApprovedArtist)
                .WithMany()
                .HasForeignKey(cn => cn.ApprovedArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerNailComponent>()
                .HasKey(cnc => cnc.CustomerNailComponentId);

            modelBuilder.Entity<CustomerNailComponent>()
                .Property(cnc => cnc.PosX)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerNailComponent>()
                .Property(cnc => cnc.PosY)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerNailComponent>()
                .Property(cnc => cnc.FingerIndex)
                .HasComment("-1 = whole hand, 0-9 = specific finger index");

            modelBuilder.Entity<FavoriteNail>()
                .HasIndex(f => new { f.UserId, f.NailDesignId, f.NailVariantId })
                .IsUnique();

            modelBuilder.Entity<FavoriteNail>()
                .ToTable(table => table.HasCheckConstraint(
                    "CK_FavoriteNail_DesignOrVariant",
                    "\"NailDesignId\" IS NOT NULL OR \"NailVariantId\" IS NOT NULL"));

            modelBuilder.Entity<FavoriteNail>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteNail>()
                .HasOne(f => f.NailDesign)
                .WithMany()
                .HasForeignKey(f => f.NailDesignId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteNail>()
                .HasOne(f => f.NailVariant)
                .WithMany()
                .HasForeignKey(f => f.NailVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            // In DbContext
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.LoyaltyTier)
                .WithMany(lt => lt.Customers)
                .HasForeignKey(c => c.LoyaltyTierId)
                .OnDelete(DeleteBehavior.SetNull); // Keep customer if tier is deleted

            modelBuilder.Entity<LoyaltyTier>()
                .HasIndex(lt => lt.MinLifetimePoints)
                .IsUnique(); // Ensure unique point thresholds

            modelBuilder.Entity<LoyaltyTransaction>()
                .HasKey(lt => lt.LoyaltyTransactionId);

            modelBuilder.Entity<LoyaltyTransaction>()
                .Property(lt => lt.TransactionType)
                .HasConversion<string>()
                .HasDefaultValue(LoyaltyTransactionType.Earned);

            modelBuilder.Entity<LoyaltyTransaction>()
                .HasIndex(lt => lt.BookingId)
                .IsUnique();

            modelBuilder.Entity<LoyaltyTransaction>()
                .HasOne(lt => lt.Customer)
                .WithMany(c => c.LoyaltyTransactions)
                .HasForeignKey(lt => lt.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoyaltyTransaction>()
                .HasOne(lt => lt.Booking)
                .WithMany()
                .HasForeignKey(lt => lt.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LoyaltyTransaction>()
                .HasOne(lt => lt.LoyaltyTier)
                .WithMany()
                .HasForeignKey(lt => lt.LoyaltyTierIdAtTime)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingId);
                entity.Property(b => b.TotalPrice).HasPrecision(18, 2);
                entity.Property(b => b.Status)
                    .HasConversion(
                            v => v.ToString(),
                            v => (BookingStatus)Enum.Parse(typeof(BookingStatus), v))
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Services>(entity =>
            {
                entity.HasKey(s => s.ServiceId);
                entity.Property(s => s.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<BookingItem>(entity =>
            {
                entity.HasKey(bi => bi.BookingItemId);
                entity.Property(bi => bi.Price).HasPrecision(18, 2);
                // Thiết lập các mối quan hệ khóa ngoại
                entity.HasOne(bi => bi.Booking)
                      .WithMany(b => b.BookingItems)
                      .HasForeignKey(bi => bi.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(bi => bi.Service)
                      .WithMany(s => s.BookingItems)
                      .HasForeignKey(bi => bi.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(bi => bi.NailVariant)
                      .WithMany()
                      .HasForeignKey(bi => bi.NailVariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<BookingHistory>(entity =>
            {
                entity.HasKey(bh => bh.BookingHistoryId);

                entity.HasOne(bh => bh.Booking)
                      .WithMany(b => b.BookingHistories)
                      .HasForeignKey(bh => bh.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            /*
            modelBuilder.Entity<BookingItem>().HasKey(bi => bi.BookingItemId);
            modelBuilder.Entity<BookingHistory>().HasKey(bh => bh.BookingHistoryId);
            modelBuilder.Entity<BookingItem>()
                .HasOne(bi => bi.Booking)
                .WithMany() 
                .HasForeignKey(bi => bi.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Services>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);
            */

            modelBuilder.Entity<CustomerNailComponent>()
                .HasOne(cnc => cnc.CustomerNail)
                .WithMany(cn => cn.CustomerNailComponents)
                .HasForeignKey(cnc => cnc.CustomerNailId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerNailComponent>()
                .HasOne(cnc => cnc.Component)
                .WithMany()
                .HasForeignKey(cnc => cnc.ComponentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerNailComponent>()
                .HasOne(cnc => cnc.CustomerComponent)
                .WithMany(cc => cc.CustomerNailComponents)
                .HasForeignKey(cnc => cnc.CustomerComponentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerNailComponent>()
                .ToTable(table => table.HasCheckConstraint(
                    "CK_CustomerNailComponent_OneComponent",
                    "(\"ComponentId\" IS NOT NULL AND \"CustomerComponentId\" IS NULL) OR (\"ComponentId\" IS NULL AND \"CustomerComponentId\" IS NOT NULL)"));

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

            modelBuilder.Entity<User>()
                .HasOne(u => u.Salon)
                .WithMany()
                .HasForeignKey(u => u.SalonId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                        .Property(u => u.Role)
                        .HasConversion<string>();


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

            modelBuilder.Entity<SkillType>()
                        .HasKey(st => st.SkillTypeId);

            modelBuilder.Entity<NailArtistSkill>()
                        .HasKey(nas => nas.NailArtistSkillId);
            modelBuilder.Entity<NailArtistSkill>()
                        .HasIndex(nas => new { nas.NailArtistId, nas.SkillTypeId })
                        .IsUnique(); // Mỗi thợ chỉ có 1 record/skill
            modelBuilder.Entity<NailArtistSkill>()
                        .HasOne(nas => nas.NailArtist)
                        .WithMany(na => na.NailArtistSkills)
                        .HasForeignKey(nas => nas.NailArtistId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<NailArtistSkill>()
                        .HasOne(nas => nas.SkillType)
                        .WithMany(st => st.NailArtistSkills)
                        .HasForeignKey(nas => nas.SkillTypeId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NailRequiredSkill>()
                        .HasKey(nrs => nrs.NailRequiredSkillId);
            modelBuilder.Entity<NailRequiredSkill>()
                        .HasIndex(nrs => new { nrs.NailVariantId, nrs.SkillTypeId })
                        .IsUnique(); // Mỗi design chỉ yêu cầu 1 level/skill
            modelBuilder.Entity<NailRequiredSkill>()
                        .HasOne(nrs => nrs.NailVariant)
                        .WithMany(nv => nv.NailRequiredSkills)
                        .HasForeignKey(nrs => nrs.NailVariantId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<NailRequiredSkill>()
                        .HasOne(nrs => nrs.SkillType)
                        .WithMany(st => st.NailRequiredSkills)
                        .HasForeignKey(nrs => nrs.SkillTypeId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingItem>()
                        .HasOne(bi => bi.CustomerNail)
                        .WithMany()
                        .HasForeignKey(bi => bi.CustomerNailId)
                        .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Procedure>(entity =>
            {
                entity.HasKey(p => p.ProcedureId);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            });
            modelBuilder.Entity<NailProcedure>(entity =>
            {
                entity.HasKey(np => np.NailProcedureId);

                entity.HasOne(np => np.NailVariant)
                      .WithMany(nv => nv.NailProcedures)
                      .HasForeignKey(np => np.NailVariantId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(np => np.Procedure)
                      .WithMany(p => p.NailProcedures)
                      .HasForeignKey(np => np.ProcedureId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<BookingProcedure>(entity =>
            {
                entity.HasKey(bp => bp.BookingProcedureId);

                entity.Property(bp => bp.ProcedureName).IsRequired().HasMaxLength(200);

                entity.Property(bp => bp.Status)
                      .HasConversion(
                          v => v.ToString(),
                          v => (BookingProcedureStatus)Enum.Parse(typeof(BookingProcedureStatus), v))
                      .HasMaxLength(20);
                entity.HasOne(bp => bp.BookingItem)
                      .WithMany(bi => bi.BookingProcedures)
                      .HasForeignKey(bp => bp.BookingItemId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(bp => bp.Procedure)
                      .WithMany()
                      .HasForeignKey(bp => bp.ProcedureId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(bp => bp.CompletedBy)
                      .WithMany()
                      .HasForeignKey(bp => bp.CompletedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            ConfigureStatusDefaults(modelBuilder);
        }

        private static void ConfigureStatusDefaults(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var statusProperty = entityType.ClrType.GetProperty("Status",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.IgnoreCase);

                if (statusProperty?.PropertyType == typeof(string))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<string>(statusProperty.Name)
                        .HasDefaultValue("Active");
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch domain events TRƯỚC khi commit:
            // → Handler (vd: BookingStatusChangedEventHandler) sẽ track BookingHistory vào cùng DbContext này
            // → base.SaveChangesAsync() commit tất cả trong 1 transaction duy nhất
            // → Nếu lỗi xảy ra → cả Booking state lẫn BookingHistory đều rollback, không mất data
            if (_mediator != null)
            {
                await _mediator.DispatchDomainEventsAsync(this);
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
