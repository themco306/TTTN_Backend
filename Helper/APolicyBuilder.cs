// File: AuthorizationPolicyBuilder.cs
using Microsoft.AspNetCore.Authorization;

namespace backend.Helper
{
    public static class APolicyBuilder
    {
        public static void Build(AuthorizationOptions options)
        {
            options.AddPolicy(AppRole.SuperAdmin, policy =>
            {
                policy.RequireRole(AppRole.SuperAdmin);
            });

            var claimValues = new[] { ClaimValue.Add, ClaimValue.Edit, ClaimValue.Show, ClaimValue.Delete };

            foreach (var claimValue in claimValues)
            {
                options.AddPolicy(AppRole.SuperAdmin + ClaimType.CategoryClaim + claimValue, policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        if (context.User.IsInRole(AppRole.SuperAdmin))
                        {
                            return true;
                        }
                        return context.User.IsInRole(AppRole.Admin) &&
                         context.User.HasClaim(ClaimType.CategoryClaim, claimValue);
                    });
                });
            }

            //     options.AddPolicy(ClaimType.ProductClaim + claimValue, policy =>
            //    {
            //        policy.RequireClaim(ClaimType.ProductClaim, claimValue);
            //    });


        }
    }
}
