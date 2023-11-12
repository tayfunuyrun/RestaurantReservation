using System.Globalization;
using System.Xml;
using KepServer.Api.Hosting.Middlewares;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Context;
using Formatting = Newtonsoft.Json.Formatting;


namespace RestaurantReservation.Api.Hosting
{
    public static class Configs
    {
        public static void ConfigureMiddleware(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            if (!environment.IsProduction())
                app.UseMiddleware<ResponseLogMiddleware>();
        }

   

        public static void ConfigureCorsOrigin(this IApplicationBuilder app)
        {
            app.UseCors(options =>
            {
                options.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true) 
                    .AllowCredentials();
            });
        }


        public static IMvcBuilder ConfigureModelValidationOptions(this IMvcBuilder builder)
        {
            return builder.ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }


        public static IMvcBuilder ConfigureJsonOptions(this IMvcBuilder builder)
        {
          
            return builder.AddNewtonsoftJson(options =>
            {

                options.SerializerSettings.ContractResolver = new DefaultContractResolver();

                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                options.SerializerSettings.Formatting = Formatting.None;

                options.SerializerSettings.FloatParseHandling = FloatParseHandling.Decimal;
                options.SerializerSettings.Culture = CultureInfo.GetCultureInfo("tr-TR");

                options.SerializerSettings.Error = (sender, args) =>
                {
                    using (LogContext.PushProperty("CurrentObject", args.CurrentObject))
                        Log.Logger.Error(args.ErrorContext.Error, "{Message}",
                            args.ErrorContext.Error.Message);
                };
            });
        }
        
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ReservationProject.v1", new OpenApiInfo { Title = "ReservationProject", Version = "v1.0", });
               

                var vScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. " +
                                  "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                                  "\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                options.AddSecurityDefinition("Bearer", vScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {vScheme, new string[]{}}
                });
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
            });
        }

        public static void ConfigureSwagger(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            //if (environment.IsProduction())
            //    return;
            
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "swagger/{documentName}/docs.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint("/swagger/ReservationProject.v1/docs.json", "ReservationProject ");

            });
            app.UseDeveloperExceptionPage();
        }
    }

}
