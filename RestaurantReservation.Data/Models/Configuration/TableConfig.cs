using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using RestaurantReservation.Data.Models.Entities;

namespace RestaurantReservation.Data.Models.Configuration
{
    public class TableConfig : IEntityTypeConfiguration<Table>
    {
        public void Configure(EntityTypeBuilder<Table> entity)
        {
            entity.ToTable("Table");

            entity.HasKey(e => e.Id);
            
            entity.HasMany(e => e.Reservations)
                .WithOne(res => res.Table)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.TableId)
                .IsRequired();
        }
    }
}
