
using backend.Context;
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
        private static string GetDescriptionForTag(TagType tagType)
{
    // Xác định mô tả cho mỗi loại tag dựa trên enum TagType
    switch (tagType)
    {
        case TagType.NewModel:
            return "Những sản phẩm mới sẽ được hiển thị";
        case TagType.BestSeller:
            return "Những sản phẩm có nhiều lượt mua sẽ được hiển thị";
        // Xác định mô tả cho các loại tag khác nếu cần
        default:
            return "Mô tả mặc định cho tag";
    }
}
        public static async Task InitializeAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,AppDbContext dbContext)
        {
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@123";
            int sort=0;
        foreach (TagType tagType in Enum.GetValues(typeof(TagType)))
    {
        string tagName = tagType.ToString(); // Sử dụng tên enum làm tên của tag
        string description = GetDescriptionForTag(tagType);
        
        // Tạo một tag nếu chưa tồn tại
        if (!dbContext.Tags.Any(t => t.Type == tagType))
        {
            var tag = new Tag
            {
                Name = tagName,
                Type = tagType,
                Description = description,
                Sort=sort++,
            };

            dbContext.Tags.Add(tag);
        }
    }

    // Lưu thay đổi vào cơ sở dữ liệu
    await dbContext.SaveChangesAsync();

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