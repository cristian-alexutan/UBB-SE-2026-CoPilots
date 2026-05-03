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
using Content.Repository;

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

        protected override void OnLaunched(LaunchActivatedEventArgs launchActivatedEventArgs)
        {
            Services = ConfigureServices();

            MainWindow = new MainWindow();
            MainWindow.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            string connectionString = ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            var serviceCollection = new ServiceCollection();
            var databaseConnectionFactory = new DatabaseConnectionFactory();
            serviceCollection.AddSingleton(databaseConnectionFactory);
            serviceCollection.AddSingleton<IClientRepo>(_ => new ClientDbRepo(connectionString));
            serviceCollection.AddSingleton<ITicketRepo>(_ => new TicketDbRepo(connectionString));
            serviceCollection.AddSingleton<IManagerRepo>(_ => new ManagerDbRepo(connectionString));
            serviceCollection.AddSingleton<IShopRepo>(_ => new ShopDbRepo(databaseConnectionFactory));
            serviceCollection.AddSingleton<IShopItemRepo>(_ => new ShopItemDbRepo(connectionString));
            serviceCollection.AddSingleton<ICartRepo>(_ => new CartDbRepo(connectionString));
            serviceCollection.AddSingleton<IReservationRepo>(_ => new ReservationDbRepo(connectionString));

            serviceCollection.AddSingleton<IShopItemService, ShopItemService>();
            serviceCollection.AddSingleton<IShopService, ShopService>();
            serviceCollection.AddSingleton<ICartService, CartService>();
            serviceCollection.AddSingleton<ITicketService, TicketService>();
            serviceCollection.AddSingleton<IClientService, ClientService>();
            serviceCollection.AddSingleton<IManagerService, ManagerService>();
            serviceCollection.AddSingleton<IReservationService, ReservationService>();

            serviceCollection.AddSingleton<UserSession>();

            serviceCollection.AddTransient<ILandingViewModel, LandingViewModel>();
            serviceCollection.AddTransient<IShopPageViewModel, ShopPageViewModel>();
            serviceCollection.AddTransient<ICartViewModel, CartViewModel>();

            serviceCollection.AddSingleton<Func<Shop, IShopItemsViewModel>>(serviceProvider => shop =>
                new ShopItemsViewModel(
                    serviceProvider.GetRequiredService<IShopItemService>(),
                    serviceProvider.GetRequiredService<ICartService>(),
                    serviceProvider.GetRequiredService<UserSession>(),
                    shop));

            serviceCollection.AddSingleton<Func<ShopItem, Shop, IItemDetailsViewModel>>(serviceProvider => (shopItem, shop) =>
                new ItemDetailsViewModel(
                    serviceProvider.GetRequiredService<ICartService>(),
                    serviceProvider.GetRequiredService<IShopItemService>(),
                    serviceProvider.GetRequiredService<UserSession>(),
                    shopItem,
                    shop));

            return serviceCollection.BuildServiceProvider();
        }
    }
}
