using System.Collections.Generic;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface IShopItemRepo
    {
        IEnumerable<ShopItem> GetAll();

        ShopItem? GetById(int shopItemId);

        void Add(ShopItem shopItem);

        void Delete(int shopItemId);

        void Update(ShopItem shopItem);
    }
}
