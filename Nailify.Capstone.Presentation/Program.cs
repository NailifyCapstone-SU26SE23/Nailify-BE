using Nailify.Capstone.Infrastructure.Configuration;
using Nailify.Capstone.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Nailify Capstone API", Version = "v1" });

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

builder.Services.AddInfrastructureToApplication(builder.Configuration);

var app = builder.Build();

app.ApplyMigrations();

app.UseInfrastructure();

app.Run();
