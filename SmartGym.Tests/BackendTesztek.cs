using System.Security.Claims;
using GymWebApiBackend.Controllers;
using GymWebApiBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace SmartGym.Tests
{
    public class BackendTesztek
    {
        private static void BeallitBejelentkezve<T>(T controller, int tagId)
            where T : ControllerBase
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, tagId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // Egy felhasználó csak a saját bérleteit látja
        [Fact]
        public async Task BerletekDb_CsakAFelhasznaloSajatRekordjait()
        {
            using var db = TestDbFactory.CreateInMemoryDb();

            db.BerletTipusok.Add(new BerletTipus
            {
                BerletTipusId = 1,
                Megnevezes = "Havi bérlet",
                Ar = 20000,
                IdotartamNapok = 30
            });
            await db.SaveChangesAsync();

            db.Berletek.Add(new Berlet
            {
                BerletId = 1,
                TagId = 100,
                BerletTipusId = 1,
                KezdetDatum = DateTime.Now.AddDays(-5),
                VegeDatum = DateTime.Now.AddDays(25),
                Aktiv = true
            });

            db.Berletek.Add(new Berlet
            {
                BerletId = 2,
                TagId = 200,
                BerletTipusId = 1,
                KezdetDatum = DateTime.Now.AddDays(-10),
                VegeDatum = DateTime.Now.AddDays(20),
                Aktiv = true
            });

            await db.SaveChangesAsync();

            // Anna (TagId=100) szűrése
            var annaBerletei = db.Berletek
                .Where(b => b.TagId == 100)
                .ToList();

            Assert.Single(annaBerletei);
            Assert.Equal(1, annaBerletei[0].BerletId);
        }

        // Bérlet vásárlása nem létező típussal hibát ad
        [Fact]
        public async Task CreateBerlet_NemLetezoTipusEseteenBadRequest()
        {
            using var db = TestDbFactory.CreateInMemoryDb();
            var controller = new BerletekController(db);
            BeallitBejelentkezve(controller, tagId: 100);

            var dto = new GymWebApiBackend.DTOs.CreateBerletDto
            {
                BerletTipusId = 9999
            };

            var eredmeny = await controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(eredmeny);
        }

        // A státusz endpoint helyesen jelzi ha bent van a felhasználó
        [Fact]
        public async Task GetStatusz_BentVanFelhasznaloeseteenIgazatVisszaadja()
        {
            using var db = TestDbFactory.CreateInMemoryDb();

            db.Belepesek.Add(new Belepes
            {
                BelepesId = 1,
                TagId = 100,
                BelepesIdopont = DateTime.Now.AddMinutes(-30),
                KilepesIdopont = null
            });

            await db.SaveChangesAsync();

            var controller = new BelepesekController(db);
            BeallitBejelentkezve(controller, tagId: 100);

            var eredmeny = await controller.GetStatusz();

            var okResult = Assert.IsType<OkObjectResult>(eredmeny);

            var bentVan = okResult.Value!.GetType()
                .GetProperty("bentVan")!
                .GetValue(okResult.Value);

            Assert.Equal(true, bentVan);
        }

        // Kilépett felhasználó már nincs bent
        [Fact]
        public async Task GetStatusz_KilepettFelhasznaloMarNincsBent()
        {
            using var db = TestDbFactory.CreateInMemoryDb();

            db.Belepesek.Add(new Belepes
            {
                BelepesId = 1,
                TagId = 100,
                BelepesIdopont = DateTime.Now.AddHours(-2),
                KilepesIdopont = DateTime.Now.AddMinutes(-15)
            });

            await db.SaveChangesAsync();

            var controller = new BelepesekController(db);
            BeallitBejelentkezve(controller, tagId: 100);

            var eredmeny = await controller.GetStatusz();

            var okResult = Assert.IsType<OkObjectResult>(eredmeny);

            var bentVan = okResult.Value!.GetType()
                .GetProperty("bentVan")!
                .GetValue(okResult.Value);

            Assert.Equal(false, bentVan);
        }
    }
}
