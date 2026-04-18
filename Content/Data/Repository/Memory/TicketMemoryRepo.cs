using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;
using Content.Repository.Interface;
namespace Content.Repository.Memory
{
    public class TicketMemoryRepo : ITicketRepo
    {
        private Dictionary<int, Ticket> Tickets;

        public TicketMemoryRepo()
        {
            Tickets = new Dictionary<int, Ticket>();
        }

        public IEnumerable<Ticket> GetAll()
        {
            return Tickets.Values;
        }

        public Ticket GetById(int Id)
        {
            Tickets.TryGetValue(Id, out Ticket Ticket);
            return Ticket;
        }

        public void Add(Ticket Ticket)
        {
            Tickets[Ticket.Id] = Ticket;
        }

        public void Delete(int Id)
        {
            Tickets.Remove(Id);
        }

        public int CountBySubcategory(string Subcategory)
        {
            return Tickets.Values.Count(t =>t.Category=="Duty Free Shops" && t.Subcategory == Subcategory);
        }
    }
}
