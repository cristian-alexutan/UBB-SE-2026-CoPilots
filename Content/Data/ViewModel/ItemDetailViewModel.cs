using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Content.Service;
using Content.User;
using Content.Domain;
using Content.Helper;

namespace Content.ViewModel
{
    public class ItemDetailsViewModel
    {
        private readonly MainService service;
        private readonly UserSession session;

        private ShopItem item;
        private Cart cart;

        public ICommand AddToCartCommand { get; }
        public ICommand UpdateItemCommand { get; }

        public ItemDetailsViewModel(MainService service, UserSession session, ShopItem item, Cart cart)
        {
            this.service = service;
            this.session = session;
            this.item = item;
            this.cart = cart;

            AddToCartCommand = new RelayCommand<int>(AddToCart);
            UpdateItemCommand = new RelayCommand<ShopItem>(UpdateItem);
        }

        private void AddToCart(int quantity)
        {
            var cartItems = cart.CartItems;
            CartItem? existingCartItem = null;
            foreach (var ci in cartItems.Values)
            {
                if (ci.ShopItem?.Id == item.Id)
                {
                    existingCartItem = ci;
                    break;
                }
            }
            var newStockQuantity = item.Quantity - quantity;
            if (existingCartItem != null)
            {
                var newCartQuantity = existingCartItem.Quantity + quantity;
                service.CartService.UpdateItemQuantity(cart.Id, existingCartItem.Id, newCartQuantity);
            }
            else
            {
                service.CartService.AddItemToCart(cart.Id, new CartItem(0, item, quantity));
            }

            service.ShopItemService.UpdateShopItem(
                new ShopItem(
                    item.Id,
                    newStockQuantity,
                    item.Price,
                    item.ShopId,
                    item.Photo,
                    item.Name,
                    item.Description));
        }

        private void UpdateItem(ShopItem updatedItem)
        {
            service.ShopItemService.UpdateShopItem(
                new ShopItem(
                    updatedItem.Id,
                    updatedItem.Quantity,
                    updatedItem.Price,
                    updatedItem.ShopId,
                    updatedItem.Photo,
                    updatedItem.Name,
                    updatedItem.Description));
        }
    }
}