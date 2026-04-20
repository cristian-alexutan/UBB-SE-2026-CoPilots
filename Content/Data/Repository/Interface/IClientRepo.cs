using Content.Domain;
using System.Collections.Generic;

namespace Content.Repository.Interface
{
    public interface IClientRepo
    {
        IEnumerable<Client> GetAll();

        Client GetById(int id);

        void Add(Client client);

        void Delete(int id);

        void Update(Client client);

    }
}
