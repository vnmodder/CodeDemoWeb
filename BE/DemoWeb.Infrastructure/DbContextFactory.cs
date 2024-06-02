using DemoWeb.Infrastructure.Constants;
using DemoWeb.Infrastructure.Databases.DemoWebDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DemoWeb.Infrastructure
{
    public interface IDbContextFactory
    {
        public DemoWebDbContext CreateDemoWebDbContextInstance();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly DbContextOptions<DemoWebDbContext> _demoWebDbContextDbContextOptions;

        public DbContextFactory(IConfiguration configuration)
        {
            _demoWebDbContextDbContextOptions = new DbContextOptionsBuilder<DemoWebDbContext>()
                .UseSqlServer(
                    connectionString: configuration.GetConnectionString("DemoWeb"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.CommandTimeout((int)TimeSpan.FromSeconds(SettingConstants.DatabaseSettings.TIMEOUT_FROM_SECONDS).TotalSeconds);
                        //sqlOptions.EnableRetryOnFailure();
                    })
                .EnableDetailedErrors()
                .Options;
        }

        public DemoWebDbContext CreateDemoWebDbContextInstance()
        {
            return new DemoWebDbContext(_demoWebDbContextDbContextOptions);
        }
    }
}
