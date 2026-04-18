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

        public Reservation(int Id, Cart ReservationCart, bool Active, DateTime ReservationDate)
        {
            this.Id = Id;
            this.ReservationCart = ReservationCart;
            this.Active = Active;
            this.ReservationDate = ReservationDate;
        }

        
    }
}
