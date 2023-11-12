using System.Text;
using System.Text.RegularExpressions;

namespace KepServer.Api.Hosting.Middlewares
{
    public class ResponseLogMiddleware
    {
        private readonly RequestDelegate Next;
        private static ILogger<ResponseLogMiddleware> Logger;

        public ResponseLogMiddleware(RequestDelegate next, ILogger<ResponseLogMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //First, get the incoming request
            string vRequest = await FormatRequest(context.Request);

            //Copy a pointer to the original response body stream
            Stream vOriginalBodyStream = context.Response.Body;

            //Create a new memory stream...
            using (var vResponseBody = new MemoryStream())
            {
                //...and use that for the temporary response body
                context.Response.Body = vResponseBody;

                //Continue down the Middleware pipeline, eventually returning to this class
                await Next(context);

                //Format the response from the server
                string vResponse = await FormatResponse(context.Response);

                if (context.Request.Path != "/docs/index.html")
                {
                    string vClearedRequest = Regex.Replace(vRequest, @"[^\u0009\u000A\u000D\u0020-\u007E]", "");
                    string vClearedRespone = Regex.Replace(vResponse, @"[^\u0009\u000A\u000D\u0020-\u007E]", "");

                    Logger.LogTrace("Request/Response: {Request} {Response}", vClearedRequest, vClearedRespone);
                }

                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await vResponseBody.CopyToAsync(vOriginalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            request.Body.Seek(0, SeekOrigin.Begin);

            return $"{request.Scheme} {request.Host} {request.Path} {request.QueryString} {bodyAsText}";
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }
    }
}
