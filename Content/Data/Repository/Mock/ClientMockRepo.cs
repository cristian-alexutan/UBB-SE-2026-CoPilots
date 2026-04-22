using System.Collections.Generic;
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
            {
                return clients[id];
            }

            return null;
        }
    }
}
