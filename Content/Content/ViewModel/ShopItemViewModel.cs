using Content.Domain;
using Content.Service;
using Content.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.ViewModel
{
    public class ShopItemsViewModel : INotifyPropertyChanged
    {
        private readonly MainService _service;
        private readonly UserSession _session;
        private readonly Shop _currentShop;

        private List<ShopItem> _allItems;
        public bool IsAdmin => _session.IsAdmin;
        public ObservableCollection<ShopItem> Items { get; set; }

        public ShopItemsViewModel(MainService service, UserSession session, Shop currentShop)
        {
            _service = service;
            _session = session;
            _currentShop = currentShop;
            LoadItems();
        }

        public void LoadItems()
        {
            var items = _service.shopItemService.GetShopItemsByShop(_currentShop.Id);
            _allItems = items.ToList();
            Items = new ObservableCollection<ShopItem>(items);
            OnPropertyChanged(nameof(Items));
        }

        public void AddItem(ShopItem item)
        {
            _service.shopItemService.AddShopItem(item);
            LoadItems();
        }

        public void DeleteItem(ShopItem item)
        {
            _service.shopItemService.RemoveShopItem(item.Id);
            LoadItems();
        }

        public void UpdateItem(ShopItem item)
        {
            _service.shopItemService.UpdateShopItem(item);
            LoadItems();
        }

        public void SortByName()
        {
            Items = new ObservableCollection<ShopItem>(
                _service.shopItemService.SortAlphabetically(_currentShop)
            );
            OnPropertyChanged(nameof(Items));
        }

        public void SortByPrice()
        {
            Items = new ObservableCollection<ShopItem>(
                _service.shopItemService.SortByPrice(_currentShop)
            );
            OnPropertyChanged(nameof(Items));
        }

        public void AddToCart(ShopItem item, int quantity)
        {
            _service.cartService.AddItemToCart(_session.UserId, new CartItem(item.Id, item, quantity));
        }

        public void Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                LoadItems(); // reset list
                return;
            }

            var filtered = _allItems
                .Where(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Items.Clear();
            foreach (var item in filtered)
                Items.Add(item);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}


/* Important note: this is for the shops page so we know which shop we're in and i can fetch the data
 private void ShopButton_Click(object sender, RoutedEventArgs e)
{
    var shop = (sender as Button).Tag as Shop;
    var shopItemsPage = new ShopItemsPage(_service, _session, shop);
    this.Frame.Navigate(typeof(ShopItemsPage), shopItemsPage);
}
*/