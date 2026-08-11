using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Configuration;
using Nailify.Capstone.Infrastructure.Extensions;
using Nailify.Capstone.Infrastructure.Service;
using Nailify.Capstone.Presentation.Extensions;
using Nailify.Capstone.Presentation.Filters;
using Nailify.Capstone.Presentation.Middlewares;
using System.Text.Json.Serialization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("ModelValidation");

        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        logger.LogWarning(
            "Model validation failed for {Method} {Path}: {@Errors}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            errors);

        return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
    };
});

builder.Services.AddNailifyHangfireAndSignalR(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Nailify Capstone API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, // Đặt kiểu là Http thay vì ApiKey
        Scheme = "Bearer",                                       // Swagger sẽ tự hiểu và dán chữ "Bearer " trước token
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Vui lòng chỉ dán chuỗi Token JWT của bạn vào ô dưới đây (Không nhập chữ 'Bearer ')."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    // Đọc XML comment của tầng API hiện tại
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Đọc XML comment của tầng Application ĐỘNG (Lấy qua Class ApiResult của Application)
    var appAssemblyName = typeof(Nailify.Capstone.Application.Common.ApiResult<>).Assembly.GetName().Name;
    var appXmlFile = $"{appAssemblyName}.xml";
    var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXmlFile);
    if (File.Exists(appXmlPath))
    {
        options.IncludeXmlComments(appXmlPath);
    }
});

builder.Services.AddInfrastructureToApplication(builder.Configuration, builder.Environment);

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReactApp", policy =>
//    {
//        policy.WithOrigins(
//            "http://localhost:5173",
//            "http://localhost:5174",
//            "http://localhost:58887",
//            "https://nailify.online"
//            )
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.ApplyMigrations();

app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization(); 
app.UseMiddleware<RoleAuthorizationMiddleware>();

app.UseInfrastructure();
app.MapHub<NotificationHub>("/notifications");
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Nailify Background Jobs Dashboard",
    Authorization = new[] { new HangfireNoAuthFilter() } 
});
await RegisterRecurringJobsAsync();
app.Run();

async Task RegisterRecurringJobsAsync()
{
    const int maxAttempts = 3;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            RegisterRecurringJobs();
            return;
        }
        catch (PostgreSqlDistributedLockException ex) when (attempt < maxAttempts)
        {
            app.Logger.LogWarning(
                ex,
                "Could not acquire Hangfire recurring-job lock. Retrying job registration in {DelaySeconds} seconds. Attempt {Attempt}/{MaxAttempts}.",
                10,
                attempt,
                maxAttempts);

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
        catch (PostgreSqlDistributedLockException ex)
        {
            app.Logger.LogError(
                ex,
                "Could not acquire Hangfire recurring-job lock after {MaxAttempts} attempts. Continuing startup; existing recurring jobs remain in Hangfire storage.",
                maxAttempts);
        }
    }
}

static void RegisterRecurringJobs()
{
    // A. Quét và hủy lịch trễ check-in quá 15 phút (Chạy định kỳ mỗi 10 phút)
    RecurringJob.AddOrUpdate<IBookingJobExecutor>(
        "cancel-late-bookings",
        executor => executor.CancelLateBookingsAsync(),
        "*/10 * * * *"
    );
    // B. Reset dọn dẹp hàng chờ vào 0h đêm mỗi ngày (Cron: 0 0 * * *)
    RecurringJob.AddOrUpdate<IWaitlistJobExecutor>(
        "clear-daily-waitlist",
        executor => executor.ClearDailyWaitlistAsync(),
        "0 0 * * *"
    );
}

