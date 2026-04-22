using System;
using System.Collections.Generic;
using System.Linq;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;
using Windows.Media.Protection.PlayReady;

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

            if (!manager.Email.Contains("@"))
            {
                throw new ArgumentException("Email is invalid", nameof(manager.Email));
            }

            if (string.IsNullOrWhiteSpace(manager.Phone))
            {
                throw new ArgumentException("Phone is required", nameof(manager.Phone));
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

            if (!manager.Email.Contains("@"))
            {
                throw new ArgumentException("Email is invalid", nameof(manager.Email));
            }

            if (string.IsNullOrWhiteSpace(manager.Phone))
            {
                throw new ArgumentException("Phone is required", nameof(manager.Phone));
            }

            return managerRepo.Update(manager);
        }

        public Manager GetAnyManager()
        {
            return managerRepo.GetAll().FirstOrDefault();
        }
    }
}
