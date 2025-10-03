using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;

namespace RapidOrder.Infrastructure
{
    public class RapidOrderDbContext : DbContext
    {
        public RapidOrderDbContext(DbContextOptions<RapidOrderDbContext> options) : base(options)
        {
        }
        public DbSet<PlaceGroup> PlaceGroups => Set<PlaceGroup>();
        public DbSet<Place> Places => Set<Place>();
        public DbSet<Setup> Setups => Set<Setup>();
        public DbSet<CallButton> CallButtons => Set<CallButton>();
        public DbSet<Mission> Missions => Set<Mission>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Watch> Watches => Set<Watch>();
        public DbSet<EventLog> EventLogs => Set<EventLog>();
        public DbSet<ActionMap> ActionMaps => Set<ActionMap>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RapidOrderDbContext).Assembly);

            modelBuilder.Entity<Place>().HasIndex(p => p.Number).IsUnique(false);
            modelBuilder.Entity<CallButton>().HasIndex(cb => cb.DeviceCode).IsUnique();
            modelBuilder.Entity<ActionMap>().HasIndex(am => new { am.DeviceCode, am.ButtonNumber }).IsUnique();

            modelBuilder.Entity<Mission>().HasIndex(m => new { m.PlaceId, m.StartedAt });
          
            

            base.OnModelCreating(modelBuilder);
        }
    }
}
