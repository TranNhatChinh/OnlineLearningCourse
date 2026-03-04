using Web.Middleware;
using Application.Abstractions;
using Application.Common.Behaviors;
using System.Text.Json;
using Infrastructure.Data;
using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MediatR;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add DbContext abstraction for Application layer
builder.Services.AddScoped<IAppDbContext, ApplicationDbContext>();

// Add Repositories (if needed separately)
// Example: builder.Services.AddScoped<IYourRepository, YourRepository>();

// Add Mapperly Mappers
// Example: builder.Services.AddSingleton<YourMapper>();

// Add MediatR with ValidationBehavior
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("Application"));
    // Add ValidationBehavior to pipeline
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Add FluentValidation - auto-discover all validators
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Application"));

// Configure JSON serialization for all endpoints & WriteAsJsonAsync
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handler - MUST be first in pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

// Map Your Endpoints
// Example: app.MapYourEndpoints();

app.Run();
