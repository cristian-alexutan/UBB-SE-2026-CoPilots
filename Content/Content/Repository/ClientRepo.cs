using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ClientRepo
    {
        private List<Domain.Client> clients;

        public ClientRepo()
        {
            clients = new List<Domain.Client>();
        }

        public void addClient(Domain.Client client)
        {
            clients.Insert(client.getId(), client);
        }

        public void deleteClient(int id)
        {
            clients.RemoveAt(id);
        }

        public List<Domain.Client> getAllClients()
        {
            return clients;
        }

        public Domain.Client getClientById(int id)
        {
            return clients[id];
        }


    }
}
