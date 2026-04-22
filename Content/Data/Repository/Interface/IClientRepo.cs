using System.Collections.Generic;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface IClientRepo
    {
        IEnumerable<Client> GetAll();

        Client GetById(int id);

        void Add(Client client);

        Client? Delete(int id);

        Client? Update(Client client);
    }
}
