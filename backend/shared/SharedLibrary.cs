using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Loki;
using Microsoft.EntityFrameworkCore;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;

namespace SharedLibrary;

public static class ServiceCollectionExtensions
{
    public static void AddSharedServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add CORS
        services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:5173")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        // Add AWS services configuration
        var awsConfig = new Amazon.Runtime.BasicAWSCredentials("test", "test");
        var awsEndpoint = new Uri("http://dotnetcv-localstack:4566");

        services.AddSingleton<IAmazonSimpleNotificationService>(_ =>
            new AmazonSimpleNotificationServiceClient(awsConfig, new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = awsEndpoint.ToString()
            }));

        services.AddSingleton<IAmazonSQS>(_ =>
            new AmazonSQSClient(awsConfig, new AmazonSQSConfig
            {
                ServiceURL = awsEndpoint.ToString()
            }));
    }

    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.LokiHttp("http://dotnetcv-loki:3100")
            .CreateLogger();

        builder.Logging.AddSerilog(logger);
    }

    public static void AddPostgresDbContext<TContext>(this IServiceCollection services, IConfiguration configuration, string connectionStringName = "DefaultConnection")
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(connectionStringName),
                b => b.MigrationsAssembly(typeof(TContext).Assembly.FullName)));
    }
}

public static class ApplicationBuilderExtensions
{
    public static void UseSharedMiddleware(this IApplicationBuilder app)
    {
        app.UseCors("FrontendPolicy");
        app.UseSerilogRequestLogging();
        
        if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
    }
}