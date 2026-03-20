using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ClientRepo
    {
        private Dictionary<int,Domain.Client> clients;

        public ClientRepo()
        {
            clients = new Dictionary<int, Domain.Client>();
        }

        public void addClient(Domain.Client client)
        {
            clients[client.getId()] = client;
        }

        public void deleteClient(int id)
        {
            clients.Remove(id);
        }

        public Dictionary<int, Domain.Client> getAllClients()
        {
            return clients;
        }

        public Domain.Client getClientById(int id)
        {
            return clients[id];
        }


    }
}
