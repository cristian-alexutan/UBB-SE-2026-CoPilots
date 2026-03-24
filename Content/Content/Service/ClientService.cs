using Content.Domain;
using Content.Repository.Interface;
using System.Collections.Generic;

namespace Content.Service
{
    public class ClientService
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
            _clientRepo.Add(client);
        }

        public void DeleteClient(int id)
        {
            _clientRepo.Delete(id);
        }


    }
}