using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProjectSystem.Core.Exceptions;
using Serilog;
using ApplicationException = ProjectSystem.Core.Exceptions.ApplicationException;

namespace ProjectSystem.Api.Filters
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var stackTrace = exception.StackTrace;

            var exceptionResult =
                $$"""
                {
                    "message": "{{exception.Message}}",
                    "stackTrace": "{{stackTrace}}",
                    "object": {{(exception is ObjectApplicationException oae ? $"\"{JsonConvert.SerializeObject(oae.Object)}\"" : "null")}}
                }
                """;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception is ApplicationException applicationException
                ? applicationException.StatusCode
                : 500;
            _logger.Error(exception, "Unhandled exception occurred, status code is {Code}",
                context.Response.StatusCode);
            return context.Response.WriteAsync(exceptionResult);
        }
    }
}
