using Content.Domain;
using Content.Service;

namespace Content.Data.Service.Interface
{
    public interface IMainService
    {
        ICartService CartService { get; }
        ClientService ClientService { get; }
        ManagerService ManagerService { get; }
        ReservationService ReservationService { get; }
        IShopItemService ShopItemService { get; }
        ShopService ShopService { get; }
        ITicketService TicketService { get; }

        IEnumerable<Shop> GetShopsSortedByTickets();
    }
}