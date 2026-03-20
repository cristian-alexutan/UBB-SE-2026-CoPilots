using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    class ShopItem
    {
        private int id;
        private int quantity;
        private float price;
        private Shop shop;

        public ShopItem(int id, int quantity, float price, Shop shop)
        {
            this.id = id;
            this.quantity = quantity;
            this.price = price;
            this.shop = shop;
        }
        public int getId() { return id; }
        public int getQuantity() { return quantity; }
        public float getPrice() { return price; }
        public Shop getShop() { return shop; }
        public void setShop(Shop shop)
        {
            this.shop = shop;
        }
        public void setId(int id) { this.id = id; }
        public void setQuantity(int quantity) { this.quantity = quantity; }
        public void setPrice(float price) { this.price = price; }

    }
}
