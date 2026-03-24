using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    public class ShopItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public Shop Shop { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }

        public ShopItem(int Id = 0, int Quantity, float Price, Shop Shop, string Name, string Desc, string Photo)
        {
            this.Id = Id;
            this.Quantity = Quantity;
            this.Price = Price;
            this.Shop = Shop;
            this.Name = Name;
            this.Description = Desc;
            this.Photo = Photo;
        }
        

    }
}
