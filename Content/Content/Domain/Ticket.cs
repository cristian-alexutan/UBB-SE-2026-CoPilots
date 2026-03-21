using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }

        public Ticket(int Id, string Category, string Subcategory)
        {
            this.Id = Id;
            this.Category = Category;
            this.Subcategory = Subcategory;
        }
    }
}
