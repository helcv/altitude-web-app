using backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class Seed
    {
        public static async Task SeedAdminAndRoles(UserManager<User> userManager, RoleManager<Role> roleManager,
                                                   IConfiguration config)
        {
            if (await userManager.Users.AnyAsync()) return;

            var roles = new List<Role>()
            {
                new Role {Name = "Admin"},
                new Role {Name = "User"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            var admin = new User
            {
                Name = "admin",
                LastName = "admin",
                Email = config["AdminSettings:Email"],
                UserName = config["AdminSettings:Email"]
            };

            var result = await userManager.CreateAsync(admin, config["AdminSettings:Password"]);
            admin.EmailConfirmed = true;
            await userManager.UpdateAsync(admin);

            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
