using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    public class Cart
    {
        public int Id { get; set; }
        public Client Client { get; set; }
        public Dictionary<int, CartItem> CartItems { get; set; }
        public Cart(int id, Client client, Dictionary<int, CartItem> cartItems)
        {
            this.Id = id;
            this.Client = client;
            this.CartItems = cartItems;
        }
        public float GetOverallPrice()
        {
            float overallPrice = 0;
            foreach (CartItem cartItem in CartItems.Values)
            {
                overallPrice += cartItem.GetTotalPrice();
            }
            return overallPrice;
        }

        public void ClearCart()
        {
            CartItems.Clear();
        }

        public void UpdateQuantity(int cartItemId, int quantity)
        {
            CartItems[cartItemId].Quantity = quantity;
        }

        public void AddCartItem(CartItem cartItem)
        {
            CartItems[cartItem.Id] = cartItem;
        }

        public void RemoveCartItem(int cartItemId)
        {
            CartItems.Remove(cartItemId);
        }
    }
}
