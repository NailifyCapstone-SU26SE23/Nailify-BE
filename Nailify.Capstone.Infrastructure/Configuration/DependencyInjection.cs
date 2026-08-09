using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Infrastructure.Configuration.PayOS;
using Nailify.Capstone.Infrastructure.DBContext;
using Nailify.Capstone.Infrastructure.Repository;
using Nailify.Capstone.Infrastructure.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

namespace Nailify.Capstone.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureToApplication(
          this IServiceCollection services,
          IConfiguration configuration,
          IHostEnvironment hostEnvironment)
        {
            // cấu hình JWT Authentication
            var jwtSection = configuration.GetSection("Jwt");
            services.Configure<JwtOptions>(jwtSection); // Đăng ký để các class như JwtProvider có thể dùng IOptions

            // Ánh xạ ngầm cấu hình ra dạng Object để nạp cho JwtBearer lúc Startup hệ thống
            var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

            // 2. CẤU HÌNH XÁC THỰC SỬ DỤNG THUỘC TÍNH ĐỊNH DANH (Dùng jwtOptions thay vì configuration thô)
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    NameClaimType = System.Security.Claims.ClaimTypes.Email,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for the notifications hub
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notifications"))
                        {
                            context.Token = accessToken;
                        }
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });
            // Cấu hình DbContext với PostgreSQL
            services.AddDbContext<NailifyDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly(
                        typeof(NailifyDbContext).Assembly.FullName)
                )
            );

            //auth
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IAuthService, AuthService>();
            // Đăng ký Unit of Work & Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICategoryTypeRepository, CategoryTypeRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<INailCategoryRepository, NailCategoryRepository>();
            services.AddScoped<INailDesignRepository, NailDesignRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ISalonRepository, SalonRepository>();
            services.AddScoped<IChairRepository, ChairRepository>();
            services.AddScoped<INailArtistRepository, NailArtistRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<INailShapeRepository, NailShapeRepository>();
            services.AddScoped<IShapeMethodConfigRepository, ShapeMethodConfigRepository>();
            services.AddScoped<INailSurfaceRepository, NailSurfaceRepository>();
            services.AddScoped<INailVariantRepository, NailVariantRepository>();
            services.AddScoped<INailComponentRepository, NailComponentRepository>();
            services.AddScoped<ICustomerComponentRepository, CustomerComponentRepository>();
            services.AddScoped<ICustomerNailRepository, CustomerNailRepository>();
            services.AddScoped<ICustomerNailComponentRepository, CustomerNailComponentRepository>();
            services.AddScoped<ISkillTypeRepository, SkillTypeRepository>();
            services.AddScoped<INailArtistSkillRepository, NailArtistSkillRepository>();
            services.AddScoped<INailRequiredSkillRepository, NailRequiredSkillRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingItemRepository, BookingItemRepository>();
            services.AddScoped<IBookingHistoryRepository, BookingHistoryRepository>();
            services.AddScoped<IServicesRepository, ServicesRepository>();
            services.AddScoped<IProcedureRepository, ProcedureRepository>();
            services.AddScoped<INailProcedureRepository, NailProcedureRepository>();
            services.AddScoped<IBookingProcedureRepository, BookingProcedureRepository>();
            services.AddScoped<IFavoriteNailRepository, FavoriteNailRepository>();
            services.AddScoped<ILoyaltyTierRepository, LoyaltyTierRepository>();
            services.AddScoped<ILoyaltyTransactionRepository, LoyaltyTransactionRepository>();
            services.AddScoped<IBookingRatingRepository, BookingRatingRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IBookingDiscountRepository, BookingDiscountRepository>();
            services.AddScoped<IUserPromotionUsageRepository, UserPromotionUsageRepository>();
            services.AddScoped<IBookingWaitlistRepository, BookingWaitlistRepository>();
            services.AddScoped<IWalkInQueueRepository, WalkInQueueRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IQuizQuestionRepository, QuizQuestionRepository>();
            services.AddScoped<IQuizOptionRepository, QuizOptionRepository>();
            services.AddScoped<ICustomerQuizAnswerRepository, CustomerQuizAnswerRepository>();
            services.AddScoped<ISalonOffDateRepository, SalonOffDateRepository>();
            services.AddScoped<INailArtistBreakRepository, NailArtistBreakRepository>();

            // Đăng ký Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ICategoryTypeService, CategoryTypeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<INailCategoryService, NailCategoryService>();
            services.AddScoped<INailDesignService, NailDesignService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ISalonService, SalonService>();
            services.AddScoped<IChairService, ChairService>();
            services.AddScoped<INailArtistService, NailArtistService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IComponentService, ComponentService>();
            services.AddScoped<INailShapeService, NailShapeService>();
            services.AddScoped<IShapeMethodConfigService, ShapeMethodConfigService>();
            services.AddScoped<INailSurfaceService, NailSurfaceService>();
            services.AddScoped<INailVariantService, NailVariantService>();
            services.AddScoped<INailVariantPriceRecalculationService, NailVariantPriceRecalculationService>();
            services.AddScoped<INailComponentService, NailComponentService>();
            services.AddScoped<ICustomerComponentService, CustomerComponentService>();
            services.AddScoped<ICustomerNailService, CustomerNailService>();
            services.AddScoped<ICustomerNailRequestsService, CustomerNailRequestsService>();
            services.AddScoped<ICustomerNailComponentService, CustomerNailComponentService>();
            services.AddScoped<ISkillTypeService, SkillTypeService>();
            services.AddScoped<INailArtistSkillService, NailArtistSkillService>();
            services.AddScoped<INailRequiredSkillService, NailRequiredSkillService>();
            services.AddScoped<CloudinaryService>();
            services.AddScoped<IQRService, QRService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingHistoryService, BookingHistoryService>();
            services.AddScoped<IServicesService, ServicesService>();
            services.AddScoped<IBookingProcedureService, BookingProcedureService>();
            services.AddScoped<IProcedureService, ProcedureService>();
            services.AddScoped<IFavoriteNailService, FavoriteNailService>();
            services.AddScoped<ILoyaltyTierService, LoyaltyTierService>();
            services.AddScoped<ILoyaltyTransactionService, LoyaltyTransactionService>();
            services.AddScoped<IBookingRatingService, BookingRatingService>();
            services.AddHttpClient<ISentimentAnalysisService, SentimentAnalysisService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IBookingDiscountService, BookingDiscountService>();
            services.AddScoped<ISlotHoldService, SlotHoldService>();
            services.AddScoped<ICustomerNailRequestsService, CustomerNailRequestsService>();
            services.AddScoped<IBookingSchedulingService, BookingSchedulingService>();
            services.AddScoped<IBookingWaitlistService, BookingWaitlistService>();
            services.AddScoped<IWalkInQueueService, WalkInQueueService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<ISalonOffDateService, SalonOffDateService>();
            services.AddScoped<INailArtistBreakService, NailArtistBreakService>();
            services.AddScoped<IBookingRescheduleService, BookingRescheduleService>();
            services.AddScoped<IBookingCreationService, BookingCreationService>();
            services.AddScoped<IBookingLifecycleService, BookingLifecycleService>();
            services.AddScoped<IBookingAssignmentService, BookingAssignmentService>();
            services.AddScoped<IBookingQueryService, BookingQueryService>();
            services.AddScoped<IBookingSkillMatchingService, BookingSkillMatchingService>();
            // Third Party
            services.AddScoped<IRecommendationService, RecommendationService>();
            if (hostEnvironment.IsDevelopment())
            {
                services.AddScoped<IEmailService, SmtpEmailService>();
            }
            else
            {
                services.AddScoped<IEmailService, SendGridEmailService>();
            }
            services.AddScoped<INailArtistEmergencyService, NailArtistEmergencyService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddHttpClient();
            services.AddScoped<PayOSHelper>();
            services.AddScoped<PayOSService>();
            services.AddScoped<RefundService>();
            services.AddScoped<ITransactionService, TransactionService>();
            // Đăng ký Cloudinary Configuration
            var cloudinarySettings = configuration.GetSection("CloudinarySettings")
                                                  .Get<CloudinaryConfiguration>();
            if (cloudinarySettings != null)
            {
                services.AddSingleton<ICloudinaryConfiguration>(cloudinarySettings);
            }

            var slotHoldSettings = configuration.GetSection("SlotHoldSettings")
                                                  .Get<SlotHoldConfiguration>()
                                   ?? new SlotHoldConfiguration();
            services.AddSingleton<ISlotHoldConfiguration>(slotHoldSettings);

            var redisSettings = configuration.GetSection("Redis")
                                             .Get<RedisConfiguration>()
                                ?? new RedisConfiguration { UseMemoryCache = true };
            services.AddSingleton<IRedisConfiguration>(redisSettings);

            var nemotronSettings = configuration.GetSection("NemotronConfig")
                                                .Get<NemotronConfiguration>()
                                   ?? new NemotronConfiguration();
           
            services.AddSingleton<INemotronConfiguration>(nemotronSettings);

            services.AddScoped<IGoogleAuthService, GoogleAuthService>();


            var googleSettings = configuration.GetSection("Google")
                                              .Get<GoogleConfiguration>()
                                 ?? new GoogleConfiguration();
            services.AddSingleton<IGoogleConfiguration>(googleSettings);

            if (redisSettings.UseMemoryCache)
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                   options.Configuration = redisSettings?.ConnectionString;
                   options.InstanceName = redisSettings?.InstanceName;
                });
            }

            var emailSettings = configuration.GetSection("SMTPEmailSettings")
                                  .Get<SmtpEmailConfiguration>()
                    ?? new SmtpEmailConfiguration();
            services.AddSingleton<IEmailConfiguration>(emailSettings);

            var sendGridSettings = configuration.GetSection("SendGrid")
                                  .Get<SendGridEmailConfiguration>()
                    ?? new SendGridEmailConfiguration();
            services.AddSingleton(sendGridSettings);

            var paymentSettings = configuration.GetSection("PayOSSettings")
                                  .Get<PayOSSettings>()
                    ?? new PayOSSettings();
            services.AddSingleton<IPayOSSettings>(paymentSettings);

            var paymentUrls = configuration.GetSection("PaymentUrls")
                                  .Get<PaymentUrls>()
                    ?? new PaymentUrls();
            if (string.IsNullOrWhiteSpace(paymentUrls.ReturnUrl))
            {
                paymentUrls.ReturnUrl = paymentSettings.ReturnUrl;
            }
            if (string.IsNullOrWhiteSpace(paymentUrls.CancelUrl))
            {
                paymentUrls.CancelUrl = paymentSettings.CancelUrl;
            }
            services.AddSingleton<IPaymentUrls>(paymentUrls);

            // Đăng ký FluentValidation từ tầng Application
            services.AddValidatorsFromAssembly(typeof(Nailify.Capstone.Application.Validation.UserRequestDTOs.UserRegisterRequestValidator).Assembly);

            // Đăng ký AutoMapper
            services.AddAutoMapper(typeof(Nailify.Capstone.Application.Mapping.MappingProfile).Assembly);

            // Đăng ký MediatR cho Assembly chứa BookingService (Tầng Application)
            services.AddMediatR(typeof(Nailify.Capstone.Application.Services.BookingService).Assembly);


            return services;
        }
    }
}
