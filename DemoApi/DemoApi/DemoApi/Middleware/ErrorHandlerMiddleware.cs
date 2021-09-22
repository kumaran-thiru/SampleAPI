using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoApi.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //httpContext.Request.Headers.TryGetValue("Accept",out var xx);
                httpContext.Response.ContentType = "application/json";
                var x = new
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Title = ex.GetType().ToString(),
                    Message = ex.Message,
                    Details = "Error in " + ex.TargetSite.DeclaringType.Name +
                                " -> " + ex.TargetSite.Name
                };
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(x));
            }
        }
        
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
