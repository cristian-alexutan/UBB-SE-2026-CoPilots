using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    class Reservation
    {

        private int id;
        private Cart reservationCart;
        private bool active;
        private DateTime reservationDate;

        public Reservation(int id, Cart reservationCart, bool active, DateTime reservationDate)
        {
            this.id = id;
            this.reservationCart = reservationCart;
            this.active = active;
            this.reservationDate = reservationDate;
        }

        public int getId() { return id; }
        public Cart getReservationCart() { return reservationCart; }
        public bool getActive() { return active; }
        public DateTime getReservationDate() { return reservationDate; }
        public void setId(int id) { this.id = id; }
        public void setReservationCart(Cart reservationCart) { this.reservationCart = reservationCart; }
        public void setActive(bool active) { this.active = active; }
        public void setReservationDate(DateTime reservationDate) { this.reservationDate = reservationDate; }

    }
}
