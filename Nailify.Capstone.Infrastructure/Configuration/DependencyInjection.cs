using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Infrastructure.DBContext;
using Nailify.Capstone.Infrastructure.Repository;
using System.Reflection;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureToApplication(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            services.AddDbContext<NailifyDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly(
                        typeof(NailifyDbContext).Assembly.FullName)
                )
            );

            // Đăng ký Unit of Work & Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryTypeRepository, CategoryTypeRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<INailDesignRepository, NailDesignRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Đăng ký Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryTypeService, CategoryTypeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<INailDesignService, NailDesignService>();

            // Đăng ký FluentValidation từ tầng Application
            services.AddValidatorsFromAssembly(typeof(Nailify.Capstone.Application.Validation.UserRequestDTOs.UserRegisterRequestValidator).Assembly);

            // Đăng ký AutoMapper
            services.AddAutoMapper(typeof(Nailify.Capstone.Application.Mapping.MappingProfile).Assembly);

            return services;
        }
    }
}
