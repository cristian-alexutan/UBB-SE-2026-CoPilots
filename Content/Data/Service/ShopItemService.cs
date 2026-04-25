using System;
using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;
using Content.Data.Service.Interface;

namespace Content.Service
{
    public class ShopItemService : IShopItemService
    {
        private readonly IShopItemRepo shopItemRepository;

        public ShopItemService(IShopItemRepo shopItemRepository)
        {
            this.shopItemRepository = shopItemRepository;
        }

        public IEnumerable<ShopItem> GetAll()
        {
            return this.shopItemRepository.GetAll();
        }

        public ShopItem GetById(int shopItemId)
        {
            ShopItem? shopItem = this.shopItemRepository.GetById(shopItemId);
            if (shopItem == null)
            {
                throw new InvalidOperationException($"Shop item with id {shopItemId} does not exist.");
            }

            return shopItem;
        }

        public IEnumerable<ShopItem> GetItemsByShopId(int shopId)
        {
            return this.shopItemRepository.GetAll()
                .Where(shopItem => shopItem.ShopId == shopId);
        }

        public IEnumerable<ShopItem> SearchItemsByName(int shopId, string searchText)
        {
            searchText ??= string.Empty;

            return this.GetItemsByShopId(shopId)
                .Where(shopItem => shopItem.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        public void RemoveShopItem(int shopItemId)
        {
            this.shopItemRepository.Delete(shopItemId);
        }

        public void AddShopItem(ShopItem shopItem)
        {
            ValidateShopItem(shopItem);
            this.shopItemRepository.Add(shopItem);
        }

        public void UpdateShopItem(ShopItem shopItem)
        {
            ValidateShopItem(shopItem);
            this.shopItemRepository.Update(shopItem);
        }

        public IEnumerable<ShopItem> GetItemsSortedByPrice(Shop currentShop)
        {
            if (currentShop == null)
            {
                throw new ArgumentNullException(nameof(currentShop));
            }

            return this.GetItemsByShopId(currentShop.Id)
                .OrderBy(shopItem => shopItem.Price);
        }

        public IEnumerable<ShopItem> GetItemsSortedAlphabetically(Shop currentShop)
        {
            if (currentShop == null)
            {
                throw new ArgumentNullException(nameof(currentShop));
            }

            return this.GetItemsByShopId(currentShop.Id)
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