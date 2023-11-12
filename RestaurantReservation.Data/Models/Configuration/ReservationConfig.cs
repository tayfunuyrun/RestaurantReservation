using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantReservation.Core.Enums;
using RestaurantReservation.Data.Models.Entities;

namespace ReservationProject.Data.Models.Configuration
{
    public class ReservationConfig : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> entity)
        {
            entity.ToTable("Reservation");

            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(reservation => !reservation.Deleted);
            entity.Property(e => e.ApprovalDate).HasColumnType("datetime");
            entity.Property(e => e.ReservationDate).HasColumnType("datetime");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(x => x.Status)
                .HasConversion(
                    v => Convert.ToByte(v),
                    v => (ReservationStatuses)Enum.Parse(typeof(ReservationStatuses), v.ToString()));
          
        }
    }
}
