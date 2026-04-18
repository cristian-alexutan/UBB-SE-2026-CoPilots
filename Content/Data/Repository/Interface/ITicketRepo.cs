using Content.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository.Interface
{
    public interface ITicketRepo
    {
        IEnumerable<Ticket> GetAll();
        Ticket GetById(int Id);
        void Add(Ticket Ticket);
        void Delete(int Id);
        int CountBySubcategory(string Subcategory);
    }
}
