using MediatR;
using System.Reflection;
using AutoTallerManager.API.Extensions;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoTallerManager.API.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "AutoTallerManager API", 
        Version = "v1",
        Description = "API para gestión de taller automotriz"
    });
    
    // Configurar autenticación JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// REGISTRA SERVICIOS Y CONFIGURACIONES PERSONALIZADAS DEL APPLICATIONSERVICEEXTENSION 
builder.Services.ConfigureCors();
builder.Services.AddApplicationServices();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddValidationErrors();
builder.Services.AddCustomRateLimiter();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        Assembly.GetExecutingAssembly(),                              // Tu API
        typeof(AutoTallerManager.Application.Abstractions.IUnitOfWork).Assembly // Tu capa Application
    );
});

// Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    string connectionString = builder.Configuration.GetConnectionString(isDocker ? "PostgresDocker" : "PostgresLocal")!;
    options.UseNpgsql(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

var app = builder.Build();

// Swagger y middlewares de desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto full-stack");
        // esto hace que swagger se ejecute en la raiz
        c.RoutePrefix = string.Empty; 
    });
}

// Middleware de excepciones (después de Swagger en desarrollo)
app.UseMiddleware<ExceptionMiddleware>();

// Middleware de auditoría
app.UseMiddleware<AuditoriaMiddleware>();

var isDockerRuntime = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (!isDockerRuntime)
{
    app.UseHttpsRedirection();
}

// Solo una política CORS (elige la que necesites)
app.UseCors("CorsPolicy"); // O la política que prefieras

app.UseRateLimiter();

// ORDEN CORRECTO: Authentication ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible for testing
public partial class Program { }