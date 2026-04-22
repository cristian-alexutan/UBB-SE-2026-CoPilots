using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface IClientService
    {
        void AddClient(Client client);
        void DeleteClient(int id);
        IEnumerable<Client> GetAllClients();
        Client GetAnyClient();
        Client GetClientById(int id);
        void UpdateClient(Client client);
    }
}