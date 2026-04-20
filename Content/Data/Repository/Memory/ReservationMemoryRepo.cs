using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Repository
{
    public class ReservationMemoryRepo : IReservationRepo
    {
        private readonly Dictionary<int, Reservation> reservations;
        private int nextId;

        public ReservationMemoryRepo()
        {
            this.reservations = new Dictionary<int, Reservation>();
            this.nextId = 1;
        }

        public void Add(Reservation Reservation)
        {
            Reservation.Id = this.nextId++;
            this.reservations[Reservation.Id] = Reservation;
        }

        public void Delete(int Id)
        {
            this.reservations.Remove(Id);
        }

        public IEnumerable<Reservation> GetAll()
        {
            return this.reservations.Values;
        }

        public Reservation GetById(int Id)
        {
            return this.reservations.ContainsKey(Id) ? this.reservations[Id] : null!;
        }

        public void Update(Reservation reservation)
        {
            if (this.reservations.ContainsKey(reservation.Id))
            {
                this.reservations[reservation.Id] = reservation;
            }
        }
    }
}
