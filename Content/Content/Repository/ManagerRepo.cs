using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Repository
{
    class ManagerRepo
    {
        private List<Domain.Manager> managers;

        public ManagerRepo()
        {
            managers = new List<Domain.Manager>();
        }

        public void addManager(Domain.Manager manager)
        {
            managers.Insert(manager.getId(),manager);
        }

        public void deleteManager(int id)
        {
            managers.RemoveAt(id);
        }

        public List<Domain.Manager> getAllManagers()
        {
            return managers;
        }

        public Domain.Manager getManagerById(int id)
        {
            return managers[id];
        }

    }
}
