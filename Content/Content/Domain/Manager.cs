using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Domain
{
    class Manager
    {
        private int id;
        private string name;
        private string email;
        private string phone;

        public Manager(int id, string name, string email, string phone)
        {
            this.id = id;
            this.name = name;
            this.email = email;
            this.phone = phone;
        }

        public int getId() { return id; }
        public string getName() { return name; }
        public string getEmail() { return email; }
        public string getPhone() { return phone; }

        public void setId(int id) { this.id = id; }
        public void setName(string name) { this.name = name; }
        public void setEmail(string email) { this.email = email; }
        public void setPhone(string phone) { this.phone = phone; }

    }
}
