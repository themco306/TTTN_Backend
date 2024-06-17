// File: AuthorizationPolicyBuilder.cs
using System.Reflection;
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
 options.AddPolicy(AppRole.Admin, policy =>
            {
                policy.RequireRole(AppRole.Admin);
            });
            var claimValues = new[] { ClaimValue.Add, ClaimValue.Edit, ClaimValue.Show, ClaimValue.Delete };

             var claimTypes = typeof(backend.Helper.ClaimType)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(fi => fi.GetRawConstantValue().ToString())
            .ToArray();
            foreach (var claimType in claimTypes)
            {
                foreach (var claimValue in claimValues)
                {
                    options.AddPolicy(AppRole.SuperAdmin + claimType + claimValue, policy =>
                    {
                        policy.RequireAssertion(context =>
                        {
                            if (context.User.IsInRole(AppRole.SuperAdmin))
                            {
                                return true;
                            }
                            return context.User.IsInRole(AppRole.Admin) &&
                             context.User.HasClaim(claimType, claimValue);
                        });
                    });
                }
            }





        }
    }
}
