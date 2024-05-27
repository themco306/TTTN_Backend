
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
        }
        private static string GetDescriptionForTag(TagType tagType)
        {
            switch (tagType)
            {
                case TagType.NewModel:
                    return "Những sản phẩm mới sẽ được hiển thị";
                case TagType.BestSeller:
                    return "Những sản phẩm có nhiều lượt mua sẽ được hiển thị";
                default:
                    return "Mô tả mặc định cho tag";
            }
        }
        public static async Task InitializeAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext)
        {
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@123";
            int sort = 0;
            foreach (TagType tagType in Enum.GetValues(typeof(TagType)))
            {
                string tagName = tagType.ToString(); 
                string description = GetDescriptionForTag(tagType);
                if (!dbContext.Tags.Any(t => t.Type == tagType))
                {
                    var tag = new Tag
                    {
                        Name = tagName,
                        Type = tagType,
                        Description = description,
                        Sort = sort++,
                    };

                    dbContext.Tags.Add(tag);
                }
            }
             if (!dbContext.WebInfos.Any())
    {
        var webInfo = new WebInfo
        {
            Icon = "default_icon.png", 
            ShopName = "Tên Cửa hàng",
            Description="Đam mê không chỉ có trên màn ảnh",
            PhoneNumber = "Số điện thoại",
            Email = "example@example.com",
            Address = "Địa chỉ",
            WorkingHours = "Giờ làm việc",
            GoogleMap="Coppy iframe google map",
            FacebookLink = "https://www.facebook.com",
            InstagramLink = "https://www.instagram.com",
            TwitterLink = "https://twitter.com"
        };

        dbContext.WebInfos.Add(webInfo);
    }

            await dbContext.SaveChangesAsync();

            foreach (var roleName in GetRoles())
            {
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
                    LastName = "User",
                    Avatar="avatar-nam.jpg",
                    PhoneNumber="0366281394",
                    Gender=true
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