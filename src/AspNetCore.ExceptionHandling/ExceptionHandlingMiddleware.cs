using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.ExceptionHandling.Models;

namespace AspNetCore.ExceptionHandling
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
        }

        public virtual async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        protected Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError($"{exception}");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new ErrorDetails()
            {
                Code = context.Response.StatusCode,
                Message = "Internal Server Error. Please refer to the logs for more information."
            }.ToString());
        }
    }
}
