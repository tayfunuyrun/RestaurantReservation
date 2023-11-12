using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Api.Services;
using RestaurantReservation.Data.Models;

namespace RestaurantReservation.Api.Hosting
{
    public static class Dependencies
    {
        public static void ConfigureDependencies(this IServiceCollection services, IWebHostEnvironment env, IConfiguration configuration)
        {
            services.AddDbContext<ReservationDbContext>(builder =>
            {
                builder.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                if (!env.IsDevelopment())
                    return;

                builder.EnableSensitiveDataLogging();
                builder.EnableDetailedErrors();
            });

            services.AddScoped<SmtpClient>((serviceProvider) =>
            {
                return new SmtpClient()
                {
                    Host = configuration.GetValue<String>("Email:Smtp:Host"),
                    Port = configuration.GetValue<int>("Email:Smtp:Port"),
                    Credentials = new NetworkCredential(
                        configuration.GetValue<String>("Email:Smtp:Username"),
                        configuration.GetValue<String>("Email:Smtp:Password")
                    ),
                };
            });

            services.AddScoped<ReservationService>();
            services.AddScoped<MailService>();



        }
    }
    
}
