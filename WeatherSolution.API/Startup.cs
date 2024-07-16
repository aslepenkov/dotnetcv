public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();
        services.AddMediatR(typeof(Startup));
        services.AddDbContext<WeatherDbContext>(options =>
            options.UseInMemoryDatabase("WeatherDb"));
        services.AddDbContext<WeatherDbContext>(options =>
            options.UseInMemoryDatabase("UserDb"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherSolution v1"));
        }

        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}