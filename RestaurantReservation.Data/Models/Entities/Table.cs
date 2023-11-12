using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Data.Models.Entities
{
    public class Table
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int TableCapacity { get; set; }


        //Navigation Prop
        public ICollection<Reservation> Reservations { get; set; }
    }
}
