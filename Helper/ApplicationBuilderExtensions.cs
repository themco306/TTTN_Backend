namespace backend.Helper
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder AddGlobalErrorHandler(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<GlobalExceptionMiddleware>();
        }

        public static IApplicationBuilder AddCustomAuthorizationHandler(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/account"), 
                builder => builder.UseMiddleware<CustomAuthorizationMiddleware>());
        }
    }
}
