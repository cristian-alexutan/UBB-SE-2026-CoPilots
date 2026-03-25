using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Service
{
    public class ShopItemService
    {
        private readonly IShopRepo _shopRepo;
        private readonly IShopItemRepo _shopItemRepo;

        public ShopItemService(IShopRepo shopRepo, IShopItemRepo shopItemRepo)
        {
            this._shopRepo = shopRepo;
            this._shopItemRepo = shopItemRepo;
        }

        public IEnumerable<ShopItem> GetAll()
        {
            return _shopItemRepo.GetAll();
        }

        public IEnumerable<ShopItem> GetShopItemsByShop(int shopID)
        {
            List<ShopItem> result = new List<ShopItem>();

            foreach (var item in _shopItemRepo.GetAll())
            {
                if (item.Shop.Id == shopID)
                {
                    result.Add(item);
                }
            }

            return result;

        }

        public void RemoveShopItem(int shopItemID)
        {
            _shopItemRepo.Delete(shopItemID);
        }

        public void AddShopItem(int quantity, float price, string name, string desc, Shop shop, string photo)
        {
            if (quantity > 0 && price > 0 && shop != null && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(desc))
            {
                ShopItem newShopItem = new ShopItem(quantity, price, shop, photo, name, desc);
                _shopItemRepo.Add(newShopItem);
            }
            else throw new Exception("One of your fields is wrong loser");
        }

        public void UpdateShopItem(int id, int quantity, float price, string name, string desc, Shop shop, string photo)
        {
            if (quantity > 0 && price > 0 && shop != null && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(desc))
            {
                ShopItem newShopItem = new ShopItem(id, quantity, price, name, desc, shop, photo);
                _shopItemRepo.Update(newShopItem);
            }
            else throw new Exception("One of your fields is wrong loser");
        }
    }

}