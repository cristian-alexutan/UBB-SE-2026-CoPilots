using Content.Service;
using System.Configuration;

public class MainService
{
    public CartService cartService { get; }
    public ShopService shopService { get; }
    public TicketService ticketService { get; }
    public ClientService clientService { get; }
    public ManagerService managerService { get; }
    public ReservationService reservationService { get; }
    public ShopItemService shopItemService { get; }

    public MainService(CartService cartService, ShopService shopService, TicketService ticketService, ClientService clientService, ManagerService managerService, ReservationService reservationService, ShopItemService shopItemService)
    {
        this.cartService = cartService;
        this.shopService = shopService;
        this.ticketService = ticketService;
        this.clientService = clientService;
        this.managerService = managerService;
        this.reservationService = reservationService;
        this.shopItemService = shopItemService;
    }
}