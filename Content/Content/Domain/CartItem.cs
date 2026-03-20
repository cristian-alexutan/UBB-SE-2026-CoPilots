using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    class CartItem
    {
        private int id;
        private ShopItem shopItem;
        private int quantity;

        public CartItem(int id, ShopItem shopItem, int quantity)
        {
            this.id = id;
            this.shopItem = shopItem;
            this.quantity = quantity;
        }

        public int getId() { return id; }
        public ShopItem getShopItem() { return shopItem; }
        public int getQuantity() { return quantity; }
        public void setShopItem(ShopItem shopItem) { this.shopItem = shopItem; }
        public void setId(int id) { this.id = id; }
        public void setQuantity(int quantity) { this.quantity = quantity; }

        public float getTotalPrice()
        {
            return shopItem.getPrice() * quantity;
        }
    }
}
