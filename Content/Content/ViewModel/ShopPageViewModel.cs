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
        private List<Shop> _allShops;

        public ObservableCollection<Shop> Shops { get; set; }
        private int nextId = 1;

        public bool IsAdmin => _session.IsAdmin;
        public Visibility AddShopVisibility => _session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;

        public bool IsCartEnabled => !_session.IsAdmin;
        public double CartOpacity => _session.IsAdmin ? 0.4 : 1.0;

        public ICommand SelectClientCommand { get; }

        public ShopPageViewModel(MainService service, UserSession session)
        {
            _service = service;
            _session = session;
            LoadItems();

        }

        public void LoadItems()
        {
            var shops = _service.shopService.GetAllAvailableShops();
            _allShops = shops.ToList();
            Shops = new ObservableCollection<Shop>(shops);
            OnPropertyChanged(nameof(Shops));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public void AddShop(string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            _service.shopService.AddShop(new Shop(nextId, name, type, _session.UserId));
            LoadItems();
        }

        public void EditShop(Shop shop, string newName, string newType)
        {
            _service.shopService.Update(new Shop(shop.Id, newName, newType, _session.UserId));
            shop.Name = newName;
            shop.Type = newType;
            LoadItems();
        }

        public void DeleteShop(Shop shop)
        {
            _service.shopService.DeleteShop(shop.Id);
            LoadItems();
        }

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

        

    }
}