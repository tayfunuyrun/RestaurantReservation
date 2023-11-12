

using Microsoft.Extensions.Configuration;


namespace RestaurantReservation.Api.Hosting
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .ConfigureModelValidationOptions()
                .ConfigureJsonOptions();

            services.ConfigureDependencies(Environment, Configuration);
            services.ConfigureSwagger();
            services.AddMemoryCache();
            

            services.AddSignalR()
                .AddJsonProtocol(
                    options =>
                        options.PayloadSerializerOptions.PropertyNamingPolicy = null
                );
         ;

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.ConfigureMiddleware(env);

            app.UseRouting();

            app.ConfigureCorsOrigin();
        
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.ConfigureSwagger(env);

        }
    }
}