using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface IManagerService
    {
        void AddManager(Manager manager);
        void DeleteManager(int id);
        IEnumerable<Manager> GetAllManagers();
        Manager GetAnyManager();
        Manager GetManagerById(int id);
        void UpdateManager(Manager manager);
    }
}