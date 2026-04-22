using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Repository
{
    public class ClientMockRepo : IClientRepo
    {
        private Dictionary<int, Client> clients;

        public ClientMockRepo()
        {
            clients = new Dictionary<int, Client>();
        }

        public void Add(Client client)
        {
            int newId;
            if (clients.Count > 0)
            {
                newId = clients.Keys.Max() + 1;
            }
            else
            {
                newId = 1;
            }
            client.Id = newId;
            clients[client.Id] = client;
        }

        public Client? Delete(int id)
        {
            if (!clients.ContainsKey(id))
            {
                return null;
            }
            Client existing = clients[id];
            clients.Remove(id);
            return existing;
        }

        public Client? Update(Client client)
        {
            if (!clients.ContainsKey(client.Id))
            {
                return null;
            }
            clients[client.Id] = client;
            return client;
        }

        public IEnumerable<Client> GetAll()
        {
            return clients.Values;
        }

        public Client GetById(int id)
        {
            if (clients.ContainsKey(id))
            {
                return clients[id];
            }

            return null;
        }
    }
}
