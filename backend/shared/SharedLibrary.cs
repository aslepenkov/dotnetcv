using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace SharedLibrary
{
    public static class ServiceExtensions
    {
        public static void AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure CORS
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                    {
                        // Allow any port for localhost
                        return allowedOrigins.Any(allowed => origin.StartsWith(allowed));
                    })
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }

        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            // Configure Serilog for request logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog(); // Use Serilog as the logging provider
        }
    }
}