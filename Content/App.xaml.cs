using System;
using System.Configuration;
using Microsoft.UI.Xaml;
using Content.Repository.Database;
using Content.Repository;
using Content.Domain;
using Content.Service; 

namespace Content
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Database repositories
            var clientDbRepo = new ClientDbRepo(conn);
            var managerDbRepo = new ManagerDbRepo(conn);
            var shopDbRepo = new ShopDbRepo(conn,managerDbRepo);
            var shopItemDbRepo = new ShopItemDbRepo(conn, shopDbRepo);
            var cartDbRepo = new CartDbRepo(conn, clientDbRepo, shopItemDbRepo);
            var ticketDbRepo = new TicketDbRepo(conn);
            var reservationDbRepo = new ReservationDbRepo(conn, cartDbRepo);

            // Service layer
            var managerService = new ManagerService(managerDbRepo);
            var ticketService = new TicketService(ticketDbRepo);
            var cartService = new CartService(cartDbRepo);
            var shopService = new ShopService(shopDbRepo);
            var shopItemService = new ShopItemService(shopItemDbRepo);
            var clientService = new ClientService(clientDbRepo);
            var reservationService = new ReservationService(reservationDbRepo);

            //Main service
            var mainService = new MainService(cartService, shopService, null, clientService, managerService, reservationService, shopItemService);

            m_window = new MainWindow(mainService);
            m_window.Activate();
        }

        private Window? m_window;
    }
}