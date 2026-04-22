using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Content.Service
{
    public class ManagerService : IManagerService
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
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (string.IsNullOrWhiteSpace(manager.Name))
            {
                throw new ArgumentException("Name is required", nameof(manager.Name));
            }

            if (string.IsNullOrWhiteSpace(manager.Email))
            {
                throw new ArgumentException("Email is required", nameof(manager.Email));
            }

            if (string.IsNullOrWhiteSpace(manager.Phone))
            {
                throw new ArgumentException("Phone is required", nameof(manager.Phone));
            }

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
