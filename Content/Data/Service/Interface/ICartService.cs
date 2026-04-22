using System.Collections.Generic;
using Content.Domain;

namespace Content.Service
{
    public interface ICartService
    {
        IEnumerable<Cart> GetAllCarts();
        Cart GetCartById(int id);
        Cart GetOrCreateCart(int userId);
        void AddCart(Cart cart);
        void DeleteCart(int id);
        void AddItemToCart(int cartId, CartItem item);
        void RemoveItemFromCart(int cartId, int cartItemId);
        void UpdateItemQuantity(int cartId, int cartItemId, int quantity);
        void ClearCart(int cartId);
    }
}