using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Service
{
    public class ManagerService
    {
        private readonly IManagerRepo _managerRepo;

        public ManagerService(IManagerRepo managerRepo)
        {
            _managerRepo = managerRepo;
        }

        public IEnumerable<Manager> GetAllManagers()
        {
            return _managerRepo.GetAll();
        }

        public Manager GetManagerById(int id)
        {
            return _managerRepo.GetById(id);
        }

        public void AddManager(Manager manager)
        {
            _managerRepo.Add(manager);
        }

        public void DeleteManager(int id)
        {
            _managerRepo.Delete(id);
        }

        public void UpdateManager(Manager manager)
        {
            _managerRepo.Update(manager);
        }

        public Manager GetAnyManager()
        {
            return _managerRepo.GetAll().FirstOrDefault();
        }
    }
}
