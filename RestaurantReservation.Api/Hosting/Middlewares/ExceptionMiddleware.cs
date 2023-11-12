using System.Net;

using Newtonsoft.Json;
using Serilog;


namespace KepServer.Api.Hosting.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate Next;
        private static ILogger<ExceptionMiddleware> Logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await Next(httpContext);
            }
            catch (Exception vEx)
            {
                await HandleExceptionAsync(httpContext, vEx);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            const HttpStatusCode code = HttpStatusCode.OK; // 200 if unexpected
            var vResult = "";

            switch (exception)
            {

                case { }:
                {
                    vResult = JsonConvert.SerializeObject(Result.PrepareFailure("Tanımsız hata"));

                    Log.Error(exception, "{Path} : {Message} ", httpContext.Request.Path, exception.Message);
                }
                    break;
            }


            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)code;

            return httpContext.Response.WriteAsync(vResult);
        }
    }
}
