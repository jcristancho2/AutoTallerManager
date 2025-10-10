
using FluentValidation;
using MediatR;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Infrastructure.UnitOfWork;
using System.Threading.RateLimiting;
using AutoTallerManager.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using AutoTallerManager.API.Helpers.Errors;
using Microsoft.AspNetCore.Identity;
using AutoTallerManager.Domain.Entities.Auth;
using AutoTallerManager.API.Services.Implementations;
using AutoTallerManager.API.Services.Interfaces;
using AutoTallerManager.API.Services.Interfaces.Auth;
using AutoTallerManager.API.Services.Implementations.Auth;


namespace AutoTallerManager.API.Extensions;

// este archivo define ciertos metodos de extensión para la aplicación, como CORS, JWT, servicios de aplicacion, RateLimiter, errores de validación, etc...
public static class ApplicationServiceExtensions
{
    // este es el metodo de CORS que se usa en la aplicación
    public static void ConfigureCors(this IServiceCollection services) =>

        services.AddCors(options =>
        {
            // estos son los dominios permitidos para la aplicación
            HashSet<String> allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "https://app.ejemplo.com",
                "https://admin.ejemplo.com"
            };
            // este es el comportamiento de CORS
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()   //WithOrigins("https://dominio.com")
                .AllowAnyMethod()          //WithMethods("GET","POST")
                .AllowAnyHeader());        //WithHeaders("accept","content-type")

            // este es el comportamiento de CORS para URLs específicas
            options.AddPolicy("CorsPolicyUrl", builder =>
                builder.WithOrigins("https://localhost:4200", "https://localhost:5500")   //WithOrigins("https://dominio.com")
                .AllowAnyMethod()          //WithMethods("GET","POST")
                .AllowAnyHeader());
            // otro comportamiento de CORS
            options.AddPolicy("Dinamica", builder =>
                builder.SetIsOriginAllowed(origin => allowed.Contains(origin))   //WithOrigins("https://dominio.com")
                .WithMethods("GET", "POST")
                .WithHeaders("Content-Type", "Authorization"));        //WithHeaders("accept","content-type")
        });
    // este método registra los servicios de la aplicación
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // estp registrar el hasher de contraseñas
        services.AddScoped<IPasswordHasher<UserMember>, PasswordHasher<UserMember>>();
        // registrar servicios 

        // este es el servicio de autenticación
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        //services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        //services.AddAutoMapper(typeof(Program).Assembly);
        // esto aparentemente es para agregar directamente todos los mapeos
        //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
    // este es el meetodo para agregar el RateLimiter 
    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, token) =>
            {
                var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
                context.HttpContext.Response.StatusCode = 429;
                context.HttpContext.Response.ContentType = "application/json";
                var mensaje = $"{{\"message\": \"Demasiadas peticiones desde la IP {ip}. Intenta más tarde.\"}}";
                await context.HttpContext.Response.WriteAsync(mensaje, token);
            };

            // Aquí no se define GlobalLimiter
            options.AddPolicy("ipLimiter", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
            // Fixed Window Limiter
            // options.AddFixedWindowLimiter("fixed", opt =>
            // {
            //     opt.Window = TimeSpan.FromSeconds(10);
            //     opt.PermitLimit = 5;
            //     opt.QueueLimit = 0;
            //     opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            // });

            // Sliding Window Limiter
            // options.AddSlidingWindowLimiter("sliding", opt =>
            // {
            //     opt.Window = TimeSpan.FromSeconds(10);
            //     opt.SegmentsPerWindow = 3;
            //     opt.PermitLimit = 6;
            //     opt.QueueLimit = 0;
            //     opt.QueueProcessingOrder = QueueProcessingOrder.NewestFirst;
            //     // Aquí se personaliza la respuesta cuando se excede el límite
            // });

            // Token Bucket Limiter
            // options.AddTokenBucketLimiter("token", opt =>
            // {
            //     opt.TokenLimit = 20;
            //     opt.TokensPerPeriod = 4;
            //     opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
            //     opt.QueueLimit = 2;
            //     opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //     opt.AutoReplenishment = true;
            // });

        });

        return services;
    }
    // este es el metodo para agregar el JWT
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        //Configuration from AppSettings
        services.Configure<JWT>(configuration.GetSection("JWT"));

        //Adding Athentication - JWT
        _ = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!))
                };
            });
        // 3. Authorization – Policies
        services.AddAuthorization(options =>
        {
            // Política que exige rol Admin
            options.AddPolicy("Admins", policy =>
                policy.RequireRole("Administrator"));

            options.AddPolicy("Others", policy =>
                policy.RequireRole("Other"));

            options.AddPolicy("Pro", policy =>
                policy.RequireRole("Professional"));

            // Política que exige claim Subscription = "Premium"
            options.AddPolicy("Professional", policy =>
                policy.RequireClaim("Subscription", "Premium"));

            // Política compuesta: rol Admin o claim Premium
            options.AddPolicy("OtherOPremium", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Other")
                || context.User.HasClaim(c =>
                        c.Type == "Subscription" && c.Value == "Premium")));
        });
    }
    // este metodos sirve para agregar los errores de validacion
    public static void AddValidationErrors(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {

                var errors = actionContext.ModelState.Where(u => u.Value!.Errors.Count > 0)
                                                .SelectMany(u => u.Value!.Errors)
                                                .Select(u => u.ErrorMessage).ToArray();

                var errorResponse = new ApiValidation()
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });
    }
}
