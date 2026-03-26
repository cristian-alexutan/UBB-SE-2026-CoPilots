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

        public int ManagerId { get; set; }

        public Shop() { }
        public Shop(int Id, string Name, string Type, int ManagerId)
        {
            this.Id = Id;
            this.Name = Name;
            this.Type = Type;
            this.ManagerId = ManagerId;
        }


        public override string ToString()
        {
            return $"Shop [id={Id}, name={Name}, type={Type}, manager={ManagerId}]";
        }

        

    }
}
