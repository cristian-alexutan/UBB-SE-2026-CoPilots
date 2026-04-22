using System.Collections.Generic;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface IManagerRepo
    {
        IEnumerable<Manager> GetAll();

        Manager GetById(int id);

        void Add(Manager manager);

        Manager? Delete(int id);

        Manager? Update(Manager manager);
    }
}
