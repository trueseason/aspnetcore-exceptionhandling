using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ExceptionHandling
{
    public class ApiExceptionHandlingMiddleware : ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public ApiExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory) : base(next, loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ApiExceptionHandlingMiddleware>();
        }

        public override async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if (httpContext.Request.IsAjaxRequest() || httpContext.Request.HasJsonContentType())
                {
                    await HandleExceptionAsync(httpContext, ex);
                }
                else
                    throw;
            }
        }
    }
}
