using AutoTallerManager.API.Extensions;
using AutoTallerManager.Infrastructure;
using AutoTallerManager.Infrastructure.Persistence.Context;
using AutoTallerManager.API.Extensions;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Common.Behaviors;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.ConfigureCors();
builder.Services.AddCustomRateLimiter();
builder.Services.AddApplicationServices();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("PostgresLocal")!;
    options.UseNpgsql(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// builder.Services.AddDbContext<AppDbContext>(opt =>
//     opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("CorsPolicy");
app.UseCors("CorsPolicyUrl");
app.UseCors("Dinamica");

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
