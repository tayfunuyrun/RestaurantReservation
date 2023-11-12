using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Data.Models.Entities;

namespace RestaurantReservation.Data.Models
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationDbContext).Assembly);
        }

        public virtual DbSet<Table> Tables { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;



    }
}
