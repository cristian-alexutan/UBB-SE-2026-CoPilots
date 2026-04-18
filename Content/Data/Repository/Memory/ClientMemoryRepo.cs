using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Content.Repository
{
    using Domain;
    using Interface;

    public class ClientMemoryRepo : IClientRepo
    {
        private Dictionary<int,Client> Clients;

        public ClientMemoryRepo()
        {
            Clients = new Dictionary<int, Domain.Client>();
        }

        public void Add(Client Client)
        {
            Clients[Client.Id] = Client;
        }


        public void Delete(int Id)
        {
            Clients.Remove(Id);
        }


        public IEnumerable<Client> GetAll()
        {
            return Clients.Values;
        }

        public Client GetById(int Id)
        {
            return Clients.ContainsKey(Id) ? Clients[Id] : null;
        }


    }
}
