using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MyWebApi.Middlewares
{
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<LoggingMiddleware> _logger = logger; 

        public async Task InvokeAsync(HttpContext context)
        {
            var requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _logger.LogInformation("[LOG] ---> have a {Method} request to {Path}",
                context.Request.Method, context.Request.Path);

            await _next(context);

            _logger.LogInformation("[LOG] <--- response with status code {StatusCode} for {Method} request to {Path}", 
                context.Response.StatusCode, context.Request.Method, context.Request.Path);
        }
    }
}
