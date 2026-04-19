using Content.Domain;
using Content.Repository.Interface;

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
            return this.ticketRepo.CountBySubcategory(subcategory);
        }

        public void AddTicket(Ticket ticket)
        {
            this.ticketRepo.Add(ticket);
        }
    }
}
