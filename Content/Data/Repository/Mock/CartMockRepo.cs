using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Repository
{
    public class CartMockRepo : ICartRepo
    {
        private Dictionary<int, Domain.Cart> carts;

        public CartMockRepo()
        {
            carts = new Dictionary<int, Domain.Cart>();
        }

        public void Add(Cart cart)
        {
            carts[cart.Id] = cart;
        }

        public void Delete(int id)
        {
            carts.Remove(id);
        }

        public void AddItemToCart(int cartId, CartItem item)
        {
            var cart = GetById(cartId);
            if (cart != null)
            {
                int newId = cart.CartItems.Count > 0 ? cart.CartItems.Keys.Max() + 1 : 1;
                item.Id = newId;
                cart.CartItems[newId] = item;
            }
        }

        public void RemoveItemFromCart(int cartId, int cartItemId)
        {
            var cart = GetById(cartId);
            if (cart != null)
            {
                cart.CartItems.Remove(cartItemId);
            }
        }

        public void UpdateItemQuantity(int cartId, int cartItemId, int quantity)
        {
            var cart = GetById(cartId);
            if (cart != null && cart.CartItems.ContainsKey(cartItemId))
            {
                cart.CartItems[cartItemId].Quantity = quantity;
            }
        }

        public IEnumerable<Cart> GetAll()
        {
            return carts.Values;
        }

        public Domain.Cart GetById(int id)
        {
            return carts.ContainsKey(id) ? carts[id] : null;
        }

        public void ClearCart(int id)
        {
            var cart = GetById(id);
            if (cart != null)
            {
                cart.CartItems.Clear();
            }
        }
    }
}
