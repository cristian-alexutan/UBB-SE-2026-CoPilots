using System.Collections.Generic;
using Content.Domain;

namespace Content.Repository.Interface
{
    public interface IManagerRepo
    {
        IEnumerable<Manager> GetAll();

        Manager GetById(int managerId);

        void Add(Manager manager);

        Manager? Delete(int managerId);

        Manager? Update(Manager manager);
    }
}
