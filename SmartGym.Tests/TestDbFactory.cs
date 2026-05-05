using GymWebApiBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartGym.Tests
{
    public static class TestDbFactory
    {
        public static ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
