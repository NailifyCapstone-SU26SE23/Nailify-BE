using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Infrastructure.DBContext;
using Nailify.Capstone.Infrastructure.Repository;
using Nailify.Capstone.Infrastructure.Service;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
            services.AddScoped<ICategoryTypeRepository, CategoryTypeRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<INailDesignRepository, NailDesignRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ISalonRepository, SalonRepository>();
            services.AddScoped<INailArtistRepository, NailArtistRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();

            // Đăng ký Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryTypeService, CategoryTypeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<INailDesignService, NailDesignService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ISalonService, SalonService>();
            services.AddScoped<INailArtistService, NailArtistService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<CloudinaryService>();

            // Đăng ký Cloudinary Configuration
            var cloudinarySettings = configuration.GetSection("CloudinarySettings")
                                                  .Get<CloudinaryConfiguration>();
            if (cloudinarySettings != null)
            {
                services.AddSingleton<ICloudinaryConfiguration>(cloudinarySettings);
            }

            // Đăng ký FluentValidation từ tầng Application
            services.AddValidatorsFromAssembly(typeof(Nailify.Capstone.Application.Validation.UserRequestDTOs.UserRegisterRequestValidator).Assembly);

            // Đăng ký AutoMapper
            services.AddAutoMapper(typeof(Nailify.Capstone.Application.Mapping.MappingProfile).Assembly);

            return services;
        }
    }
}
