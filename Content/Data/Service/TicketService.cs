using System.Linq;
using Content.Domain;
using Content.Repository.Interface;
using Content.Data.Service.Interface;

namespace Content.Service
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepo ticketRepo;

        public TicketService(ITicketRepo ticketRepo)
        {
            this.ticketRepo = ticketRepo;
        }

        public int CountTicketsBySubcategory(string subcategory)
        {
            if (string.IsNullOrEmpty(subcategory))
            {
                throw new ArgumentException("Subcategory cannot be null or empty.", nameof(subcategory));
            }

            return this.ticketRepo.GetAll().Count(ticket => ticket.Subcategory == subcategory);
        }

        public void AddTicket(Ticket ticket)
        {
            this.ticketRepo.Add(ticket);
        }
    }
}
