namespace Content.Domain
{
    public class Shop
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int ManagerId { get; }

        public Shop(int id, string name, string type, int managerId)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.ManagerId = managerId;
        }

        public Shop(string name, string type, int managerId)
        {
            this.Name = name;
            this.Type = type;
            this.ManagerId = managerId;
        }
    }
}
