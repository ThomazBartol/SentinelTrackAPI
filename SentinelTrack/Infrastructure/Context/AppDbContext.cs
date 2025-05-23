using Microsoft.EntityFrameworkCore;
using SentinelTrack.Domain.Entities;

namespace SentinelTrack.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Moto> Motos { get; set; }
        public DbSet<Yard> Yards { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Moto>().ToTable("ST_Motos");
            modelBuilder.Entity<Yard>().ToTable("ST_Yards");
        }
    }
}
