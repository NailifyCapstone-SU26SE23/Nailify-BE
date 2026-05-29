using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Infrastructure.DBContext;

namespace Nailify.Capstone.Infrastructure.Extensions
{
    public static class DatabaseMigrationExtension
    {
        public static IHost MigrateDatabase<TContent>(
            this IHost host,
            Func<TContent, IServiceProvider, Task> seeder) where TContent : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContent>>();
            var context = services.GetRequiredService<TContent>();

            try
            {
                logger.LogInformation("Migrating PostgreSQL database...");
                ExecuteMigrations(context);

                logger.LogInformation("Seeding PostgreSQL database...");
                InvokeSeeder(seeder, context, services).GetAwaiter().GetResult();

                logger.LogInformation("Migration and seeding completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the PostgreSQL database.");
                throw;
            }

            return host;
        }

        private static void ExecuteMigrations<TContext>(TContext context) where TContext : DbContext
        {
            context.Database.Migrate();
        }

        private static async Task InvokeSeeder<TContext>(
            Func<TContext, IServiceProvider, Task> seeder,
            TContext context,
            IServiceProvider services) where TContext : DbContext
        {
            await seeder(context, services);
        }
    }
}
