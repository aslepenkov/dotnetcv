using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using app.Infrastructure;
using UsersService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add service-specific services
builder.Services
    .AddApplicationServices()
    .AddSwaggerServices()
    .AddPostgresDbContext<UserDbContext>(builder.Configuration)
    .AddAwsServices(builder.Configuration)
    .AddCorsPolicy()
    .AddFluentMigrator(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    Database.UpdateDatabase(scope.ServiceProvider);
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
