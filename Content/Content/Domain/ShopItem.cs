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
        public string Photo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ShopItem(int Id, int Quantity, float Price, Shop Shop, string Photo, string Name, string Description)
        {
            this.Id = Id;
            this.Quantity = Quantity;
            this.Price = Price;
            this.Shop = Shop;
            this.Photo = Photo;
            this.Name = Name;
            this.Description = Description;
        }


    }
}
