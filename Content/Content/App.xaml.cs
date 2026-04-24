using System;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Database;
using Content.Repository.Interface;
using Content.Service;
using Content.User;
using Content.ViewModel;
using Content.ViewModel.Interface;
using Content.Data.ViewModel.Interface;

namespace Content
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public static MainWindow MainWindow { get; private set; } = null!;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Services = ConfigureServices();

            MainWindow = new MainWindow();
            MainWindow.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            string conn = ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            var services = new ServiceCollection();

            services.AddSingleton<IClientRepo>(_ => new ClientDbRepo(conn));
            services.AddSingleton<ITicketRepo>(_ => new TicketDbRepo(conn));
            services.AddSingleton<IManagerRepo>(_ => new ManagerDbRepo(conn));
            services.AddSingleton<IShopRepo>(_ => new ShopDbRepo(conn));
            services.AddSingleton<IShopItemRepo>(_ => new ShopItemDbRepo(conn));
            services.AddSingleton<ICartRepo>(sp => new CartDbRepo(
                conn,
                sp.GetRequiredService<IClientRepo>(),
                sp.GetRequiredService<IShopItemRepo>()));
            services.AddSingleton<IReservationRepo>(sp => new ReservationDbRepo(
                conn,
                sp.GetRequiredService<ICartRepo>()));

            services.AddSingleton<IShopItemService, ShopItemService>();
            services.AddSingleton<IShopService, ShopService>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<ITicketService, TicketService>();
            services.AddSingleton<IClientService, ClientService>();
            services.AddSingleton<IManagerService, ManagerService>();
            services.AddSingleton<IReservationService, ReservationService>();

            services.AddSingleton<UserSession>();

            services.AddTransient<ILandingViewModel, LandingViewModel>();
            services.AddTransient<IShopPageViewModel, ShopPageViewModel>();
            services.AddTransient<ICartViewModel, CartViewModel>();

            services.AddSingleton<Func<Shop, IShopItemsViewModel>>(sp => shop =>
                new ShopItemsViewModel(
                    sp.GetRequiredService<IShopItemService>(),
                    sp.GetRequiredService<ICartService>(),
                    sp.GetRequiredService<UserSession>(),
                    shop));

            services.AddSingleton<Func<ShopItem, Shop, IItemDetailsViewModel>>(sp => (item, shop) =>
                new ItemDetailsViewModel(
                    sp.GetRequiredService<ICartService>(),
                    sp.GetRequiredService<IShopItemService>(),
                    sp.GetRequiredService<UserSession>(),
                    item,
                    shop));

            return services.BuildServiceProvider();
        }
    }
}
