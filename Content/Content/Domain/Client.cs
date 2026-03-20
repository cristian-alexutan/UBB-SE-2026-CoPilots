using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    class Client
    {
        private int id;
        private string name;

        public Client(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        public int getId()
        {
            return id;
        }
        public string getName()
        {
            return name;
        }
        public void setId(int id) { this.id = id; }
        public void setName(string name) { this.name = name; }
    }
}
