using System.Collections.ObjectModel;
using System.ComponentModel;
using Content.Domain;
using Content.Service;
using Content.User;

namespace Content.ViewModel
{
    public class ShopItemsViewModel : INotifyPropertyChanged
    {
        private readonly IShopItemService shopItemService;
        private readonly ICartService cartService;
        private readonly UserSession session;
        private readonly Shop currentShop;

        public bool IsAdmin => this.session.IsAdmin;
        public ObservableCollection<ShopItem> Items { get; set; } = new ObservableCollection<ShopItem>();

        public ShopItemsViewModel(IShopItemService shopItemService, ICartService cartService, UserSession session, Shop currentShop)
        {
            this.shopItemService = shopItemService;
            this.cartService = cartService;
            this.session = session;
            this.currentShop = currentShop;
            LoadItems();
        }

        public void LoadItems()
        {
            Items = new ObservableCollection<ShopItem>(
                this.shopItemService.GetItemsByShopId(currentShop.Id));
            OnPropertyChanged(nameof(Items));
        }

        public void AddItem(ShopItem item)
        {
            this.shopItemService.AddShopItem(item);
            LoadItems();
        }

        public void DeleteItem(ShopItem item)
        {
            this.shopItemService.RemoveShopItem(item.Id);
            LoadItems();
        }

        public void UpdateItem(ShopItem item)
        {
            this.shopItemService.UpdateShopItem(item);
            LoadItems();
        }

        public void SortByName()
        {
            Items = new ObservableCollection<ShopItem>(
                this.shopItemService.GetItemsSortedAlphabetically(currentShop));
            OnPropertyChanged(nameof(Items));
        }

        public void SortByPrice()
        {
            Items = new ObservableCollection<ShopItem>(
                this.shopItemService.GetItemsSortedByPrice(currentShop));
            OnPropertyChanged(nameof(Items));
        }

        public void AddToCart(ShopItem item, int quantity)
        {
            this.cartService.AddItemToCart(this.session.UserId, new CartItem(item.Id, item, quantity));
        }

        public void Search(string query)
        {
            Items = new ObservableCollection<ShopItem>(
                this.shopItemService.SearchItemsByName(currentShop.Id, query));
            OnPropertyChanged(nameof(Items));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}