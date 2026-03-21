using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public Manager Manager { get; set; }

        public Shop(int Id, string Name, string Type, Manager manager)
        {
            this.Id = Id;
            this.Name = Name;
            this.Type = Type;
            Manager = manager;
        }


        public override string ToString()
        {
            return $"Shop [id={Id}, name={Name}, type={Type}, manager={Manager}]";
        }

        

    }
}
