using Content.Domain;
using System.Collections.Generic;

namespace Content.Repository.Interface
{
    public interface IManagerRepo
    {
        IEnumerable<Manager> GetAll();

        Manager GetById(int id);

        void Add(Manager manager);

        void Delete(int id);

        void Update(Manager manager);
    }
}
