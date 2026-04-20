using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Repository
{
    public class ClientMemoryRepo : IClientRepo
    {
        private Dictionary<int,Client> clients;

        public ClientMemoryRepo()
        {
            clients = new Dictionary<int, Client>();
        }

        public void Add(Client client)
        {
            clients[client.Id] = client;
        }


        public void Delete(int id)
        {
            clients.Remove(id);
        }

        public void Update(Client client)
        {
            clients[client.Id] = client;
        }

        public IEnumerable<Client> GetAll()
        {
            return clients.Values;
        }

        public Client GetById(int id)
        {
            if (clients.ContainsKey(id))
                return clients[id];

            return null;

        }

    }
}
