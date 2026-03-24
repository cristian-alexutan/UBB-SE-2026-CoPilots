using Content.Domain;
using Content.Repository.Interface;

namespace Content.Service
{
    public class TicketService
    {
        private readonly ITicketRepo _ticketRepo;

        public TicketService(ITicketRepo ticketRepo)
        {
            _ticketRepo = ticketRepo;
        }

        public int CountTicketsBySubcategory(string subcategory)
        {
            return _ticketRepo.CountBySubcategory(subcategory);
        }

        public void AddTicket(Ticket ticket)
        {
            _ticketRepo.Add(ticket);
        }
    }
}