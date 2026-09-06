using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services.BackgroundJobs;
using Nailify.Capstone.Infrastructure.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Extensions
{
    public static class HangfireSignalRExtensions
    {
        public static IServiceCollection AddNailifyHangfireAndSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Đăng ký các Service
            services.AddTransient<IScheduledJobService, ScheduledJobService>();
            services.AddTransient<INotificationService, SignalRNotificationService>();
            services.AddTransient<IWaitlistJobExecutor, WaitlistJobExecutor>();
            services.AddTransient<IBookingJobExecutor, BookingJobExecutor>();
            // 2. Đăng ký Hangfire sử dụng PostgreSQL
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(connectionString);
                }, new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    DistributedLockTimeout = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true
                }));
            // 3. Đăng ký Hangfire Server
            services.AddHangfireServer(options =>
            {
                options.ServerName = "Nailify Background Server";
                options.WorkerCount = 2;
            });
            // 4. Đăng ký SignalR
            services.AddSignalR();
            return services;
        }
    }
}
