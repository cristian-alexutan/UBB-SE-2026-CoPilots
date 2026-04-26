using System.Collections.Generic;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface ITicketRepo
    {
        IEnumerable<Ticket> GetAll();

        Ticket GetById(int ticketId);

        void Add(Ticket ticket);

        void Delete(int ticketId);

        int CountBySubcategory(string subcategory);
    }
}
