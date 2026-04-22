namespace Content.Service
{
    using Content.Data.Service.Interface;
    using Content.Domain;
    using Content.Repository.Interface;
    using System.Collections;

    public class ShopService: IShopService
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

            var nameExists = this.shopRepo.GetAll().Any(s => string.Equals(s.Name, shop.Name));
            if (nameExists)
            {
                throw new Exception("Shop name already exists");
            }

            this.shopRepo.Add(shop);
        }

        public void DeleteShop(int id)
        {
            this.shopRepo.Delete(id);
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

            var isDuplicate = shopRepo.GetAll().Any(s =>
                s.Id != shop.Id && string.Equals(s.Name, shop.Name));

            if (isDuplicate)
            {
                throw new Exception("Shop with given name already exists");
            }

            this.shopRepo.Update(shop);
        }

        public IEnumerable<Shop> SearchByName(string input)
        {
            var filtered = this.GetAllAvailableShops()
                .Where(i => i.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return filtered;
        }
    }
}