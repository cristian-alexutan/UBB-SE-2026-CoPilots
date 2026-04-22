using System;
using System.Collections.Generic;
using System.Linq;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientRepo clientRepo;

        public ClientService(IClientRepo clientRepo)
        {
            this.clientRepo = clientRepo;
        }

        public IEnumerable<Client> GetAllClients()
        {
            return clientRepo.GetAll();
        }

        public Client GetClientById(int id)
        {
            return clientRepo.GetById(id);
        }

        public void AddClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(client.Name))
            {
                throw new ArgumentException("Name is required", nameof(client.Name));
            }

            clientRepo.Add(client);
        }

        public Client? DeleteClient(int id)
        {
            return clientRepo.Delete(id);
        }

        public Client? UpdateClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrWhiteSpace(client.Name))
            {
                throw new ArgumentException("Name is required", nameof(client.Name));
            }
            return clientRepo.Update(client);
        }

        public Client GetAnyClient()
        {
            return clientRepo.GetAll().FirstOrDefault();
        }
    }
}