using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Content.Data.Service.Interface;
using Content.Data.ViewModel.Interface;
using Content.Domain;
using Content.User;
using Content.ViewModel.Interface;

namespace Content.ViewModel
{
    public class ShopPageViewModel : IShopPageViewModel
    {
        private readonly IShopService shopService;
        private readonly ITicketService ticketService;
        private readonly UserSession session;
        private List<Shop> allShops = new ();
        private double clientCartOpacity = 1.0;
        private double adminCartOpacity = 0.4;

        public ObservableCollection<Shop> Shops { get; } = new ObservableCollection<Shop>();

        public bool IsAdmin => session.IsAdmin;
        public bool CanAddShop => session.IsAdmin;
        public bool IsCartEnabled => !session.IsAdmin;
        public double CartOpacity => session.IsAdmin ? adminCartOpacity : clientCartOpacity;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ShopPageViewModel(IShopService shopService, ITicketService ticketService, UserSession session)
        {
            this.shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            this.ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            LoadShops();
        }

        public void AddShop(string name, string type)
        {
            shopService.AddShop(new Shop(name, type, session.UserId));
            LoadShops();
        }

        public void EditShop(Shop shop, string newName, string newType)
        {
            shopService.UpdateShop(new Shop(shop.Id, newName, newType, session.UserId));
            LoadShops();
        }

        public void DeleteShop(Shop shop)
        {
            shopService.DeleteShop(shop.Id);
            LoadShops();
        }

        public void Search(string query)
        {
            ReplaceShops(shopService.SearchByName(query));
        }

        public void SortByReviews()
        {
            var sorted = shopService
                .GetAllAvailableShops()
                .OrderBy(shop => ticketService.CountTicketsBySubcategory(shop.Name));
            ReplaceShops(sorted);
        }

        public void SortAlphabetically()
        {
            ReplaceShops(shopService.SortAlphabetically(allShops));
        }

        private void ReplaceShops(IEnumerable<Shop> shops)
        {
            allShops = shops.ToList();
            Shops.Clear();
            foreach (var shop in allShops)
            {
                Shops.Add(shop);
            }
        }
        public void LoadShops()
        {
            this.ReplaceShops(shopService.GetAllAvailableShops());
        }
    }
}
