using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Repository
{
    public class ManagerMockRepo : IManagerRepo
    {
        private Dictionary<int, Manager> managers;

        public ManagerMockRepo()
        {
            managers = new Dictionary<int, Manager>();
        }

        public void Add(Manager manager)
        {
            int newId;
            if (managers.Count > 0)
            {
                newId = managers.Keys.Max() + 1;
            }
            else
            {
                newId = 1;
            }
            manager.Id = newId;
            managers[manager.Id] = manager;
        }

        public Manager? Delete(int id)
        {
            if (!managers.ContainsKey(id))
            {
                return null;
            }
            Manager existing = managers[id];
            managers.Remove(id);
            return existing;
        }

        public Manager? Update(Manager manager)
        {
            if (!managers.ContainsKey(manager.Id))
            {
                return null;
            }
            managers[manager.Id] = manager;
            return manager;
        }

        public IEnumerable<Manager> GetAll()
        {
            return managers.Values;
        }

        public Manager? GetById(int id)
        {
            managers.TryGetValue(id, out Manager? manager);
            return manager;
        }
    }
}
