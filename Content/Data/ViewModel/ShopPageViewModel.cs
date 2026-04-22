using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Content.Domain;
using Content.Service;
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

        public ObservableCollection<Shop> Shops { get; } = new ObservableCollection<Shop>();

        public bool IsAdmin => session.IsAdmin;
        public bool CanAddShop => session.IsAdmin;
        public bool IsCartEnabled => !session.IsAdmin;
        public double CartOpacity => session.IsAdmin ? 0.4 : 1.0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ShopPageViewModel(IShopService shopService, ITicketService ticketService, UserSession session)
        {
            this.shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            this.ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            LoadItems();
        }

        public void LoadItems()
        {
            ReplaceShops(shopService.GetAllAvailableShops());
        }

        public void AddShop(string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Shop name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Shop type cannot be empty");
            }

            shopService.AddShop(new Shop(name, type, session.UserId));
            LoadItems();
        }

        public void EditShop(Shop shop, string newName, string newType)
        {
            if (shop == null)
            {
                throw new ArgumentNullException(nameof(shop));
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Shop name cannot be empty.", nameof(newName));
            }

            if (string.IsNullOrWhiteSpace(newType))
            {
                throw new ArgumentException("Shop type cannot be empty.", nameof(newType));
            }

            shopService.UpdateShop(new Shop(shop.Id, newName, newType, session.UserId));
            LoadItems();
        }

        public void DeleteShop(Shop shop)
        {
            if (shop == null)
            {
                throw new ArgumentNullException(nameof(shop));
            }

            shopService.DeleteShop(shop.Id);
            LoadItems();
        }

        public void Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                LoadItems();
                return;
            }

            ReplaceShops(shopService.SearchByName(query));
        }

        public void SortByReviews()
        {
            var sorted = shopService
                .GetAllAvailableShops()
                .OrderBy(s => ticketService.CountTicketsBySubcategory(s.Name));
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
    }
}
