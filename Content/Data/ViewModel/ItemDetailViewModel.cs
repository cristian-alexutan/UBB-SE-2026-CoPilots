using System.Windows.Input;
using Content.Domain;
using Content.Helper;
using Content.Service;
using Content.User;
using Content.ViewModel.Interface;

namespace Content.ViewModel
{
    public class ItemDetailsViewModel : IItemDetailsViewModel
    {
        private readonly ICartService cartService;
        private readonly IShopItemService shopItemService;
        private readonly UserSession session;

        private ShopItem item;
        private Cart cart;

        public ICommand AddToCartCommand { get; }
        public ICommand UpdateItemCommand { get; }

        public ItemDetailsViewModel(ICartService cartService, IShopItemService shopItemService, UserSession session, ShopItem item, Cart cart)
        {
            this.cartService = cartService;
            this.shopItemService = shopItemService;
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
                cartService.UpdateItemQuantity(cart.Id, existingCartItem.Id, newCartQuantity);
            }
            else
            {
                cartService.AddItemToCart(cart.Id, new CartItem(0, item, quantity));
            }

            shopItemService.UpdateShopItem(
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
            shopItemService.UpdateShopItem(
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
