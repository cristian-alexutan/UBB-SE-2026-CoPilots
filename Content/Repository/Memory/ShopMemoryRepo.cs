using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    public class ShopMemoryRepo : IShopRepo
    {
        private Dictionary<int,Domain.Shop> Shops;

        public ShopMemoryRepo()
        {
            Shops = new Dictionary<int,Domain.Shop>();
        }

        public void Add(Shop Shop)
        {
            Shops[Shop.Id]=Shop;
        }

        public void Delete(int Id)
        {
            Shops.Remove(Id);
        }

        public IEnumerable<Shop> GetAll()
        {
            return Shops.Values;
        }

        public Domain.Shop GetById(int Id)
        {
            Shops.TryGetValue(Id, out Shop Shop);
            return Shop;
        }

        public void Update(Shop Shop)
        {
            if (Shops.ContainsKey(Shop.Id))
            {
                Shops[Shop.Id] = Shop;
            }
        }
    }
}
