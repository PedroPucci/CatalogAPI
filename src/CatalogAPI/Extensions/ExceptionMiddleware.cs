using CatalogAPI.Application.Contracts.DomainErrors;
using Microsoft.Data.SqlClient;
using Serilog;

namespace CatalogAPI.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.StatusCode = exception switch
            {
                GameCatalogAlreadyExistsException =>
                    StatusCodes.Status409Conflict,

                SqlException =>
                    StatusCodes.Status503ServiceUnavailable,

                _ =>
                    StatusCodes.Status500InternalServerError
            };

            context.Response.ContentType = "application/json";

            Log.Error(
                exception,
                "An error occurred while processing the request. StatusCode: {StatusCode}",
                context.Response.StatusCode);

            var message = exception switch
            {
                GameCatalogAlreadyExistsException =>
                    exception.Message,

                SqlException =>
                    "The database is currently unavailable. Please try again later.",

                _ =>
                    "An unexpected error occurred. Please contact support if the problem persists."
            };

            return context.Response.WriteAsJsonAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            });
        }
    }
}