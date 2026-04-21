using Content.Domain;
using Content.Service;

namespace Content.Data.Service.Interface
{
    public interface IMainService
    {
        ICartService cartService { get; }
        ClientService clientService { get; }
        ManagerService managerService { get; }
        ReservationService reservationService { get; }
        IShopItemService ShopItemService { get; }
        ShopService shopService { get; }
        ITicketService ticketService { get; }

        IEnumerable<Shop> GetShopsSortedByTickets();
    }
}