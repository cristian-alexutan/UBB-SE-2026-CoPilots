using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Content.Domain;
using Content.Data.Service.Interface;
using Content.User;
using Content.ViewModel.Interface;

namespace Content.ViewModel
{
    public class ShopItemsViewModel : IShopItemsViewModel
    {
        private readonly IShopItemService shopItemService;
        private readonly ICartService cartService;
        private readonly UserSession session;
        private readonly Shop currentShop;

        public bool IsAdmin => this.session.IsAdmin;

        public bool CanAddItem => this.session.IsAdmin;

        public bool IsCartEnabled => !this.session.IsAdmin;

        public ObservableCollection<ShopItem> Items { get; } = new ObservableCollection<ShopItem>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ShopItemsViewModel(IShopItemService shopItemService, ICartService cartService, UserSession session, Shop currentShop)
        {
            this.shopItemService = shopItemService ?? throw new ArgumentNullException(nameof(shopItemService));
            this.cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.currentShop = currentShop ?? throw new ArgumentNullException(nameof(currentShop));
            this.LoadItems();
        }

        public void LoadItems()
        {
            this.ReplaceItems(this.shopItemService.GetItemsByShopId(this.currentShop.Id));
        }

        public void AddItem(string name, string description, string priceText, string quantityText, string imagePath)
        {
            if (!float.TryParse(priceText, out float price))
            {
                throw new ArgumentException("Price is not a valid number.");
            }

            if (!int.TryParse(quantityText, out int quantity))
            {
                throw new ArgumentException("Quantity is not a valid number.");
            }

            this.shopItemService.AddShopItem(new ShopItem(quantity, price, this.currentShop.Id, imagePath, name, description));
            this.LoadItems();
        }

        public void UpdateItem(ShopItem item, string name, string description, string priceText, string quantityText, string imagePath)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!float.TryParse(priceText, out float price))
            {
                throw new ArgumentException("Price is not a valid number.");
            }

            if (!int.TryParse(quantityText, out int quantity))
            {
                throw new ArgumentException("Quantity is not a valid number.");
            }

            this.shopItemService.UpdateShopItem(new ShopItem(item.Id, quantity, price, item.ShopId, imagePath, name, description));
            this.LoadItems();
        }

        public void DeleteItem(ShopItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            this.shopItemService.RemoveShopItem(item.Id);
            this.LoadItems();
        }

        public void AddToCart(ShopItem item, int quantity)
        {
            this.cartService.GetOrCreateCart(this.session.UserId);
            this.cartService.AddItemToCart(this.session.UserId, new CartItem(item.Id, item, quantity));
        }

        public void SortByName()
        {
            this.ReplaceItems(this.shopItemService.GetItemsSortedAlphabetically(this.currentShop));
        }

        public void SortByPrice()
        {
            this.ReplaceItems(this.shopItemService.GetItemsSortedByPrice(this.currentShop));
        }

        public void Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                this.LoadItems();
                return;
            }

            this.ReplaceItems(this.shopItemService.SearchItemsByName(this.currentShop.Id, query));
        }

        private void ReplaceItems(IEnumerable<ShopItem> items)
        {
            this.Items.Clear();
            foreach (var item in items)
            {
                this.Items.Add(item);
            }
        }
    }
}
