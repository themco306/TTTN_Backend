
using System.Net;
using System.Text.Json;
using backend.Exceptions;
using KeyNotFoundException = backend.Exceptions.KeyNotFoundException;
using UnauthorizedAccessException = backend.Exceptions.UnauthorizedAccessException;
using NotImplementedException = backend.Exceptions.NotImplementedException;

namespace backend.Helper
{
    public class GlobalExceptionMiddleware 
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionMiddleware(RequestDelegate requestDelegate)
        {
            _next = requestDelegate;
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
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            string message;
            string stackTrace;

            if (ex is BadRequestException)
            {
                status = HttpStatusCode.BadRequest;
            }
            else if (ex is NotFoundException || ex is KeyNotFoundException)
            {
                status = HttpStatusCode.NotFound;
            }
            else if (ex is NotImplementedException)
            {
                status = HttpStatusCode.NotImplemented;
            }
            else if (ex is UnauthorizedAccessException)
            {
                status = HttpStatusCode.Unauthorized;
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
            }

            message = ex.Message;
            // stackTrace = ex.StackTrace;

            var exceptionResult = JsonSerializer.Serialize(new
            {
                error = message,
                // stackTrace = stackTrace 
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            await context.Response.WriteAsync(exceptionResult);
        }

    }
}