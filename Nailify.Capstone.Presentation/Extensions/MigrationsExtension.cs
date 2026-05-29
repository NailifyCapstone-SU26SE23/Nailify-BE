using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Infrastructure.DBContext;
using Nailify.Capstone.Infrastructure.Extensions;

namespace Nailify.Capstone.Presentation.Extensions
{
    public static class MigrationsExtension
    {
        public static IHost ApplyMigrations(this IHost host)
        {
            host.MigrateDatabase<NailifyDbContext>(async (context, services) =>
            {
                var logger = services.GetRequiredService<ILogger<NailifyContextSeed>>();

                await NailifyContextSeed.SeedProductAsync(context, logger);
            });
            return host;
        }
    }
}
