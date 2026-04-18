using System;
using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Service
{
    public class ShopItemService : IShopItemService
    {
        private readonly IShopItemRepo shopItemRepo;

        public ShopItemService(IShopItemRepo shopItemRepo)
        {
            this.shopItemRepo = shopItemRepo;
        }

        public IEnumerable<ShopItem> GetAll()
        {
            return this.shopItemRepo.GetAll();
        }

        public ShopItem GetById(int shopItemId)
        {
            ShopItem? item = this.shopItemRepo.GetById(shopItemId);
            if (item == null)
            {
                throw new InvalidOperationException($"Shop item with id {shopItemId} does not exist.");
            }

            return item;
        }

        public IEnumerable<ShopItem> GetShopItemsByShop(int shopId)
        {
            return this.shopItemRepo.GetAll()
                .Where(item => item.Shop != null && item.Shop.Id == shopId);
        }

        public IEnumerable<ShopItem> SearchItemsByName(int shopId, string searchText)
        {
            if (searchText == null)
            {
                searchText = string.Empty;
            }

            return this.GetShopItemsByShop(shopId)
                .Where(item => item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        public void RemoveShopItem(int shopItemId)
        {
            this.shopItemRepo.Delete(shopItemId);
        }

        public void AddShopItem(ShopItem item)
        {
            ValidateShopItem(item);
            this.shopItemRepo.Add(item);
        }

        public void UpdateShopItem(ShopItem item)
        {
            ValidateShopItem(item);
            this.shopItemRepo.Update(item);
        }

        public IEnumerable<ShopItem> SortByPrice(Shop currentShop)
        {
            return this.GetShopItemsByShop(currentShop.Id)
                .OrderBy(item => item.Price);
        }

        public IEnumerable<ShopItem> SortAlphabetically(Shop currentShop)
        {
            return this.GetShopItemsByShop(currentShop.Id)
                .OrderBy(item => item.Name);
        }

        private static void ValidateShopItem(ShopItem item)
        {
            if (item.Shop == null)
            {
                throw new ArgumentException("Shop item must have a Shop assigned.", nameof(item));
            }

            if (item.Quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.", nameof(item));
            }

            if (item.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(item));
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new ArgumentException("Shop item name cannot be empty.", nameof(item));
            }
        }
    }
}