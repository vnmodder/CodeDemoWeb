using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DemoWeb.Infrastructure.Databases.DemoWebDB.Entities;
using Microsoft.EntityFrameworkCore;
using DemoWeb.Infrastructure.Databases.DemoWebDB.Extensions;

namespace DemoWeb.Infrastructure.Databases.DemoWebDB
{
    public class DemoWebDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DemoWebDbContext(DbContextOptions<DemoWebDbContext> options)
     : base(options)
        {
        }

        #region DbSet declarations.
        public virtual DbSet<User> UserTables { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configuration off IdentityDbContext.
            base.OnModelCreating(modelBuilder);

            // Add dumydata
            modelBuilder.AddDummyData();

            // Apply all configurations that have attached to DbContext.DbSets.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DemoWebDbContext).Assembly, entity => entity.Namespace.Contains("DemoWebDb"));
        }
    }
}
