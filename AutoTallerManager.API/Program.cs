using MediatR;
using System.Reflection;
using AutoTallerManager.API.Extensions;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoTallerManager.API.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    //string connectionString = builder.Configuration.GetConnectionString(isDocker ? "PostgresDocker" : "PostgresLocal")!;
    //options.UseNpgsql(connectionString);
    //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    var connectionStringKey = isDocker ? "PostgresDocker" : "PostgresLocal";
    var connectionString = builder.Configuration.GetConnectionString(connectionStringKey);

    // Log para debugging
    Console.WriteLine($"========================================");
    Console.WriteLine($"🔍 Entorno detectado: {(isDocker ? "DOCKER" : "LOCAL")}");
    Console.WriteLine($"🔑 Usando conexión: {connectionStringKey}");
    Console.WriteLine($"🔗 Connection String: {connectionString}");
    Console.WriteLine($"========================================");

    // Validar que existe la cadena de conexión
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException($"❌ No se encontró la cadena de conexión '{connectionStringKey}' en appsettings.json");
    }

    options.UseNpgsql(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

});

var app = builder.Build();

// ✅ MIGRACIÓN AUTOMÁTICA Y VERIFICACIÓN DE CONEXIÓN (OPCIONAL PERO RECOMENDADO)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        Console.WriteLine("📊 Verificando conexión a la base de datos...");
        
        // Verificar si la base de datos puede conectarse
        var canConnect = dbContext.Database.CanConnect();
        
        if (canConnect)
        {
            Console.WriteLine("✅ Conexión a la base de datos exitosa");
            
            // Aplicar migraciones pendientes
            Console.WriteLine("📊 Aplicando migraciones...");
            dbContext.Database.Migrate();
            Console.WriteLine("✅ Migraciones aplicadas exitosamente");
        }
        else
        {
            Console.WriteLine("❌ No se pudo conectar a la base de datos");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al conectar con la base de datos:");
        Console.WriteLine($"   Mensaje: {ex.Message}");
        Console.WriteLine($"   Tipo: {ex.GetType().Name}");
        
        if (ex.InnerException != null)
        {
            Console.WriteLine($"   Error interno: {ex.InnerException.Message}");
        }
        
        // En desarrollo, puedes comentar esta línea para que continúe sin BD
        // En producción, es mejor que falle si no hay conexión
        throw;
    }
}

// Swagger y middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoTaller Manager API");
        // Esto hace que swagger se ejecute en la raíz
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("========================================");
Console.WriteLine("🚀 AutoTaller Manager API iniciada");
Console.WriteLine($"🌐 Entorno: {app.Environment.EnvironmentName}");
Console.WriteLine($"📍 URL: http://localhost:8080 (interno)");
Console.WriteLine("========================================");

app.Run();
/*
var app = builder.Build();
Console.WriteLine(builder.Configuration.GetConnectionString("Postgres"));

// Swagger y middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto full-stack");
        // esto hace que swagger se ejecute en la raiz
        c.RoutePrefix = string.Empty; 
    });
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
*/