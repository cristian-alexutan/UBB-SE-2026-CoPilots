using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Domain;

namespace Content.Repository
{
    public class ManagerMemoryRepo : IManagerRepo
    {
        private Dictionary<int,Domain.Manager> Managers;

        public ManagerMemoryRepo()
        {
            Managers = new Dictionary<int,Domain.Manager>();
        }

        public void Add(Domain.Manager Manager)
        {
            Managers[Manager.Id]=Manager;
        }

        public void Delete(int Id)
        {
            Managers.Remove(Id);
        }

        public IEnumerable<Manager> GetAll()
        {
            return Managers.Values;
        }

        public Domain.Manager GetById(int Id)
        {
            Managers.TryGetValue(Id, out Manager Manager);
            return Manager;
        }

    

    }
}
