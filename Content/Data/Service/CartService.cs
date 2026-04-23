using System;
using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Service
{
    public class CartService : ICartService
    {
        private readonly ICartRepo cartRepo;
        private readonly IShopItemService shopItemService;
        private const int MinimumCartItemQuantity = 1;

        public CartService(ICartRepo cartRepo, IShopItemService shopItemService)
        {
            this.cartRepo = cartRepo;
            this.shopItemService = shopItemService;
        }

        public IEnumerable<Cart> GetAllCarts()
        {
            return this.cartRepo.GetAll();
        }

        public Cart GetCartById(int id)
        {
            return this.cartRepo.GetById(id);
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

        public void DeleteCart(int id)
        {
            this.cartRepo.Delete(id);
        }

        public void AddItemToCart(int cartId, CartItem item)
        {
            var cart = this.cartRepo.GetById(cartId);
            CartItem existing = null;
            if (cart != null)
            {
                foreach (var ci in cart.CartItems.Values)
                {
                    if (ci.ShopItem?.Id == item.ShopItem.Id)
                    {
                        existing = ci;
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
                foreach (var ci in cart.CartItems.Values)
                {
                    if (ci.Id == cartItemId)
                    {
                        cartItem = ci;
                        break;
                    }
                }
            }
            if (cartItem != null)
            {
                var shopItem = this.shopItemService.GetById(cartItem.ShopItem.Id);
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

            return cart.GetOverallPrice();
        }
        public void DecreaseItemQuantity(int cartId, int cartItemId)
        {
            var cart = this.cartRepo.GetById(cartId);
            CartItem cartItem = null;
            if (cart != null)
            {
                foreach (var ci in cart.CartItems.Values)
                {
                    if (ci.Id == cartItemId)
                    {
                        cartItem = ci;
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
                this.cartRepo.UpdateItemQuantity(cartId, cartItemId, cartItem.Quantity - 1);
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
            foreach (var ci in cart.CartItems.Values)
            {
                if (ci.Id == cartItemId)
                {
                    cartItem = ci;
                    break;
                }
            }
            return cartItem != null && cartItem.Quantity == MinimumCartItemQuantity;
        }
    }
}