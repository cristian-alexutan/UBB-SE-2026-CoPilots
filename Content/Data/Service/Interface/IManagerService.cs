using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface IManagerService
    {
        void AddManager(Manager manager);
        Manager? DeleteManager(int id);
        IEnumerable<Manager> GetAllManagers();
        Manager GetAnyManager();
        Manager GetManagerById(int id);
        Manager? UpdateManager(Manager manager);
    }
}