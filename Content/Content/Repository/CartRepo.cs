using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class CartRepo
    {
        private Dictionary<int,Domain.Cart> carts;

        public CartRepo()
        {
            carts = new Dictionary<int,Domain.Cart>();
        }

        public void addCart(Domain.Cart cart)
        {
            carts[cart.getId()] = cart;
        }

        public void deleteCart(int id)
        {
            carts.Remove(id);
        }

        public Dictionary<int,Domain.Cart> getAllCarts()
        {
            return carts;
        }

        public Domain.Cart getCartById(int id)
        {
            return carts[id];
        }

        public void updateQuantity(int id, int cartItemId, int quantity)
        {
            carts[id].updateQuantity(cartItemId, quantity);
        }

        public void clearCart(int id)
        {
            carts[id].clearCart();
        }
    }
}
