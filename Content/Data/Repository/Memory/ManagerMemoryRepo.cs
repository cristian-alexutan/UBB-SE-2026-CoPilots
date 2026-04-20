using Content.Domain;
using Content.Repository.Interface;
using System.Collections.Generic;

namespace Content.Repository
{
    public class ManagerMemoryRepo : IManagerRepo
    {
        private Dictionary<int, Manager> managers;

        public ManagerMemoryRepo()
        {
            managers = new Dictionary<int,Manager>();
        }

        public void Add(Manager manager)
        {
            managers[manager.Id]=manager;
        }

        public void Delete(int id)
        {
            managers.Remove(id);
        }

        public void Update(Manager manager)
        {
            managers[manager.Id] = manager;
        }

        public IEnumerable<Manager> GetAll()
        {
            return managers.Values;
        }

        public Manager GetById(int id)
        {
            managers.TryGetValue(id, out Manager manager);
            return manager;
        }

    }
}
