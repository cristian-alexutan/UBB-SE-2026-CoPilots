using System;
using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;

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
            if (this.shopItems.ContainsKey(shopItem.Id))
            {
                throw new InvalidOperationException($"Shop item with id {shopItem.Id} already exists.");
            }

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
            if (!this.shopItems.ContainsKey(shopItem.Id))
            {
                throw new InvalidOperationException($"Shop item with id {shopItem.Id} does not exist.");
            }

            this.shopItems[shopItem.Id] = shopItem;
        }
    }
}