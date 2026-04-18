using Content.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository.Interface
{
    public interface IReservationRepo
    {
        IEnumerable<Reservation> GetAll();
        Reservation GetById(int Id);
        void Add(Reservation Reservation);
        void Delete(int Id);
        void Update(Reservation reservation);
    }
}
