using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ManagerRepo
    {
        private Dictionary<int,Domain.Manager> managers;

        public ManagerRepo()
        {
            managers = new Dictionary<int,Domain.Manager>();
        }

        public void addManager(Domain.Manager manager)
        {
            managers[manager.getId()]=manager;
        }

        public void deleteManager(int id)
        {
            managers.Remove(id);
        }

        public Dictionary<int,Domain.Manager> getAllManagers()
        {
            return managers;
        }

        public Domain.Manager getManagerById(int id)
        {
            return managers[id];
        }

    }
}
