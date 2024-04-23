using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Helper
{
    public class SeedData
    {
        private static IEnumerable<string> GetRoles()
        {
            yield return AppRole.SuperAdmin;
            yield return AppRole.Admin;
            yield return AppRole.Customer;
            yield return AppRole.Manager;
            yield return AppRole.Accountant;
            yield return AppRole.HR;
            yield return AppRole.Warehouse;
        }
        public static async Task InitializeAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@123";

            foreach (var roleName in GetRoles())
            {
                // Kiểm tra xem vai trò đã tồn tại chưa
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Nếu không tồn tại, tạo mới vai trò
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Tạo một người dùng quản trị nếu chưa tồn tại
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new AppUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(adminUser);
                    await userManager.ConfirmEmailAsync(adminUser, confirmationToken);
                    // Gán role quản trị cho người dùng quản trị
                    await userManager.AddToRoleAsync(adminUser, AppRole.SuperAdmin);
                }
            }
        }
    }
}