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
            this.cartService.AddItemToCart(this.cart.Id, new CartItem(0, this.item, quantity));
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
