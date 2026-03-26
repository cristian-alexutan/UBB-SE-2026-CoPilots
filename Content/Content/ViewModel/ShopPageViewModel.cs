using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Content.Domain;
using Content.Service;
using Content.User;
using System.Windows.Input;

namespace Content.ViewModel
{
    public class ShopPageViewModel
    {
        private readonly MainService _service;
        private readonly UserSession _session;

        public ObservableCollection<Shop> Shops { get; set; } = new ObservableCollection<Shop>();
        private int nextId = 1;

        public bool IsAdmin => _session.IsAdmin;
        public Visibility AddShopVisibility => _session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdminVisibility => _session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        public bool IsCartEnabled => !_session.IsAdmin;
        public double CartOpacity => _session.IsAdmin ? 0.4 : 1.0;

        public ICommand SelectClientCommand { get; }

        public ShopPageViewModel(MainService service, UserSession session)
        {
            _service = service;
            _session = session;

            Shops.Add(new Shop(nextId++, "Chocolate Heaven", "Food", null));
            Shops.Add(new Shop(nextId++, "Cosmetics Corner", "Beauty", null));
            Shops.Add(new Shop(nextId++, "Designer Bags", "Fashion", null));
            Shops.Add(new Shop(nextId++, "Gourmet Delights", "Food", null));
            Shops.Add(new Shop(nextId++, "Luxury Boutique", "Fashion", null));
        }

        public void AddShop(string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            Shops.Add(new Shop(nextId++, name, type, null));
        }

        public void EditShop(Shop shop, string newName, string newType)
        {
            shop.Name = newName;
            shop.Type = newType;
        }

        public void DeleteShop(Shop shop)
        {
            Shops.Remove(shop);
        }
    }
}