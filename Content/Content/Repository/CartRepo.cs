using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class CartRepo
    {
        private List<Domain.Cart> carts;

        public CartRepo()
        {
            carts = new List<Domain.Cart>();
        }

        public void addCart(Domain.Cart cart)
        {
            carts.Insert(cart.getId(), cart);
        }

        public void deleteCart(int id)
        {
            carts.RemoveAt(id);
        }

        public List<Domain.Cart> getAllCarts()
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
