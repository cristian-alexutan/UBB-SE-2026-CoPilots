using System.Collections.Generic;
using Content.Domain;

namespace Content.Service
{
    public interface IShopItemService
    {
        IEnumerable<ShopItem> GetAll();

        ShopItem GetById(int shopItemId);

        IEnumerable<ShopItem> GetShopItemsByShop(int shopId);

        IEnumerable<ShopItem> SearchItemsByName(int shopId, string searchText);

        void RemoveShopItem(int shopItemId);

        void AddShopItem(ShopItem shopItem);

        void UpdateShopItem(ShopItem shopItem);

        IEnumerable<ShopItem> SortByPrice(Shop currentShop);

        IEnumerable<ShopItem> SortAlphabetically(Shop currentShop);
    }
}
