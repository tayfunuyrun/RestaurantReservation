using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Data.Models.Dto
{
    public class TableDto
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int TableCapacity { get; set; }
    }
}
