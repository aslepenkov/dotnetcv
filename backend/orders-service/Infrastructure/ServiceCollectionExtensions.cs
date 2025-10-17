using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Infrastructure;
using StackExchange.Redis;
using MassTransit;
using OpenTelemetry.Trace;

namespace OrdersService.Infrastructure;

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
                Title = "OrdersService API", 
                Version = "v1",
                Description = "OrdersService API for order management"
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

    public static IServiceCollection AddResiliencePolicies(this IServiceCollection services)
    {
        services.AddHttpClient("DefaultClient")
            .AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());

        return services;
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));

        return services;
    }

    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMQ"));
            });
        });

        return services;
    }

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddJaegerExporter(options =>
                    {
                        options.AgentHost = configuration["Jaeger:Host"];
                        options.AgentPort = configuration.GetValue<int>("Jaeger:Port");
                    });
            });

        return services;
    }
}