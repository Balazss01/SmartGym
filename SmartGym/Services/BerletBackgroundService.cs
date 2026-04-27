using GymWebApiBackend.Data;
using GymWebApiBackend.Models;
using Microsoft.EntityFrameworkCore;

public class BerletBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BerletBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var most = DateTime.Now;

            var aktivando = await context.Berletek
                .Where(b => b.Aktiv && b.KezdetDatum <= most && b.KezdetDatum > most.AddSeconds(-30))
                .ToListAsync();

            foreach (var b in aktivando)
            {
                context.Ertesitesek.Add(new Ertesites
                {
                    TagId = b.TagId,
                    Uzenet = $"Bérleted most aktiválódott! ({b.KezdetDatum:yyyy.MM.dd})",
                    Olvasott = false,
                    Datum = most
                });
            }

            await context.SaveChangesAsync();

            await Task.Delay(30000, stoppingToken);
        }
    }
}