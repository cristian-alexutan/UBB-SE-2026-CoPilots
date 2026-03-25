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
        private Dictionary<int,Domain.ShopItem> ShopItems;

        public ShopItemMemoryRepo()
        {
            ShopItems = new Dictionary<int,Domain.ShopItem>();
        }

        public void Add(ShopItem ShopItem)
        {
            ShopItems[ShopItem.Id]=ShopItem;
        }

        public void Delete(int Id)
        {
            ShopItems.Remove(Id);
        }

        public IEnumerable<ShopItem> GetAll()
        {
            return ShopItems.Values;
        }

        public ShopItem GetById(int Id)
        {
            ShopItems.TryGetValue(Id, out ShopItem ShopItem);
            return ShopItem;
        }

        public void Update(ShopItem ShopItem)
        {
            if (ShopItems.ContainsKey(ShopItem.Id))
            {
                ShopItems[ShopItem.Id] = ShopItem;
            }
        }
    }
}
