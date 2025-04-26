using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace UsersService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        
        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "UsersService API", 
                Version = "v1",
                Description = "UsersService API for user management"
            });
        });
        
        return services;
    }

    public static IServiceCollection AddPostgresDbContext<TContext>(
        this IServiceCollection services, 
        IConfiguration configuration, 
        string connectionStringName = "DefaultConnection") where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(connectionStringName),
                b => b.MigrationsAssembly(typeof(TContext).Assembly.FullName)));
        
        return services;
    }

    public static IServiceCollection AddAwsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var awsConfig = new BasicAWSCredentials("test", "test");
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

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
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

        return services;
    }
}