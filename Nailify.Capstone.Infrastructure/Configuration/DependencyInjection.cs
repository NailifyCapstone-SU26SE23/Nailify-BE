using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Infrastructure.DBContext;
using Nailify.Capstone.Infrastructure.Repository;
using Nailify.Capstone.Infrastructure.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureToApplication(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            // cấu hình JWT Authentication
            var jwtSection = configuration.GetSection("Jwt");
            services.Configure<JwtOptions>(jwtSection); // Đăng ký để các class như JwtProvider có thể dùng IOptions

            // Ánh xạ ngầm cấu hình ra dạng Object để nạp cho JwtBearer lúc Startup hệ thống
            var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

            // 2. CẤU HÌNH XÁC THỰC SỬ DỤNG THUỘC TÍNH ĐỊNH DANH (Dùng jwtOptions thay vì configuration thô)
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    NameClaimType = JwtRegisteredClaimNames.Email,
                    RoleClaimType = "role"
                };
            });
            // Cấu hình DbContext với PostgreSQL
            services.AddDbContext<NailifyDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly(
                        typeof(NailifyDbContext).Assembly.FullName)
                )
            );

            //auth
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IAuthService, AuthService>();
            // Đăng ký Unit of Work & Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICategoryTypeRepository, CategoryTypeRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<INailDesignRepository, NailDesignRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ISalonRepository, SalonRepository>();
            services.AddScoped<INailArtistRepository, NailArtistRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<INailShapeRepository, NailShapeRepository>();
            services.AddScoped<INailSurfaceRepository, NailSurfaceRepository>();
            services.AddScoped<INailVariantRepository, NailVariantRepository>();
            services.AddScoped<INailComponentRepository, NailComponentRepository>();
            services.AddScoped<ICustomerComponentRepository, CustomerComponentRepository>();
            services.AddScoped<ICustomerNailRepository, CustomerNailRepository>();
            services.AddScoped<ICustomerNailComponentRepository, CustomerNailComponentRepository>();
            services.AddScoped<ISkillTypeRepository, SkillTypeRepository>();
            services.AddScoped<INailArtistSkillRepository, NailArtistSkillRepository>();
            services.AddScoped<INailRequiredSkillRepository, NailRequiredSkillRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingItemRepository, BookingItemRepository>();
            services.AddScoped<IBookingHistoryRepository, BookingHistoryRepository>();
            services.AddScoped<IServicesRepository, ServicesRepository>();
            services.AddScoped<IProcedureRepository, ProcedureRepository>();
            services.AddScoped<INailProcedureRepository, NailProcedureRepository>();
            services.AddScoped<IBookingProcedureRepository, BookingProcedureRepository>();
            services.AddScoped<IFavoriteNailRepository, FavoriteNailRepository>();
            services.AddScoped<ILoyaltyTierRepository, LoyaltyTierRepository>();
            services.AddScoped<ILoyaltyTransactionRepository, LoyaltyTransactionRepository>();
            services.AddScoped<IBookingRatingRepository, BookingRatingRepository>();
            // Đăng ký Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryTypeService, CategoryTypeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<INailDesignService, NailDesignService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ISalonService, SalonService>();
            services.AddScoped<INailArtistService, NailArtistService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IComponentService, ComponentService>();
            services.AddScoped<INailShapeService, NailShapeService>();
            services.AddScoped<INailSurfaceService, NailSurfaceService>();
            services.AddScoped<INailVariantService, NailVariantService>();
            services.AddScoped<INailComponentService, NailComponentService>();
            services.AddScoped<ICustomerComponentService, CustomerComponentService>();
            services.AddScoped<ICustomerNailService, CustomerNailService>();
            services.AddScoped<ICustomerNailRequestsService, CustomerNailRequestsService>();
            services.AddScoped<ICustomerNailComponentService, CustomerNailComponentService>();
            services.AddScoped<ISkillTypeService, SkillTypeService>();
            services.AddScoped<INailArtistSkillService, NailArtistSkillService>();
            services.AddScoped<INailRequiredSkillService, NailRequiredSkillService>();
            services.AddScoped<CloudinaryService>();
            services.AddScoped<IQRService, QRService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingHistoryService, BookingHistoryService>();
            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<IBookingProcedureService, BookingProcedureService>();
            services.AddScoped<IProcedureService, ProcedureService>();
            services.AddScoped<IFavoriteNailService, FavoriteNailService>();
            services.AddScoped<ILoyaltyTierService, LoyaltyTierService>();
            services.AddScoped<ILoyaltyTransactionService, LoyaltyTransactionService>();
            services.AddScoped<IBookingRatingService, BookingRatingService>();
            services.AddScoped<ISlotHoldService, SlotHoldService>();
            services.AddScoped<ICustomerNailRequestsService, CustomerNailRequestsService>();
            // Đăng ký Cloudinary Configuration
            var cloudinarySettings = configuration.GetSection("CloudinarySettings")
                                                  .Get<CloudinaryConfiguration>();
            if (cloudinarySettings != null)
            {
                services.AddSingleton<ICloudinaryConfiguration>(cloudinarySettings);
            }

            var slotHoldSettings = configuration.GetSection("SlotHoldSettings")
                                                  .Get<SlotHoldConfiguration>()
                                   ?? new SlotHoldConfiguration();
            services.AddSingleton<ISlotHoldConfiguration>(slotHoldSettings);

            var redisSettings = configuration.GetSection("Redis")
                                             .Get<RedisConfiguration>()
                                ?? new RedisConfiguration { UseMemoryCache = true };
            services.AddSingleton<IRedisConfiguration>(redisSettings);

            if (redisSettings.UseMemoryCache)
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                   options.Configuration = redisSettings?.ConnectionString;
                   options.InstanceName = redisSettings?.InstanceName;
                });
            }

            // Đăng ký FluentValidation từ tầng Application
            services.AddValidatorsFromAssembly(typeof(Nailify.Capstone.Application.Validation.UserRequestDTOs.UserRegisterRequestValidator).Assembly);

            // Đăng ký AutoMapper
            services.AddAutoMapper(typeof(Nailify.Capstone.Application.Mapping.MappingProfile).Assembly);

            // Đăng ký MediatR cho Assembly chứa BookingService (Tầng Application)
            services.AddMediatR(typeof(Nailify.Capstone.Application.Services.BookingService).Assembly);


            return services;
        }
    }
}
