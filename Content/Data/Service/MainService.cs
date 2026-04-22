using System.Configuration;
using Content.Repository;
using Content.Repository.Database;
using Content.Domain;
using System.Collections.Generic;
using System.Linq;
using Content.Data.Service.Interface;
namespace Content.Service
{
    public class MainService : IMainService
    {
        public ICartService cartService { get; }
        public ShopService shopService { get; }
        public ITicketService ticketService { get; }
        public ClientService clientService { get; }
        public ManagerService managerService { get; }
        public ReservationService reservationService { get; }
        public IShopItemService ShopItemService { get; }

        public MainService(string connectionString)
        {

            var clientRepo = new ClientDbRepo(connectionString);
            var ticketRepo = new TicketDbRepo(connectionString);
            var managerRepo = new ManagerDbRepo(connectionString);
            var shopRepo = new ShopDbRepo(connectionString);
            var shopItemRepo = new ShopItemDbRepo(connectionString);
            var cartRepo = new CartDbRepo(connectionString, clientRepo, shopItemRepo);
            var reservationRepo = new ReservationDbRepo(connectionString, cartRepo);


            this.ShopItemService = new ShopItemService(shopItemRepo);
            this.shopService = new ShopService(shopRepo);
            this.cartService = new CartService(cartRepo, ShopItemService);
            this.ticketService = new TicketService(ticketRepo);
            this.clientService = new ClientService(clientRepo);
            this.managerService = new ManagerService(managerRepo);
            
            this.reservationService = new ReservationService(reservationRepo,ShopItemService,cartService);
            
        }


        public IEnumerable<Shop> GetShopsSortedByTickets()
        {
            var shops = shopService.GetAllAvailableShops();
            return shops.OrderBy(s => ticketService.CountTicketsBySubcategory(s.Name));
        }

    }

}