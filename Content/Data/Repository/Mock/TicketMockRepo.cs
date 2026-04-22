using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Repository.Memory
{
    public class TicketMockRepo : ITicketRepo
    {
        private Dictionary<int, Ticket> tickets;

        public TicketMockRepo()
        {
            this.tickets = new Dictionary<int, Ticket>();
        }

        public IEnumerable<Ticket> GetAll()
        {
            return this.tickets.Values;
        }

        public Ticket GetById(int id)
        {
            this.tickets.TryGetValue(id, out Ticket ticket);
            return ticket;
        }

        public void Add(Ticket ticket)
        {
            this.tickets[ticket.Id] = ticket;
        }

        public void Delete(int id)
        {
            this.tickets.Remove(id);
        }

        public int CountBySubcategory(string subcategory)
        {
            return this.tickets.Values.Count(t => t.Category == "Duty Free Shops" && t.Subcategory == subcategory);
        }
    }
}
