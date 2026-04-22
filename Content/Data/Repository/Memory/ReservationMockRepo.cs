using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Repository
{
    public class ReservationMockRepo : IReservationRepo
    {
        private readonly Dictionary<int, Reservation> reservations;
        private int nextId;

        public ReservationMockRepo()
        {
            this.reservations = new Dictionary<int, Reservation>();
            this.nextId = 1;
        }

        public void Add(Reservation reservation)
        {
            reservation.Id = this.nextId++;
            this.reservations[reservation.Id] = reservation;
        }

        public void Delete(int id)
        {
            this.reservations.Remove(id);
        }

        public IEnumerable<Reservation> GetAll()
        {
            return this.reservations.Values;
        }

        public Reservation GetById(int id)
        {
            return this.reservations.ContainsKey(id) ? this.reservations[id] : null!;
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
