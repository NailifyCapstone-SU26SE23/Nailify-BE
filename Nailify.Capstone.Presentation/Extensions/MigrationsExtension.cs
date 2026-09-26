using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Nailify.Capstone.Presentation.Extensions
{
    public static class MigrationsExtension
    {
        public static IHost ApplyMigrations(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetService<ILogger<NailifyDbContext>>();

            try
            {
                var dbContext = services.GetRequiredService<NailifyDbContext>();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while applying EF Core database migrations. Application startup will continue.");
            }

            return host;
        }
    }
}

