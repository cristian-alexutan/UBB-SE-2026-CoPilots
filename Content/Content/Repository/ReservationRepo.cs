using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ReservationRepo
    {
        private List<Domain.Reservation> reservations;

        public ReservationRepo()
        {
            reservations = new List<Domain.Reservation>();
        }

        public void addReservation(Domain.Reservation reservation)
        {
            reservations.Insert(reservation.getId(), reservation);
        }

        public void deleteReservation(int id)
        {
            reservations.RemoveAt(id);
        }

        public List<Domain.Reservation> getAllReservations()
        {
            return reservations;
        }

        public Domain.Reservation getReservationById(int id)
        {
            return reservations[id];
        }



    }
}
