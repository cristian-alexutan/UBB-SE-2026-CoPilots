using Content.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Repository.Interface;
namespace Content.Repository
{
    public class CartMemoryRepo : ICartRepo
    {
        private Dictionary<int,Domain.Cart> carts;

        public CartMemoryRepo()
        {
            carts = new Dictionary<int,Domain.Cart>();
        }

        public void Add(Cart Cart)
        {
            carts[Cart.Id] = Cart;
        }

        public void Delete(int Id)
        {
            carts.Remove(Id);
        }



        public void AddItemToCart(int CartId, CartItem Item)
        {
            var Cart = GetById(CartId);
            if (Cart != null)
            {
                int newId = Cart.CartItems.Count > 0 ? Cart.CartItems.Keys.Max() + 1 : 1;
                Item.Id = newId;
                Cart.CartItems[newId] = Item;
            }
        }

        public void RemoveItemFromCart(int CartId, int CartItemId)
        {
            var Cart = GetById(CartId);
            if (Cart != null)
            {
                Cart.CartItems.Remove(CartItemId);
            }
        }

        public void UpdateItemQuantity(int CartId, int CartItemId, int Quantity)
        {
            var Cart = GetById(CartId);
            if (Cart != null && Cart.CartItems.ContainsKey(CartItemId))
            {
                Cart.CartItems[CartItemId].Quantity = Quantity;
            }
        }

        public IEnumerable<Cart> GetAll()
        {
            return carts.Values;
        }

        public Domain.Cart GetById(int Id)
        {
            return carts.ContainsKey(Id) ? carts[Id] : null;
        }


        public void ClearCart(int Id)
        {
            var Cart = GetById(Id);
            if (Cart != null)
            {
                Cart.CartItems.Clear();
            }
        }
    }
}
