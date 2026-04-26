using System;
using System.Collections.Generic;
using System.Linq;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;

namespace Content.Service
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepo managerRepo;

        public ManagerService(IManagerRepo managerRepo)
        {
            this.managerRepo = managerRepo;
        }

        public IEnumerable<Manager> GetAllManagers()
        {
            return managerRepo.GetAll();
        }

        public Manager GetManagerById(int id)
        {
            return managerRepo.GetById(id);
        }

        public void AddManager(Manager manager)
        {
            if (manager == null)
            {
                throw new Exception("Manager must not be null");
            }

            if (string.IsNullOrWhiteSpace(manager.Name))
            {
                throw new Exception("Name field must not be empty");
            }

            if (string.IsNullOrWhiteSpace(manager.Email))
            {
                throw new Exception("Email field must not be empty");
            }

            if (!manager.Email.Contains("@"))
            {
                throw new Exception("Email field must be valid");
            }

            if (string.IsNullOrWhiteSpace(manager.Phone))
            {
                throw new Exception("Phone number field must not be empty");
            }

            managerRepo.Add(manager);
        }

        public Manager? DeleteManager(int id)
        {
            return managerRepo.Delete(id);
        }

        public Manager? UpdateManager(Manager manager)
        {
            if (manager == null)
            {
                throw new Exception("Manager must not be null");
            }

            if (string.IsNullOrWhiteSpace(manager.Name))
            {
                throw new Exception("Name field must not be empty");
            }

            if (string.IsNullOrWhiteSpace(manager.Email))
            {
                throw new Exception("Email field must not be empty");
            }

            if (!manager.Email.Contains("@"))
            {
                throw new Exception("Email field must be valid");
            }

            if (string.IsNullOrWhiteSpace(manager.Phone))
            {
                throw new Exception("Phone number field must not be empty");
            }

            return managerRepo.Update(manager);
        }

        public Manager? GetAnyManager()
        {
            Manager? manager = managerRepo.GetAll().FirstOrDefault();
            if (manager == null)
            {
                throw new Exception();
            }
            return manager;
        }
    }
}
