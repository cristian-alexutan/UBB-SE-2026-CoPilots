using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface IReservationRepo
    {
        IEnumerable<Reservation> GetAll();
        Reservation GetById(int reservationId);
        void Add(Reservation reservation);
        void Delete(int reservationId);
        void Update(Reservation reservation);
    }
}
