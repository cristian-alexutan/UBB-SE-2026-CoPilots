using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    class Cart
    {
        private int id;
        private Client client;
        private List<CartItem> cartItems;

        public Cart(int id, Client client, List<CartItem> cartItems)
        {
            this.id = id;
            this.client = client;
            this.cartItems = cartItems;
        }
        public int getId() { return id; }
        public Client getClient() { return client; }
        public List<CartItem> getCartItems() { return cartItems; }
        public void setClient(Client client) { this.client = client; }
        public void setCartItems(List<CartItem> cartItems) { this.cartItems = cartItems; }
        public void setId(int id) { this.id = id; }
        public float getOverallPrice()
        {
            float totalPrice = 0;
            foreach (CartItem cartItem in cartItems)
            {
                totalPrice += cartItem.getTotalPrice();
            }
            return totalPrice;
        }

        public void clearCart()
        {
            cartItems.Clear();
        }

        public void updateQuantity(int cartItemId, int quantity)
        {
            cartItems[cartItemId].setQuantity(quantity);
        }

    }
}
