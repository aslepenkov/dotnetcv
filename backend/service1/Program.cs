using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedLibrary;
using Service1.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure logging using shared library
builder.ConfigureLogging();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add EF Core with PostgreSQL using shared configuration
builder.Services.AddPostgresDbContext<UserDbContext>(builder.Configuration);

// Add shared services
builder.Services.AddSharedServices(builder.Configuration);

var app = builder.Build();

// Use shared middleware
app.UseSharedMiddleware();

// Map endpoints
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Json(new { 
    status = "ok",
    timestamp = DateTime.UtcNow,
    service = "Service1"
}))
.WithName("HealthCheck");

app.Run();
