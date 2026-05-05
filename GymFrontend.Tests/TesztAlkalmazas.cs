using System.Net.Http;
using GymFrontend;
using GymWebApiBackend.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymFrontend.Tests
{
    public class TesztAlkalmazas : WebApplicationFactory<Program>
    {
        public Func<HttpRequestMessage, HttpResponseMessage>? ApiValaszGenerator { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("FrontendTesztDb"));

                services.AddHttpClient("Api")
                    .ConfigurePrimaryHttpMessageHandler(() =>
                        new MockApiHandler(this));
            });
        }

        private class MockApiHandler : HttpMessageHandler
        {
            private readonly TesztAlkalmazas _factory;

            public MockApiHandler(TesztAlkalmazas factory) => _factory = factory;

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_factory.ApiValaszGenerator == null)
                {
                    return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotImplemented));
                }
                return Task.FromResult(_factory.ApiValaszGenerator(request));
            }
        }
    }
}
