using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;

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
