using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Content.Repository;
using Content.Repository.Database;
using Content.Domain;
using Content.Data.Service.Interface;
namespace Content.Service
{
    public class MainService : IMainService
    {
        public ICartService CartService { get; }
        public ShopService ShopService { get; }
        public ITicketService TicketService { get; }
        public ClientService ClientService { get; }
        public ManagerService ManagerService { get; }
        public ReservationService ReservationService { get; }
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
            this.ShopService = new ShopService(shopRepo);
            this.CartService = new CartService(cartRepo, ShopItemService);
            this.TicketService = new TicketService(ticketRepo);
            this.ClientService = new ClientService(clientRepo);
            this.ManagerService = new ManagerService(managerRepo);

            this.ReservationService = new ReservationService(reservationRepo, ShopItemService, CartService);
        }

        public IEnumerable<Shop> GetShopsSortedByTickets()
        {
            var shops = ShopService.GetAllAvailableShops();
            return shops.OrderBy(s => TicketService.CountTicketsBySubcategory(s.Name));
        }
    }
}