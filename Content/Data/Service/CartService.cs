namespace Content.Service
{
    using System;
    using System.Collections.Generic;
    using Content.Domain;
    using Content.Repository.Interface;

    public class CartService : ICartService
    {
        private const int MinimumCartItemQuantity = 1;
        private readonly ICartRepo cartRepo;
        private readonly IShopItemService shopItemService;

        public CartService(ICartRepo cartRepo, IShopItemService shopItemService)
        {
            this.cartRepo = cartRepo;
            this.shopItemService = shopItemService;
        }

        public IEnumerable<Cart> GetAllCarts()
        {
            var carts = this.cartRepo.GetAll();
            foreach (var cart in carts)
            {
                cart.CartItems = this.BuildCartItemList(cart.Id);
            }
            return carts;
        }

        public Cart GetCartById(int id)
        {
            var cart = this.cartRepo.GetById(id);
            if (cart != null)
            {
                cart.CartItems = this.BuildCartItemList(id);
            }
            return cart;
        }

        public Cart GetOrCreateCart(int userId)
        {
            var cart = this.cartRepo.GetById(userId);
            if (cart == null)
            {
                cart = new Cart(userId, new Client(userId, "Current Client"), new Dictionary<int, CartItem>());
                this.cartRepo.Add(cart);
            }
            else
            {
                cart.CartItems = this.BuildCartItemList(userId);
            }
            return cart;
        }

        public void AddCart(Cart cart)
        {
            this.cartRepo.Add(cart);
        }

        public void DeleteCart(int id)
        {
            this.cartRepo.Delete(id);
        }

        public void AddItemToCart(int cartId, CartItem item)
        {
            var rawItems = this.cartRepo.GetRawCartItems(cartId);
            int existingCartItemId = 0;
            int existingQuantity = 0;
            bool existingFound = false;

            foreach (var (cartItemId, itemId, quantity) in rawItems)
            {
                if (itemId == item.ShopItem.Id)
                {
                    existingCartItemId = cartItemId;
                    existingQuantity = quantity;
                    existingFound = true;
                    break;
                }
            }

            var shopItem = this.shopItemService.GetById(item.ShopItem.Id);
            int alreadyInCartQuantity = 0;
            if (existingFound)
            {
                alreadyInCartQuantity = existingQuantity;
            }
            int totalQuantity = alreadyInCartQuantity + item.Quantity;

            if (shopItem == null || totalQuantity > shopItem.Quantity)
            {
                throw new InvalidOperationException("Not enough stock.");
            }

            if (existingFound)
            {
                this.cartRepo.UpdateItemQuantity(cartId, existingCartItemId, totalQuantity);
            }
            else
            {
                this.cartRepo.AddItemToCart(cartId, item);
            }
        }

        public void RemoveItemFromCart(int cartId, int cartItemId)
        {
            this.cartRepo.RemoveItemFromCart(cartId, cartItemId);
        }

        public void UpdateItemQuantity(int cartId, int cartItemId, int quantity)
        {
            var rawItems = this.cartRepo.GetRawCartItems(cartId);
            int foundItemId = 0;
            bool found = false;

            foreach (var (rawCartItemId, itemId, rawQuantity) in rawItems)
            {
                if (rawCartItemId == cartItemId)
                {
                    foundItemId = itemId;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                var shopItem = this.shopItemService.GetById(foundItemId);
                if (shopItem != null && quantity > shopItem.Quantity)
                {
                    throw new InvalidOperationException("Not enough stock.");
                }
            }

            this.cartRepo.UpdateItemQuantity(cartId, cartItemId, quantity);
        }

        public void ClearCart(int cartId)
        {
            this.cartRepo.ClearCart(cartId);
        }

        public double GetCartTotal(int cartId)
        {
            var cart = this.cartRepo.GetById(cartId);
            if (cart == null)
            {
                return 0;
            }

            double total = 0;
            var cartItems = this.BuildCartItemList(cartId);
            foreach (var cartItem in cartItems.Values)
            {
                total += cartItem.GetTotalPrice();
            }
            return total;
        }

        public void DecreaseItemQuantity(int cartId, int cartItemId)
        {
            var rawItems = this.cartRepo.GetRawCartItems(cartId);
            int foundQuantity = 0;
            bool found = false;

            foreach (var (rawCartItemId, itemId, quantity) in rawItems)
            {
                if (rawCartItemId == cartItemId)
                {
                    foundQuantity = quantity;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return;
            }

            if (foundQuantity > MinimumCartItemQuantity)
            {
                this.cartRepo.UpdateItemQuantity(cartId, cartItemId, foundQuantity - MinimumCartItemQuantity);
            }
            else
            {
                this.cartRepo.RemoveItemFromCart(cartId, cartItemId);
            }
        }

        public bool IsLastCartItem(int cartId, int cartItemId)
        {
            var rawItems = this.cartRepo.GetRawCartItems(cartId);

            foreach (var (rawCartItemId, itemId, quantity) in rawItems)
            {
                if (rawCartItemId == cartItemId)
                {
                    return quantity == MinimumCartItemQuantity;
                }
            }

            return false;
        }

        private Dictionary<int, CartItem> BuildCartItemList(int cartId)
        {
            var result = new Dictionary<int, CartItem>();
            var rawItems = this.cartRepo.GetRawCartItems(cartId);
            foreach (var (cartItemId, itemId, quantity) in rawItems)
            {
                var shopItem = this.shopItemService.GetById(itemId);
                if (shopItem != null)
                {
                    result[cartItemId] = new CartItem(cartItemId, shopItem, quantity);
                }
            }
            return result;
        }
    }
}