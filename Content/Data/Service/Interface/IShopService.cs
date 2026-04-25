using System;
using System.Collections.Generic;
using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface IShopService
    {
        IEnumerable<Shop> GetAllAvailableShops();

        void AddShop(Shop shop);

        void DeleteShop(int id);

        void UpdateShop(Shop shop);

        IEnumerable<Shop> SortAlphabetically(IEnumerable<Shop> shops);
        public IEnumerable<Shop> SearchByName(string input);
    }
}
