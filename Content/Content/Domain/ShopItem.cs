namespace Content.Domain
{
    public class ShopItem
    {
        public ShopItem()
        {
        }

        public ShopItem(int id, int quantity, float price, Shop shop, string photo, string name, string description)
        {
            this.Id = id;
            this.Quantity = quantity;
            this.Price = price;
            this.Shop = shop;
            this.Photo = photo;
            this.Name = name;
            this.Description = description;
        }

        public int Id { get; set; }

        public int Quantity { get; set; }

        public float Price { get; set; }

        public Shop? Shop { get; set; }

        public string Photo { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
