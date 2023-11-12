using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

using RestaurantReservation.Core.Enums;

namespace RestaurantReservation.Data.Models.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ReservationNo { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string MailAddress { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public int TableId { get; set; }
        public ReservationStatuses Status { get; set; }
        public bool Deleted{ get; set; }

        //Navigation Prop
        public virtual Table Table { get; set; }


    }
}
