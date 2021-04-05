using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;
using AspNetCore.ExceptionHandling.Models;

namespace AspNetCore.ExceptionHandling
{
    public static class ExceptionHandlingExtensions
    {
        public static bool HasJsonContentType(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return request.GetTypedHeaders().Accept.Contains(new MediaTypeHeaderValue("application/json"));
        }

        public static bool IsAjaxRequest(this HttpRequest request, string httpVerb = "")
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!string.IsNullOrWhiteSpace(httpVerb) && request.Method != httpVerb)
            {
                return false;
            }

            if (request.Headers != null)
            {
                var headers = request.Headers["X-Requested-With"];
                return headers.Count > 0 && string.Equals(headers[0], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiExceptionHandlingMiddleware>();
        }

        public static void UseApiExceptionHandler(this IApplicationBuilder app, ILogger logger = null)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger?.LogError(contextFeature.Error, "ExceptionHandlerFeature");
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            Code = context.Response.StatusCode,
                            Message = "Internal Server Error. Please refer to the logs for more information."
                        }.ToString());
                    }
                });
            });
        }

        public static void UseApiExceptionHandler(this IApplicationBuilder app, string errorHandlingPath, ILogger logger = null)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    if (context.Request.IsAjaxRequest() || context.Request.HasJsonContentType())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (contextFeature != null)
                        {
                            logger?.LogError(contextFeature.Error, "ExceptionHandlerFeature");
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                Code = context.Response.StatusCode,
                                Message = "Internal Server Error. Please refer to the logs for more information."
                            }.ToString());
                        }
                    }
                    else
                    {
                        context.Response.Redirect(errorHandlingPath);
                    }
                });
            });
        }
    }
}
