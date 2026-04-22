using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Content.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientRepo _clientRepo;

        public ClientService(IClientRepo clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public IEnumerable<Client> GetAllClients()
        {
            return _clientRepo.GetAll();
        }

        public Client GetClientById(int id)
        {
            return _clientRepo.GetById(id);
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

            _clientRepo.Add(client);
        }

        public void DeleteClient(int id)
        {
            _clientRepo.Delete(id);
        }

        public void UpdateClient(Client client)
        {
            _clientRepo.Update(client);
        }

        public Client GetAnyClient()
        {
            return _clientRepo.GetAll().FirstOrDefault();
        }

    }
}