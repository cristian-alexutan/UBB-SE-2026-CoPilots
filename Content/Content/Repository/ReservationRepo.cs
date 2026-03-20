using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ReservationRepo
    {
        private Dictionary<int,Domain.Reservation> reservations;

        public ReservationRepo()
        {
            reservations = new Dictionary<int,Domain.Reservation>();
        }

        public void addReservation(Domain.Reservation reservation)
        {
            reservations[reservation.getId()]=reservation;
        }

        public void deleteReservation(int id)
        {
            reservations.Remove(id);
        }

        public Dictionary<int,Domain.Reservation> getAllReservations()
        {
            return reservations;
        }

        public Domain.Reservation getReservationById(int id)
        {
            return reservations[id];
        }



    }
}
