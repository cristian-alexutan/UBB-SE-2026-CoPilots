namespace Content.Domain
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Client(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
        
    }
}
