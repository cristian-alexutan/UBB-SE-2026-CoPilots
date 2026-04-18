using Content.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface ICartRepo
    {
        IEnumerable<Cart> GetAll();
        Cart GetById(int Id);
        void Add(Cart Cart);
        void Delete(int Id);

        void AddItemToCart(int CartId, CartItem Item);
        void RemoveItemFromCart(int CartId, int CartItemId);
        void UpdateItemQuantity(int CartId, int CartItemId, int Quantity);
        void ClearCart(int CartId);
    }
}
