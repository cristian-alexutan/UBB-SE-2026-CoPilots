using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    class Shop
    {
        private int id;
        private string name;
        private string type;

        public Shop(int id, string name, string type)
        {
            this.id = id;
            this.name = name;
            this.type = type;
        }

        public int getId() { return id; }
        public string getName() { return name; }
        public string getType() { return type; }

        public override string ToString()
        {
            return $"Shop [id={id}, name={name}, type={type}]";
        }

        public void setId(int id) { this.id = id; }
        public void setName(string name) { this.name = name; }
        public void setType(string type) { this.type = type; }

    }
}
