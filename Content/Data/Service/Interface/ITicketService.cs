using Content.Domain;

namespace Content.Service
{
    public interface ITicketService
    {
        int CountTicketsBySubcategory(string subcategory);

        void AddTicket(Ticket ticket);
    }
}
