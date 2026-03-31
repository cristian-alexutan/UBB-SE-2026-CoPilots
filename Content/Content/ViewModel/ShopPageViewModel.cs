using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Content.Domain;
using Content.Service;
using Content.User;
using System.Windows.Input;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System;
namespace Content.ViewModel
{
    public class ShopPageViewModel : INotifyPropertyChanged
    {
        private readonly MainService _service;
        private readonly UserSession _session;
        // This is the master list used for sorting
        private List<Shop> _allShops;

        // This is bound to the gridview
        public ObservableCollection<Shop> Shops { get; } = new ObservableCollection<Shop>();
        // Added these for clarity
        public bool IsAdmin => _session.IsAdmin;
        public Visibility AddShopVisibility => _session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        public bool IsCartEnabled => !_session.IsAdmin;
        public double CartOpacity => _session.IsAdmin ? 0.4 : 1.0;

        public ShopPageViewModel(MainService service, UserSession session)
        {
            _service = service;
            _session = session;
            LoadItems();

        }

        // Fetches all available shops from the service and refreshes the observable collection.
        public void LoadItems()
        {
            var shops = _service.shopService.GetAllAvailableShops();
            _allShops = shops.ToList();
            Shops.Clear();
            foreach (var s in _allShops)
                Shops.Add(s);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Adds a shop and refreshes the list via LoadItems()
        public void AddShop(string name, string type)
        {
            // Here 0 is a throwaway value, since we use identity on the database shop_id the passed value doesn't matter
            if (string.IsNullOrWhiteSpace(name)) return;
            _service.shopService.AddShop(new Shop(0, name, type, _session.UserId));
            LoadItems();
        }

        // Updates the shop based on user input and refreshes the list
        public void EditShop(Shop shop, string newName, string newType)
        {
            _service.shopService.Update(new Shop(shop.Id, newName, newType, _session.UserId));
            shop.Name = newName;
            shop.Type = newType;
            LoadItems();
        }

        // Deletes the shop and refreshes the list
        public void DeleteShop(Shop shop)
        {
            _service.shopService.DeleteShop(shop.Id);
            LoadItems();
        }
        // Filters the shops based on user input (case insensitive)
        public void Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                LoadItems();
                return;
            }

            var filtered = _allShops
                .Where(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Shops.Clear();
            foreach (var item in filtered)
                Shops.Add(item);
        }

        // Sorts the shops based on complaints
        public void SortByReviews()
        {
            if (_allShops == null) return;

            var sorted = _service.GetShopsSortedByTickets().ToList();

            _allShops = sorted;

            Shops.Clear();
            foreach (var shop in sorted)
                Shops.Add(shop);
        }

        // Sorts the shops based on name
        public void SortAlphabetically()
        {
            if (_allShops == null) return;

            var sorted = _service.shopService.SortAlphabetically(_allShops).ToList();

            Shops.Clear();
            foreach (var shop in sorted)
                Shops.Add(shop);
        }


    }
}