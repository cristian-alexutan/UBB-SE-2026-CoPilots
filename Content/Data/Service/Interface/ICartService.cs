using Content.Domain;
using System.Collections.Generic;

namespace Content.Service
{
    public interface ICartService
    {
        IEnumerable<Cart> GetAllCarts();
        Cart GetCartById(int id);
        void AddCart(Cart cart);
        void DeleteCart(int id);
        void AddItemToCart(int cartId, CartItem item);
        void RemoveItemFromCart(int cartId, int cartItemId);
        void UpdateItemQuantity(int cartId, int cartItemId, int quantity);
        void ClearCart(int cartId);
    }
}