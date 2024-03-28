
using backend.Exceptions;
using UnauthorizedAccessException = backend.Exceptions.UnauthorizedAccessException;
using ForbiddenAccessException  = backend.Exceptions.ForbiddenAccessException;
namespace backend.Helper
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Kiểm tra xem yêu cầu hiện tại đã được xác thực và có quyền truy cập không
            if (!context.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Bạn chưa đăng nhập");
            }

            await _next(context);
        }
    }
}