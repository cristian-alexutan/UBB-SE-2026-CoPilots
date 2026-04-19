using System.Collections.Generic;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface ITicketRepo
    {
        IEnumerable<Ticket> GetAll();

        Ticket GetById(int id);

        void Add(Ticket ticket);

        void Delete(int id);

        int CountBySubcategory(string subcategory);
    }
}
