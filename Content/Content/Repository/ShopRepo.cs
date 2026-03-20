using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ShopRepo
    {
        private List<Domain.Shop> shops;

        public ShopRepo()
        {
            shops = new List<Domain.Shop>();
        }

        public void addShop(Domain.Shop shop)
        {
            shops.Insert(shop.getId(), shop);
        }

        public void deleteShop(int id)
        {
            shops.RemoveAt(id);
        }

        public List<Domain.Shop> getAllShops()
        {
            return shops;
        }

        public Domain.Shop getShopById(int id)
        {
            return shops[id];
        }

        public void updateShop(int id, Domain.Shop shop)
        {
            shops[id] = shop;
        }
    }
}
