using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface ICartRepo
    {
        IEnumerable<Cart> GetAll();
        Cart GetById(int id);
        void Add(Cart cart);
        void Delete(int id);

        void AddItemToCart(int cartId, CartItem item);
        void RemoveItemFromCart(int cartId, int cartItemId);
        void UpdateItemQuantity(int cartId, int cartItemId, int quantity);
        void ClearCart(int cartId);
        IEnumerable<(int CartItemId, int ItemId, int Quantity)> GetRawCartItems(int cartId);
    }
}
