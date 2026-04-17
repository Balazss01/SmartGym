using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Identity;

namespace GymWebApiBackend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();


            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
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

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
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

                var result = await userManager.CreateAsync(normalUser, "User123!");
                if (result.Succeeded)
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
            if (!dbContext.BerletTipusok.Any())
            {
                var berletTipusok = new[]
                {
                    new BerletTipus
                    {
                        Megnevezes = "Napijegy",
                        Ar = 1500,
                        IdotartamNapok = 1
                    },
                    new BerletTipus
                    {
                        Megnevezes = "Heti bérlet",
                        Ar = 7000,
                        IdotartamNapok = 7
                    },
                    new BerletTipus
                    {
                        Megnevezes = "Havi bérlet",
                        Ar = 20000,
                        IdotartamNapok = 30
                    },
                    new BerletTipus
                    {
                        Megnevezes = "Féléves bérlet",
                        Ar = 100000,
                        IdotartamNapok = 180
                    },
                    new BerletTipus
                    {
                        Megnevezes = "Éves bérlet",
                        Ar = 180000,
                        IdotartamNapok = 365
                    }
                };

                dbContext.BerletTipusok.AddRange(berletTipusok);
                dbContext.SaveChanges();
            }


            if (!dbContext.Szekrenyek.Any())
            {
                var szekrenyek = new List<Szekreny>();

                for (int i = 1; i <= 50; i++)
                {
                    szekrenyek.Add(new Szekreny
                    {
                        SzekrenySzam = i,
                        Aktiv = true
                    });
                }

                dbContext.Szekrenyek.AddRange(szekrenyek);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}