using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
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
                var passwordHasher = services.GetRequiredService<IPasswordHasher>();

                await NailifyContextSeed.SeedProductAsync(context, logger, passwordHasher);
            });
            return host;
        }
    }
}
