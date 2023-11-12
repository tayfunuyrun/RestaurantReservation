
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Data.Models.Entities;

namespace ReservationProject.Data.Models.Configuration
{
    public class DbGeneratedValueConfiguration : IEntityTypeConfiguration<DbGeneratedValue>
    {
        public void Configure(EntityTypeBuilder<DbGeneratedValue> entity)
        {
            entity.HasNoKey();
        }
    }
}