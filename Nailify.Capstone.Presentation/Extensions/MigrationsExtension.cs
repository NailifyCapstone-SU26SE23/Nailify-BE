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
            scope.ServiceProvider
                .GetRequiredService<NailifyDbContext>()
                .Database.Migrate();
            return host;
        }
    }
}
