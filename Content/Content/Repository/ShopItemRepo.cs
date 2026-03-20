using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ShopItemRepo
    {
        private List<Domain.ShopItem> shopItems;

        public ShopItemRepo()
        {
            shopItems = new List<Domain.ShopItem>();
        }

        public void addShopItem(Domain.ShopItem shopItem)
        {
            shopItems.Insert(shopItem.getId(), shopItem);
        }

        public void deleteShopItem(int id)
        {
            shopItems.RemoveAt(id);
        }

        public List<Domain.ShopItem> getAllShopItems()
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
