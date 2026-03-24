using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    public class ReservationMemoryRepo : IReservationRepo
    {
        private Dictionary<int,Domain.Reservation> Reservations;

        public ReservationMemoryRepo()
        {
            Reservations = new Dictionary<int,Domain.Reservation>();
        }

        public void Add(Domain.Reservation Reservation)
        {
            Reservations[Reservation.Id]=Reservation;
        }

        public void Delete(int Id)
        {
            Reservations.Remove(Id);
        }

        public IEnumerable<Reservation> GetAll()
        {
            return Reservations.Values;
        }

        public Reservation GetById(int Id)
        {
            Reservations.TryGetValue(Id, out Reservation Reservation);
            return Reservation;
        }



    }
}
