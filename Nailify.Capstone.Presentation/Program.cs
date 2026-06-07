using Microsoft.OpenApi.Models;
using Nailify.Capstone.Infrastructure.Configuration;
using Nailify.Capstone.Presentation.Extensions;
using Nailify.Capstone.Presentation.Filters;
using Nailify.Capstone.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
}); 
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

builder.Services.AddInfrastructureToApplication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.ApplyMigrations();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseMiddleware<RoleAuthorizationMiddleware>();

app.UseInfrastructure();

app.Run();
