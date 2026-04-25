using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface ITicketService
    {
        int CountTicketsBySubcategory(string subcategory);

        void AddTicket(Ticket ticket);
    }
}
