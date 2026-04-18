using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    public class ShopItemMemoryRepo : IShopItemRepo
    {
        private readonly Dictionary<int, ShopItem> shopItems;

        public ShopItemMemoryRepo()
        {
            this.shopItems = new Dictionary<int, ShopItem>();
        }

        public void Add(ShopItem shopItem)
        {
            this.shopItems[shopItem.Id] = shopItem;
        }

        public void Delete(int id)
        {
            this.shopItems.Remove(id);
        }

        public IEnumerable<ShopItem> GetAll()
        {
            return this.shopItems.Values;
        }

        public ShopItem? GetById(int id)
        {
            this.shopItems.TryGetValue(id, out ShopItem? shopItem);
            return shopItem;
        }

        public void Update(ShopItem shopItem)
        {
            if (this.shopItems.ContainsKey(shopItem.Id))
            {
                this.shopItems[shopItem.Id] = shopItem;
            }
        }
    }
}
