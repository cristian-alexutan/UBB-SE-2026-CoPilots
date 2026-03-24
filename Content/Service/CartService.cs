using Content.Domain;
using Content.Repository.Interface;
using System.Collections.Generic;

namespace Content.Service
{
    public class CartService
    {
        private readonly ICartRepo _cartRepo;

        public CartService(ICartRepo cartRepo)
        {
            _cartRepo = cartRepo;
        }

        public IEnumerable<Cart> GetAllCarts()
        {
            return _cartRepo.GetAll();
        }

        public Cart GetCartById(int id)
        {
            return _cartRepo.GetById(id);
        }

        public void AddCart(Cart cart)
        {
            _cartRepo.Add(cart);
        }

        public void DeleteCart(int id)
        {
            _cartRepo.Delete(id);
        }

        public void AddItemToCart(int cartId, CartItem item)
        {
            _cartRepo.AddItemToCart(cartId, item);
        }

        public void RemoveItemFromCart(int cartId, int cartItemId)
        {
            _cartRepo.RemoveItemFromCart(cartId, cartItemId);
        }

        public void UpdateItemQuantity(int cartId, int cartItemId, int quantity)
        {
            _cartRepo.UpdateItemQuantity(cartId, cartItemId, quantity);
        }

        public void ClearCart(int cartId)
        {
            _cartRepo.ClearCart(cartId);
        }
    }
}