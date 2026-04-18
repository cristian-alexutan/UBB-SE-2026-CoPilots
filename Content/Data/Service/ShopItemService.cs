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
            ShopItem? shopItem = this.shopItemRepo.GetById(shopItemId);
            if (shopItem == null)
            {
                throw new InvalidOperationException($"Shop item with id {shopItemId} does not exist.");
            }

            return shopItem;
        }

        public IEnumerable<ShopItem> GetShopItemsByShop(int shopId)
        {
            return this.shopItemRepo.GetAll()
                .Where(shopItem => shopItem.ShopId == shopId);
        }

        public IEnumerable<ShopItem> SearchItemsByName(int shopId, string searchText)
        {
            if (searchText == null)
            {
                searchText = string.Empty;
            }

            return this.GetShopItemsByShop(shopId)
                .Where(shopItem => shopItem.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        public void RemoveShopItem(int shopItemId)
        {
            this.shopItemRepo.Delete(shopItemId);
        }

        public void AddShopItem(ShopItem shopItem)
        {
            ValidateShopItem(shopItem);
            this.shopItemRepo.Add(shopItem);
        }

        public void UpdateShopItem(ShopItem shopItem)
        {
            ValidateShopItem(shopItem);
            this.shopItemRepo.Update(shopItem);
        }

        public IEnumerable<ShopItem> SortByPrice(Shop currentShop)
        {
            if (currentShop == null)
            {
                throw new ArgumentNullException(nameof(currentShop));
            }

            return this.GetShopItemsByShop(currentShop.Id)
                .OrderBy(shopItem => shopItem.Price);
        }

        public IEnumerable<ShopItem> SortAlphabetically(Shop currentShop)
        {
            if (currentShop == null)
            {
                throw new ArgumentNullException(nameof(currentShop));
            }

            return this.GetShopItemsByShop(currentShop.Id)
                .OrderBy(shopItem => shopItem.Name);
        }

        private static void ValidateShopItem(ShopItem shopItem)
        {
            if (shopItem.ShopId <= 0)
            {
                throw new ArgumentException("Shop item must have a valid shop id.", nameof(shopItem));
            }

            if (shopItem.Quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.", nameof(shopItem));
            }

            if (shopItem.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(shopItem));
            }

            if (string.IsNullOrWhiteSpace(shopItem.Name))
            {
                throw new ArgumentException("Shop item name cannot be empty.", nameof(shopItem));
            }
        }
    }
}