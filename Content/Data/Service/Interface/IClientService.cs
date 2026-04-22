using Content.Domain;

namespace Content.Data.Service.Interface
{
    public interface IClientService
    {
        void AddClient(Client client);
        Client? DeleteClient(int id);
        IEnumerable<Client> GetAllClients();
        Client GetAnyClient();
        Client GetClientById(int id);
        Client? UpdateClient(Client client);
    }
}