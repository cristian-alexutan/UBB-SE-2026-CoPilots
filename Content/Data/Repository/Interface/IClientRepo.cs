using System.Collections.Generic;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface IClientRepo
    {
        IEnumerable<Client> GetAll();

        Client GetById(int clientId);

        void Add(Client client);

        Client? Delete(int clientId);

        Client? Update(Client client);
    }
}
