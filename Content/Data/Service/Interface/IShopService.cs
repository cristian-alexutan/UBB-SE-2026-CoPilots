using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;

namespace Content.Service
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
