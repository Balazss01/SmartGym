using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GymFrontend.Tests
{
    public class FrontendTesztek : IClassFixture<TesztAlkalmazas>
    {
        private readonly TesztAlkalmazas _factory;

        public FrontendTesztek(TesztAlkalmazas factory) => _factory = factory;

        // A főoldal betölthető és HTML választ ad
        [Fact]
        public async Task Foooldal_BetolthetoEsHtmlValasztAd()
        {
            var client = _factory.CreateClient();

            var valasz = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.OK, valasz.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                valasz.Content.Headers.ContentType?.ToString());
        }

        // A Kapcsolat oldal megjelenik és tartalmaz lényeges szövegeket
        [Fact]
        public async Task KapcsolatOldal_MegfeleloSzTartalmatTartalmaz()
        {
            var client = _factory.CreateClient();

            var valasz = await client.GetAsync("/Kapcsolat");
            var html = await valasz.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, valasz.StatusCode);
            Assert.Contains("Kapcsolat", html);
            Assert.Contains("info@smartgym.hu", html);
            Assert.Contains("Üzenet küldése", html);
        }

        // Login form üres adatokkal újra megjeleníti az oldalt (validáció miatt)
        [Fact]
        public async Task Login_UresAdatokkalHibaUzenet()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

            var loginOldal = await client.GetAsync("/Login");
            var loginHtml = await loginOldal.Content.ReadAsStringAsync();
            var antiForgery = AntiForgeryTokenKinyer(loginHtml);

            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", ""),
                new KeyValuePair<string, string>("Password", ""),
                new KeyValuePair<string, string>("__RequestVerificationToken", antiForgery)
            });

            var valasz = await client.PostAsync("/Login", form);

            Assert.Equal(HttpStatusCode.OK, valasz.StatusCode);
        }

        // A Galéria oldal elérhető bejelentkezés nélkül is
        [Fact]
        public async Task GaleriaOldal_BejelentkezesNelkulIsBetolthet()
        {
            var client = _factory.CreateClient();

            var valasz = await client.GetAsync("/Galeria");

            Assert.Equal(HttpStatusCode.OK, valasz.StatusCode);
        }

        private static string AntiForgeryTokenKinyer(string html)
        {
            const string kulcs = "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"";
            var index = html.IndexOf(kulcs);
            if (index < 0) return "";

            var kezdo = index + kulcs.Length;
            var veg = html.IndexOf("\"", kezdo);
            return html[kezdo..veg];
        }
    }
}
