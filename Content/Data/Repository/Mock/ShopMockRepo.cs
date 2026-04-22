namespace Content.Repository
{
    using Content.Domain;
    using Content.Repository.Interface;

    public class ShopMockRepo : IShopRepo
    {
        private readonly Dictionary<int, Domain.Shop> shops;

        public ShopMockRepo()
        {
            this.shops = new Dictionary<int, Domain.Shop>();
        }

        public void Add(Shop shop)
        {
            this.shops[shop.Id] = shop;
        }

        public Shop? Delete(int id)
        {
            if (!this.shops.TryGetValue(id, out Shop? shop))
            {
                return null;
            }

            this.shops.Remove(id);
            return shop;
        }

        public IEnumerable<Shop> GetAll()
        {
            return this.shops.Values;
        }

        public Domain.Shop GetById(int id)
        {
            this.shops.TryGetValue(id, out Shop shop);
            return shop;
        }

        public Shop? Update(Shop shop)
        {
            if (this.shops.ContainsKey(shop.Id))
            {
                this.shops[shop.Id] = shop;
            }

            return null;
        }
    }
}