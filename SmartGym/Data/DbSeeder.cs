using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Identity;

namespace GymWebApiBackend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@smartgym.hu";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    TeljesNev = "Rendszergazda"
                };

                var adminResult = await userManager.CreateAsync(adminUser, "Admin123!");

                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var userEmail = "user@smartgym.hu";
            var normalUser = await userManager.FindByEmailAsync(userEmail);

            if (normalUser == null)
            {
                normalUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    TeljesNev = "Teszt Felhasználó"
                };

                var userResult = await userManager.CreateAsync(normalUser, "User123!");

                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "User");

                    var ujTag = new Tag
                    {
                        IdentityUserId = normalUser.Id,
                        Vezeteknev = "Teszt",
                        Keresztnev = "Felhasználó",
                        SzuletesiDatum = new DateTime(2000, 1, 1),
                        Aktiv = true
                    };

                    dbContext.Tagok.Add(ujTag);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}