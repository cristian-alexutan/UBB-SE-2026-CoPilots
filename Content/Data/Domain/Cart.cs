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
        public Dictionary<int,CartItem> CartItems { get; set; }


        public Cart() { }
        public Cart(int Id, Client Client, Dictionary<int,CartItem> CartItems)
        {
            this.Id = Id;
            this.Client = Client;
            this.CartItems = CartItems;
        }
        public float GetOverallPrice()
        {
            float OverallPrice = 0;
            foreach (CartItem CartItem in CartItems.Values)
            {
                OverallPrice += CartItem.GetTotalPrice();
            }
            return OverallPrice;
        }

        public void ClearCart()
        {
            CartItems.Clear();
        }

        public void UpdateQuantity(int CartItemId, int Quantity)
        {
            CartItems[CartItemId].Quantity=Quantity;
        }

        public void AddCartItem(CartItem CartItem)
        {
            CartItems[CartItem.Id] = CartItem;
        }

        public void RemoveCartItem(int CartItemId)
        {
            CartItems.Remove(CartItemId);
        }

    }
}
