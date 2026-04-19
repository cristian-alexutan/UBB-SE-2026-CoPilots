namespace Content.Domain
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public string Subcategory { get; set; }

        public Ticket(int id, string category, string subcategory)
        {
            this.Id = id;
            this.Category = category;
            this.Subcategory = subcategory;
        }
    }
}
