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
            var existing = cart?.CartItems.Values.FirstOrDefault(ci => ci.ShopItem?.Id == item.ShopItem.Id);
            var shopItem = this.shopItemService.GetById(item.ShopItem.Id);

            int totalQuantity = (existing?.Quantity ?? 0) + item.Quantity;
            if (shopItem == null || totalQuantity > shopItem.Quantity)
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
            var cartItem = cart?.CartItems.Values.FirstOrDefault(ci => ci.Id == cartItemId);
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
    }
}