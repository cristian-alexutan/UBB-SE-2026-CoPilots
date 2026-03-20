using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ShopItemRepo
    {
        private Dictionary<int,Domain.ShopItem> shopItems;

        public ShopItemRepo()
        {
            shopItems = new Dictionary<int,Domain.ShopItem>();
        }

        public void addShopItem(Domain.ShopItem shopItem)
        {
            shopItems[shopItem.getId()]=shopItem;
        }

        public void deleteShopItem(int id)
        {
            shopItems.Remove(id);
        }

        public Dictionary<int,Domain.ShopItem> getAllShopItems()
        {
            return shopItems;
        }

        public Domain.ShopItem getShopItemById(int id)
        {
            return shopItems[id];
        }

        public void updateShopItem(int id, Domain.ShopItem shopItem)
        {
            shopItems[id] = shopItem;
        }
    }
}
