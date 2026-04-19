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
        private readonly MainService _service;
        private readonly UserSession _session;

        private ShopItem _item;
        private Cart _cart;

        public ICommand AddToCartCommand { get; }
        public ICommand UpdateItemCommand { get; }

        public ItemDetailsViewModel(MainService service, UserSession session, ShopItem item, Cart cart)
        {
            _service = service;
            _session = session;
            _item = item;
            _cart = cart;

            AddToCartCommand = new RelayCommand<int>(AddToCart);
            UpdateItemCommand = new RelayCommand<ShopItem>(UpdateItem);
        }

        private void AddToCart(int quantity)
        {
            var cartItems = _cart.CartItems;
            CartItem? existingCartItem = null;
            foreach (var ci in cartItems.Values)
            {
                if (ci.ShopItem?.Id == _item.Id)
                {
                    existingCartItem = ci;
                    break;
                }
            }
            var newStockQuantity = _item.Quantity - quantity;
            if (existingCartItem != null)
            {
                var newCartQuantity = existingCartItem.Quantity + quantity;
                _service.cartService.UpdateItemQuantity(_cart.Id, existingCartItem.Id, newCartQuantity);
            }
            else
            {
                _service.cartService.AddItemToCart(_cart.Id, new CartItem(0, _item, quantity));
            }

            _service.ShopItemService.UpdateShopItem(
                new ShopItem(
                    _item.Id,
                    newStockQuantity,
                    _item.Price,
                    _item.ShopId,
                    _item.Photo,
                    _item.Name,
                    _item.Description
                )
            );
        }

        private void UpdateItem(ShopItem updatedItem)
        {
            _service.ShopItemService.UpdateShopItem(
                new ShopItem(
                    updatedItem.Id,
                    updatedItem.Quantity,
                    updatedItem.Price,
                    updatedItem.ShopId,
                    updatedItem.Photo,
                    updatedItem.Name,
                    updatedItem.Description
                )
            );
        }


    }


}