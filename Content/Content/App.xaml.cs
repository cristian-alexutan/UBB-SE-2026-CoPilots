using System.Configuration;
using Microsoft.UI.Xaml;
using Content.Data.Service.Interface;
using Content.Repository.Database;
using Content.Service;
using Content.User;

namespace Content
{
    public partial class App : Application
    {
        public static IClientService ClientService { get; private set; } = null!;
        public static IManagerService ManagerService { get; private set; } = null!;
        public static IShopService ShopService { get; private set; } = null!;
        public static IShopItemService ShopItemService { get; private set; } = null!;
        public static ICartService CartService { get; private set; } = null!;
        public static ITicketService TicketService { get; private set; } = null!;
        public static IReservationService ReservationService { get; private set; } = null!;
        public static UserSession Session { get; private set; } = null!;
        public static MainWindow MainWindow { get; private set; } = null!;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            string conn = ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            var clientRepo = new ClientDbRepo(conn);
            var ticketRepo = new TicketDbRepo(conn);
            var managerRepo = new ManagerDbRepo(conn);
            var shopRepo = new ShopDbRepo(conn);
            var shopItemRepo = new ShopItemDbRepo(conn);
            var cartRepo = new CartDbRepo(conn, clientRepo, shopItemRepo);
            var reservationRepo = new ReservationDbRepo(conn, cartRepo);

            ShopItemService = new ShopItemService(shopItemRepo);
            ShopService = new ShopService(shopRepo);
            CartService = new CartService(cartRepo, ShopItemService);
            TicketService = new TicketService(ticketRepo);
            ClientService = new ClientService(clientRepo);
            ManagerService = new ManagerService(managerRepo);
            ReservationService = new ReservationService(reservationRepo, ShopItemService, CartService);

            Session = new UserSession();
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
