using System.Collections.Generic;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface IShopItemRepo
    {
        IEnumerable<ShopItem> GetAll();

        ShopItem? GetById(int id);

        void Add(ShopItem shopItem);

        void Delete(int id);

        void Update(ShopItem shopItem);
    }
}
