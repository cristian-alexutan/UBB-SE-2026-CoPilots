using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    public class CartItem
    {
        public int Id { get; set; }
        public ShopItem ShopItem { get; set; }
        public int Quantity { get; set; }

        public CartItem(int Id, ShopItem ShopItem, int Quantity)
        {
            this.Id = Id;
            this.ShopItem = ShopItem;
            this.Quantity = Quantity;
        }

        public float GetTotalPrice()
        {
            return ShopItem.Price * Quantity;
        }
    }
}
