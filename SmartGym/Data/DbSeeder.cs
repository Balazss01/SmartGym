using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartGym.Models;

namespace GymWebApiBackend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // --- SZEREPKÖRÖK ---
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // --- ADMIN FELHASZNÁLÓ ---
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

            // --- TESZT USER ÉS TAG ---
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

            // --- BÉRLET TÍPUSOK ---
            if (!dbContext.BerletTipusok.Any())
            {
                var berletTipusok = new[]
                {
                    new BerletTipus { Megnevezes = "Napijegy", Ar = 1500, IdotartamNapok = 1 },
                    new BerletTipus { Megnevezes = "Heti bérlet", Ar = 7000, IdotartamNapok = 7 },
                    new BerletTipus { Megnevezes = "Havi bérlet", Ar = 20000, IdotartamNapok = 30 },
                    new BerletTipus { Megnevezes = "Féléves bérlet", Ar = 100000, IdotartamNapok = 180 },
                    new BerletTipus { Megnevezes = "Éves bérlet", Ar = 180000, IdotartamNapok = 365 }
                };

                dbContext.BerletTipusok.AddRange(berletTipusok);
                await dbContext.SaveChangesAsync();
            }

            // --- SZEKRÉNYEK ---
            if (!dbContext.Szekrenyek.Any())
            {
                var szekrenyek = new List<Szekreny>();
                for (int i = 1; i <= 100; i++)
                {
                    szekrenyek.Add(new Szekreny { SzekrenySzam = i, Aktiv = true });
                }
                dbContext.Szekrenyek.AddRange(szekrenyek);
                await dbContext.SaveChangesAsync();
            }

            // --- HELYSZÍNEK (Konditermek) ---
            if (!dbContext.Helyszinek.Any())
            {
                var helyszinek = new List<Helyszin>
    {
        new Helyszin
        {
            Nev = "SmartGYM Budapest Belváros",
            Varos = "Budapest",
            Cim = "1051 Budapest, Váci utca 12.",
            Leiras = "Prémium konditerem a belváros szívében, modern gépekkel, személyi edzőkel és wellness részleggel.",
            Telefon = "+36 1 234 5678",
            Email = "budapest.belvaros@smartgym.hu",
            NyitvatartasHetfo = "06:00 – 22:00",
            NyitvatartasHeto = "08:00 – 20:00",
            NyitvatartasVasarnap = "09:00 – 18:00",
            Szelesseg = 47.4979,
            Hosszusag = 19.0402,
            Kep = "🏙️",
            Aktiv = true,
            Szolgaltatasok = new() { "Személyi edző", "Csoportos órák", "Sauna", "Parkoló", "Büfé" }
        },
        new Helyszin
        {
            Nev = "SmartGYM Budapest Budai",
            Varos = "Budapest",
            Cim = "1016 Budapest, Mészáros utca 5.",
            Leiras = "Nyugodt, intim légkörű edzőterem a budai oldalon, panorámás nézettel a Dunára.",
            Telefon = "+36 1 345 6789",
            Email = "budapest.budai@smartgym.hu",
            NyitvatartasHetfo = "06:00 – 21:00",
            NyitvatartasHeto = "08:00 – 20:00",
            NyitvatartasVasarnap = "09:00 – 17:00",
            Szelesseg = 47.4924,
            Hosszusag = 19.0366,
            Kep = "🌉",
            Aktiv = true,
            Szolgaltatasok = new() { "Személyi edző", "Yoga", "Pilates", "Szekrények" }
        },
        new Helyszin
        {
            Nev = "SmartGYM Debrecen",
            Varos = "Debrecen",
            Cim = "4024 Debrecen, Piac utca 28.",
            Leiras = "Kelet-Magyarország legnagyobb SmartGYM helyszíne. Három emelet, olimpiai emelvény.",
            Telefon = "+36 52 123 456",
            Email = "debrecen@smartgym.hu",
            NyitvatartasHetfo = "05:30 – 22:00",
            NyitvatartasHeto = "07:00 – 21:00",
            NyitvatartasVasarnap = "09:00 – 18:00",
            Szelesseg = 47.5316,
            Hosszusag = 21.6273,
            Kep = "🏋️",
            Aktiv = true,
            Szolgaltatasok = new() { "CrossFit", "Olimpiai emelés", "Személyi edző", "Parkoló", "Sauna" }
        },
        new Helyszin
        {
            Nev = "SmartGYM Miskolc",
            Varos = "Miskolc",
            Cim = "3525 Miskolc, Széchenyi István út 14.",
            Leiras = "Modern edzőközpont Miskolc centrumában. Teljes kardió és erő zóna.",
            Telefon = "+36 46 234 567",
            Email = "miskolc@smartgym.hu",
            NyitvatartasHetfo = "06:00 – 22:00",
            NyitvatartasHeto = "08:00 – 20:00",
            NyitvatartasVasarnap = "10:00 – 17:00",
            Szelesseg = 48.1035,
            Hosszusag = 20.7784,
            Kep = "⚡",
            Aktiv = true,
            Szolgaltatasok = new() { "Kardió", "Erőedzés", "Csoportos órák", "Szekrények" }
        },
        new Helyszin
        {
            Nev = "SmartGYM Pécs",
            Varos = "Pécs",
            Cim = "7621 Pécs, Király utca 40.",
            Leiras = "Mediterrán hangulatú edzőterem tetőteraszttal és úszómedencével.",
            Telefon = "+36 72 345 678",
            Email = "pecs@smartgym.hu",
            NyitvatartasHetfo = "06:00 – 22:00",
            NyitvatartasHeto = "08:00 – 21:00",
            NyitvatartasVasarnap = "09:00 – 18:00",
            Szelesseg = 46.0727,
            Hosszusag = 18.2330,
            Kep = "🌅",
            Aktiv = true,
            Szolgaltatasok = new() { "Uszoda", "Tetőterasz", "Személyi edző", "Büfé", "Sauna" }
        },
        new Helyszin
        {
            Nev = "SmartGYM Győr",
            Varos = "Győr",
            Cim = "9021 Győr, Baross Gábor út 17.",
            Leiras = "Ipari design, csúcskategóriás gépek, és 24/7-es hozzáférés.",
            Telefon = "+36 96 456 789",
            Email = "gyor@smartgym.hu",
            NyitvatartasHetfo = "00:00 – 23:59",
            NyitvatartasHeto = "00:00 – 23:59",
            NyitvatartasVasarnap = "00:00 – 23:59",
            Szelesseg = 47.6875,
            Hosszusag = 17.6504,
            Kep = "🔑",
            Aktiv = true,
            Szolgaltatasok = new() { "24/7 nyitva", "Kardió", "Erőedzés", "Parkoló", "Szekrények" }
        },
        new Helyszin
        {
            Nev = "SmartGYM Székesfehérvár",
            Varos = "Székesfehérvár",
            Cim = "8000 Székesfehérvár, Fő utca 3.",
            Leiras = "Tradicionális powerlifting zóna és modern funkcionális edzőtér.",
            Telefon = "+36 22 567 890",
            Email = "szfehervár@smartgym.hu",
            NyitvatartasHetfo = "06:00 – 22:00",
            NyitvatartasHeto = "08:00 – 20:00",
            NyitvatartasVasarnap = "09:00 – 17:00",
            Szelesseg = 47.1896,
            Hosszusag = 18.4116,
            Kep = "💪",
            Aktiv = true,
            Szolgaltatasok = new() { "Powerlifting", "Funkcionális edzés", "Személyi edző" }
        },
        new Helyszin
        {
            Nev = "SmartGYM Sopron",
            Varos = "Sopron",
            Cim = "9400 Sopron, Várkerület 22.",
            Leiras = "Osztrák stílusú wellness és erőteljes edzőtér kombinációja.",
            Telefon = "+36 99 678 901",
            Email = "sopron@smartgym.hu",
            NyitvatartasHetfo = "06:00 – 21:00",
            NyitvatartasHeto = "07:00 – 20:00",
            NyitvatartasVasarnap = "09:00 – 17:00",
            Szelesseg = 47.6849,
            Hosszusag = 16.5855,
            Kep = "🏰",
            Aktiv = true,
            Szolgaltatasok = new() { "Wellness", "Sauna", "Gőzfürdő", "Személyi edző", "Parkoló" }
        }
    };

                dbContext.Helyszinek.AddRange(helyszinek);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}