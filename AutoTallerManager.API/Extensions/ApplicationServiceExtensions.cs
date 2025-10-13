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
using AutoTallerManager.API.Services;
using AutoTallerManager.Application.Services;

namespace AutoTallerManager.API.Extensions;

// this file defines application extension methods such as CORS, JWT, app services,
// RateLimiter, validation errors, etc.
public static class ApplicationServiceExtensions
{
    // CORS policy registration
    public static void ConfigureCors(this IServiceCollection services) =>

        services.AddCors(options =>
        {
            // allowed domains for the application
            HashSet<String> allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "https://app.ejemplo.com",
                "https://admin.ejemplo.com"
            };
            // default CORS behavior
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()   //WithOrigins("https://dominio.com")
                .AllowAnyMethod()          //WithMethods("GET","POST")
                .AllowAnyHeader());        //WithHeaders("accept","content-type")

            // CORS behavior for specific URLs
            options.AddPolicy("CorsPolicyUrl", builder =>
                builder.WithOrigins("https://localhost:4200", "https://localhost:5500")   //WithOrigins("https://dominio.com")
                .AllowAnyMethod()          //WithMethods("GET","POST")
                .AllowAnyHeader());
            // another CORS behavior using a dynamic origin list
            options.AddPolicy("Dinamica", builder =>
                builder.SetIsOriginAllowed(origin => allowed.Contains(origin))   //WithOrigins("https://dominio.com")
                .WithMethods("GET", "POST")
                .WithHeaders("Content-Type", "Authorization"));        //WithHeaders("accept","content-type")
        });

    // registers application services
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // register password hasher
        services.AddScoped<IPasswordHasher<UserMember>, PasswordHasher<UserMember>>();
        
        // register services 
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register application services
        services.AddScoped<ICalculadoraFechasService, CalculadoraFechasService>();
        services.AddScoped<IValidadorDisponibilidadVehiculoService, ValidadorDisponibilidadVehiculoService>();

        // Registrar MediatR/Validators/AutoMapper desde la capa de Application
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        // FluentValidation registration
        // If using FluentValidation.DependencyInjectionExtensions
        // services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddAutoMapper(typeof(Program).Assembly);
        
        // también agregar todos los mapeos de los ensamblados actualmente cargados
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    // adds the RateLimiter with specific rules for AutoTallerManager
    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Configurar respuesta personalizada cuando se excede el límite
            options.OnRejected = async (context, token) =>
            {
                var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
                var endpoint = context.HttpContext.Request.Path;
                
                context.HttpContext.Response.StatusCode = 429;
                context.HttpContext.Response.ContentType = "application/json";
                
                var mensaje = $@"{{
                    ""error"": ""Rate limit exceeded"",
                    ""message"": ""Demasiadas peticiones desde la IP {ip} para el endpoint {endpoint}. Intenta más tarde."",
                    ""retryAfter"": ""60 segundos"",
                    ""timestamp"": ""{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}""
                }}";
                
                await context.HttpContext.Response.WriteAsync(mensaje, token);
            };

            // Regla específica para órdenes de servicio: 60 solicitudes por minuto
            options.AddPolicy("OrdenesServicio", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 5,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            // Regla específica para repuestos: 30 solicitudes por minuto
            options.AddPolicy("Repuestos", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 3,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            // Regla específica para autenticación: 10 intentos por minuto
            options.AddPolicy("Auth", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 2,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            // Regla específica para facturas: 20 solicitudes por minuto
            options.AddPolicy("Facturas", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 20,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 3,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            // Regla global para endpoints generales: 100 solicitudes por minuto
            options.AddPolicy("Global", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 10,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            // Regla para usuarios autenticados: límites más altos
            options.AddPolicy("Authenticated", httpContext =>
            {
                var userId = httpContext.User?.Identity?.Name ?? "anonymous";
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 200,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 20,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
        });

        return services;
    }

    // adds JWT authentication and authorization
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        //Configuration from AppSettings
        services.Configure<JWT>(configuration.GetSection("JWT"));

        //Adding Authentication - JWT
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
        
        // Authorization – Policies
        services.AddAuthorization(options =>
        {
            // Policy requiring Administrator role
            options.AddPolicy("Admins", policy =>
                policy.RequireRole("Administrator"));

            options.AddPolicy("Others", policy =>
                policy.RequireRole("Other"));

            options.AddPolicy("Pro", policy =>
                policy.RequireRole("Professional"));

            // Policy requiring claim Subscription = "Premium"
            options.AddPolicy("Professional", policy =>
                policy.RequireClaim("Subscription", "Premium"));

            // Composite policy: role Other or Subscription claim Premium
            options.AddPolicy("OtherOPremium", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Other")
                || context.User.HasClaim(c =>
                        c.Type == "Subscription" && c.Value == "Premium")));
        });
    }

    // adds custom validation error response formatting
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