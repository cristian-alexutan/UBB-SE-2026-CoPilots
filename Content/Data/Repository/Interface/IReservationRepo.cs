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
        Reservation GetById(int id);
        void Add(Reservation reservation);
        void Delete(int id);
        void Update(Reservation reservation);
    }
}
