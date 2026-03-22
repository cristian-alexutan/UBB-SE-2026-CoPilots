using Content.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository.Interface
{
    public interface IShopItemRepo
    {
        IEnumerable<ShopItem> GetAll();
        ShopItem GetById(int Id);
        void Add(ShopItem ShopItem);
        void Delete(int Id);
        void Update(ShopItem ShopItem);
    }
}
