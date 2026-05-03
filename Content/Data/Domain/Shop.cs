namespace Content.Domain
{
    public class Shop
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public Manager Manager { get; }

        public Shop(int id, string name, string type, Manager manager)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.Manager = manager;
        }

        public Shop(string name, string type, Manager manager)
        {
            this.Name = name;
            this.Type = type;
            this.Manager = manager;
        }
    }
}
