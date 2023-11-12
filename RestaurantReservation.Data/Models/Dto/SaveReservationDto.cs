using RestaurantReservation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Data.Models.Dto
{
    public class SaveReservationDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string MailAddress { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservationDate { get; set; }

    }
}
