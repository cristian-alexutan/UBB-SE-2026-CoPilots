using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface IManagerService
    {
        void AddManager(Manager manager);
        Manager? DeleteManager(int managerId);
        IEnumerable<Manager> GetAllManagers();
        Manager? GetAnyManager();
        Manager GetManagerById(int managerId);
        Manager? UpdateManager(Manager manager);
    }
}