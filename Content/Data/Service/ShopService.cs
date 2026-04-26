using System.Collections;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Service
{
    public class ShopService : IShopService
    {
        private readonly IShopRepo shopRepo;

        public ShopService(IShopRepo shopRepo)
        {
            this.shopRepo = shopRepo;
        }

        public IEnumerable<Shop> GetAllAvailableShops()
        {
            return this.shopRepo.GetAll();
        }

        public void AddShop(Shop shop)
        {
            if (string.IsNullOrWhiteSpace(shop.Name))
            {
                throw new Exception("Name field must not be empty");
            }

            if (string.IsNullOrWhiteSpace(shop.Type))
            {
                throw new Exception("Type field must not be empty");
            }

            var nameExists = this.shopRepo.GetAll().Any(otherShop => string.Equals(otherShop.Name, shop.Name, StringComparison.OrdinalIgnoreCase));
            if (nameExists)
            {
                throw new Exception("Shop name already exists");
            }

            this.shopRepo.Add(shop);
        }

        public void DeleteShop(int shopId)
        {
            this.shopRepo.Delete(shopId);
        }

        public IEnumerable<Shop> SortAlphabetically(IEnumerable<Shop> shops)
        {
            return shops.OrderBy(shop => shop.Name);
        }

        public void UpdateShop(Shop shop)
        {
            if (string.IsNullOrWhiteSpace(shop.Name))
            {
                throw new Exception("Name field must not be empty");
            }
            if (string.IsNullOrWhiteSpace(shop.Type))
            {
                throw new Exception("Type field must not be empty");
            }

            var isDuplicate = shopRepo.GetAll().Any(newShop =>
                newShop.Id != shop.Id && string.Equals(newShop.Name, shop.Name));

            if (isDuplicate)
            {
                throw new Exception("Shop with given name already exists");
            }

            this.shopRepo.Update(shop);
        }

        public IEnumerable<Shop> SearchByName(string input)
        {
            var filtered = this.GetAllAvailableShops()
                .Where(shop => shop.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return filtered;
        }
    }
}