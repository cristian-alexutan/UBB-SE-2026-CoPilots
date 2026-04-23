using Content.Domain;
using Content.Repository.Database;

namespace TestProject.Repository
{
    public class ClientDbRepoTests
    {
        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        [Test]
        public void AddTestSuccesfull()
        {
            ClientDbRepo repo = new ClientDbRepo(ConnectionString);
            Client client = new Client(0, "Test Client");
            repo.Add(client);
            Client? result = repo.GetById(client.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Test Client"));
            repo.Delete(client.Id);
        }

        [Test]
        public void DeleteTestSuccesfull()
        {
            ClientDbRepo repo = new ClientDbRepo(ConnectionString);
            Client client = new Client(0, "Test Client");
            repo.Add(client);
            Client? result = repo.GetById(client.Id);
            Assert.That(result, Is.Not.Null);
            repo.Delete(client.Id);
            result = repo.GetById(client.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteTestUnsuccesfull_IdDoesntExist()
        {
            ClientDbRepo repo = new ClientDbRepo(ConnectionString);
            Client? result = repo.Delete(-2);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void UpdateTestSuccesfull()
        {
            ClientDbRepo repo = new ClientDbRepo(ConnectionString);
            Client client = new Client(0, "Test Client");
            repo.Add(client);
            repo.Update(new Client(client.Id, "Updated Client"));
            Client? result = repo.GetById(client.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Updated Client"));
            repo.Delete(client.Id);
        }

        [Test]
        public void UpdateTestUnsuccesfull_ClientIsNull()
        {
            ClientDbRepo repo = new ClientDbRepo(ConnectionString);
            Client? result = repo.Update(new Client(-1, "Updated Client"));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAllTestSuccesfull()
        {
            ClientDbRepo repo = new ClientDbRepo(ConnectionString);
            Client client1 = new Client(0, "Test Client 1");
            Client client2 = new Client(0, "Test Client 2");
            repo.Add(client1);
            repo.Add(client2);

            IEnumerable<Client> result = repo.GetAll();

            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));

            repo.Delete(client1.Id);
            repo.Delete(client2.Id);
        }

        [Test]
        public void GetByIdTestUnsuccessful_IdDoesntExist()
        {
            ClientDbRepo repo = new ClientDbRepo(ConnectionString);

            Client? result = repo.GetById(-1);

            Assert.That(result, Is.Null);
        }
    }
}