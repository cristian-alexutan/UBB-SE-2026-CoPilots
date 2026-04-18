using Content.Service;
using System.Configuration;
using Content.Repository;
using Content.Repository.Database;
using Content.Domain;
using System.Collections.Generic;
using System.Linq;
namespace Content.Service
{
    public class MainService
    {
        public CartService cartService { get; }
        public ShopService shopService { get; }
        public TicketService ticketService { get; }
        public ClientService clientService { get; }
        public ManagerService managerService { get; }
        public ReservationService reservationService { get; }
        public ShopItemService shopItemService { get; }

        public MainService(string connectionString)
        {

            var clientRepo = new ClientDbRepo(connectionString);
            var ticketRepo = new TicketDbRepo(connectionString);
            var managerRepo = new ManagerDbRepo(connectionString);
            var shopRepo = new ShopDbRepo(connectionString, managerRepo);
            var shopItemRepo = new ShopItemDbRepo(connectionString, shopRepo);
            var cartRepo = new CartDbRepo(connectionString, clientRepo, shopItemRepo);
            var reservationRepo = new ReservationDbRepo(connectionString, cartRepo);


            this.cartService = new CartService(cartRepo);
            this.shopService = new ShopService(shopRepo);
            this.ticketService = new TicketService(ticketRepo);
            this.clientService = new ClientService(clientRepo);
            this.managerService = new ManagerService(managerRepo);
            this.reservationService = new ReservationService(reservationRepo);
            this.shopItemService = new ShopItemService(shopItemRepo);
        }


        public IEnumerable<Shop> GetShopsSortedByTickets()
        {
            var shops = shopService.GetAllAvailableShops();
            return shops.OrderBy(s => ticketService.CountTicketsBySubcategory(s.Name));
        }

    }

}