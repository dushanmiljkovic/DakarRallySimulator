using DakarRallySimulator.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace DakarRallySimulator.Db
{
    public class DakarRellyContext : DbContext
    {
        public DakarRellyContext(DbContextOptions<DakarRellyContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>()
             .HasOne(s => s.Race)
             .WithMany(g => g.Vehicles)
             .HasForeignKey(s => s.RaceId);

            base.OnModelCreating(modelBuilder);

        }
        //public DbSet<Ranking> Rankings { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

    }
}
