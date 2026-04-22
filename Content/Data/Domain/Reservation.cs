using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    public class Reservation
    {
        public int Id { get; set; }
        public Cart ReservationCart { get; set; }
        public bool Active { get; set; }
        public DateTime ReservationDate { get; set; }

        public Reservation(int id, Cart reservationCart, bool active, DateTime reservationDate)
        {
            this.Id = id;
            this.ReservationCart = reservationCart;
            this.Active = active;
            this.ReservationDate = reservationDate;
        }

        public Reservation(Cart reservationCart, bool active, DateTime reservationDate)
        {
            this.ReservationCart = reservationCart;
            this.Active = active;
            this.ReservationDate = reservationDate;
        }
    }
}
