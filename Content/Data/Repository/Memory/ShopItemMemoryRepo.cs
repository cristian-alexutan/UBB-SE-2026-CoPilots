using Content.Domain;
using Content.Repository.Interface;

namespace Content.Repository
{
    public class ShopItemMemoryRepo : IShopItemRepo
    {
        private readonly Dictionary<int, ShopItem> shopItems;
        private int nextId;

        public ShopItemMemoryRepo()
        {
            this.shopItems = new Dictionary<int, ShopItem>();
            this.nextId = 1;
        }

        public void Add(ShopItem shopItem)
        {
            shopItem.Id = this.nextId++;
            this.shopItems[shopItem.Id] = shopItem;
        }

        public void Delete(int id)
        {
            this.shopItems.Remove(id);
        }

        public IEnumerable<ShopItem> GetAll()
        {
            return this.shopItems.Values;
        }

        public ShopItem? GetById(int id)
        {
            this.shopItems.TryGetValue(id, out ShopItem? shopItem);
            return shopItem;
        }

        public void Update(ShopItem shopItem)
        {
            if (this.shopItems.ContainsKey(shopItem.Id))
            {
                this.shopItems[shopItem.Id] = shopItem;
            }
        }
    }
}
