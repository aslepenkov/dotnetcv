using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace OrdersService.Infrastructure;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrdersService API V1");
        });
        
        return app;
    }

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(contextFeature.Error, "Unhandled exception");

                    await context.Response.WriteAsJsonAsync(new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = env.IsDevelopment() 
                            ? contextFeature.Error.Message 
                            : "An unexpected error occurred"
                    });
                }
            });
        });
        
        return app;
    }
}