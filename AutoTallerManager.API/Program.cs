

using AutoTallerManager.API.Extensions;
using AutoTallerManager.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
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

// Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    string connectionString = builder.Configuration.GetConnectionString(isDocker ? "PostgresDocker" : "PostgresLocal")!;
    options.UseNpgsql(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

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