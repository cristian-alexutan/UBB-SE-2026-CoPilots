using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;


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

        public void AddShopItem(ShopItem item)
        {
            if (item.Quantity > 0 && item.Price > 0 && item.Shop != null && !string.IsNullOrEmpty(item.Name))
            {
                _shopItemRepo.Add(item);
            }
            else throw new Exception("One of your fields is wrong loser");
        }

        public void UpdateShopItem(ShopItem item)
        {
            if (item.Quantity > 0 && item.Price > 0 && !string.IsNullOrEmpty(item.Name))
            {
                _shopItemRepo.Update(item);
            }
            else throw new Exception("One of your fields is wrong loser");
        }

        public IEnumerable<ShopItem> SortByPrice()
        {
            IEnumerable<ShopItem> sorted = _shopItemRepo.GetAll()
                .OrderBy(item => item.Price);

            return sorted;

        }

        public IEnumerable<ShopItem> SortAlphabetically()
        {
            IEnumerable<ShopItem> sorted = _shopItemRepo.GetAll()
                .OrderBy(item => item.Name);

            return sorted;
        }
    }

}