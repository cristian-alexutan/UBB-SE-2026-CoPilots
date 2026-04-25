using System;
using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Content.Data.Service.Interface;

namespace Content.Service
{
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
            return this.cartRepo.GetAll();
        }

        public Cart GetCartById(int cartId)
        {
            return this.cartRepo.GetById(cartId);
        }

        public Cart GetOrCreateCart(int userId)
        {
            var cart = this.cartRepo.GetById(userId);
            if (cart == null)
            {
                cart = new Cart(userId, new Client(userId, "Current Client"), new Dictionary<int, CartItem>());
                this.cartRepo.Add(cart);
            }

            return cart;
        }

        public void AddCart(Cart cart)
        {
            this.cartRepo.Add(cart);
        }

        public void DeleteCart(int cartId)
        {
            this.cartRepo.Delete(cartId);
        }

        public void AddItemToCart(int cartId, CartItem item)
        {
            var cart = this.cartRepo.GetById(cartId);
            CartItem existing = null;
            if (cart != null)
            {
                foreach (var currentCartItem in cart.CartItems.Values)
                {
                    if (currentCartItem.ShopItem?.Id == item.ShopItem.Id)
                    {
                        existing = currentCartItem;
                        break;
                    }
                }
            }

            var shopItem = this.shopItemService.GetById(item.ShopItem.Id);
            int totalQuantity = (existing?.Quantity ?? 0) + item.Quantity;

            if (totalQuantity > shopItem.Quantity)
            {
                throw new InvalidOperationException("Not enough stock.");
            }

            if (existing != null)
            {
                this.cartRepo.UpdateItemQuantity(cartId, existing.Id, totalQuantity);
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
            var cart = this.cartRepo.GetById(cartId);
            CartItem cartItem = null;
            if (cart != null)
            {
                foreach (var currentCartItem in cart.CartItems.Values)
                {
                    if (currentCartItem.Id == cartItemId)
                    {
                        cartItem = currentCartItem;
                        break;
                    }
                }
            }

            if (cartItem != null)
            {
                var shopItem = this.shopItemService.GetById(cartItem.ShopItem.Id);
                if (quantity > shopItem.Quantity)
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

            return cart.GetOverallPrice();
        }

        public void DecreaseItemQuantity(int cartId, int cartItemId)
        {
            var cart = this.cartRepo.GetById(cartId);
            CartItem cartItem = null;
            if (cart != null)
            {
                foreach (var currentCartItem in cart.CartItems.Values)
                {
                    if (currentCartItem.Id == cartItemId)
                    {
                        cartItem = currentCartItem;
                        break;
                    }
                }
            }

            if (cartItem == null)
            {
                return;
            }

            if (cartItem.Quantity > MinimumCartItemQuantity)
            {
                this.cartRepo.UpdateItemQuantity(cartId, cartItemId, cartItem.Quantity - MinimumCartItemQuantity);
            }
            else
            {
                this.cartRepo.RemoveItemFromCart(cartId, cartItemId);
            }
        }

        public bool IsLastCartItem(int cartId, int cartItemId)
        {
            var cart = this.cartRepo.GetById(cartId);
            if (cart == null)
            {
                return false;
            }

            CartItem cartItem = null;
            foreach (var currentCartItem in cart.CartItems.Values)
            {
                if (currentCartItem.Id == cartItemId)
                {
                    cartItem = currentCartItem;
                    break;
                }
            }

            return cartItem != null && cartItem.Quantity == MinimumCartItemQuantity;
        }
    }
}