using Api;
using EF_Core_Perf.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EF_Core_Perf
{
    public class HighwayDbContext : DbContext
    {
        public HighwayDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected HighwayDbContext()
        {
        }

        public DbSet<RoadEntity> Roads { get; set; }

        public DbSet<SqlResult> RawResult { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SqlResult>().HasNoKey();
        }
    }

    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<HighwayDbContext>
    {
        public HighwayDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HighwayDbContext>();
            optionsBuilder.UseSqlite("Data Source=blog.db");

            return new HighwayDbContext(optionsBuilder.Options);
        }
    }
}
