using Microsoft.AspNetCore.Hosting;
using RestaurantReservation.Api.Hosting;
using Serilog;
using Serilog.Core;


namespace Reservation.Api
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception vEx)
            {
                Log.Fatal(vEx, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfiguration vBuiltConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var vLoggingLevelSwitch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(vLoggingLevelSwitch)
                .ReadFrom.Configuration(vBuiltConfig)
                .CreateLogger();

            return Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {

            })
                .ConfigureHostConfiguration(builder => { builder.AddConfiguration(vBuiltConfig); })
                .ConfigureAppConfiguration((hostingContext, config) => { })
                .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog();
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseKestrel((context, options) =>
                        {
                            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
                        })
                    .UseIISIntegration()
                        .UseStartup<Startup>();

                });


        }
    }
}