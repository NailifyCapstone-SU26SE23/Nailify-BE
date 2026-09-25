using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nailify.Capstone.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Nailify.Capstone.Presentation.Extensions
{
    public static class MigrationsExtension
    {
        public static IHost ApplyMigrations(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            try
            {
                scope.ServiceProvider
                    .GetRequiredService<NailifyDbContext>()
                    .Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<NailifyDbContext>>();
                logger.LogError(ex, "An error occurred while applying database migrations.");
            }
            return host;
        }
    }
}
