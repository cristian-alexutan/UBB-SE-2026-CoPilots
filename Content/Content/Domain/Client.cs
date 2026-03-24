using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
