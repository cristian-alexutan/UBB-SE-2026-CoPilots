using Content.Domain;
using Content.Repository.Database;

namespace TestProject.Repository
{
    public class ClientDbRepoTests
    {
        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        [Test]
        public void AddTest()
        {
            var repo = new ClientDbRepo(ConnectionString);
            var client = new Client(0, "Test Client");
            repo.Add(client);
            var inserted = repo.GetAll().FirstOrDefault(c => c.Name == "Test Client");
            Assert.That(inserted, Is.Not.Null);
            Assert.That(inserted.Name, Is.EqualTo("Test Client"));
            repo.Delete(inserted.Id);
        }

        [Test]
        public void DeleteTestSuccessful()
        {
            var repo = new ClientDbRepo(ConnectionString);
            repo.Add(new Client(0, "Test Client"));
            var inserted = repo.GetAll().FirstOrDefault(c => c.Name == "Test Client");
            Assert.That(inserted, Is.Not.Null);
            repo.Delete(inserted.Id);
            var result = repo.GetById(inserted.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteTestUnsuccessful()
        {
            var repo = new ClientDbRepo(ConnectionString);
            Assert.DoesNotThrow(() => repo.Delete(-2));
        }

        [Test]
        public void UpdateTestSuccessful()
        {
            var repo = new ClientDbRepo(ConnectionString);
            repo.Add(new Client(0, "Test Client"));
            var inserted = repo.GetAll().FirstOrDefault(c => c.Name == "Test Client");
            Assert.That(inserted, Is.Not.Null);
            repo.Update(new Client(inserted.Id, "Updated Client"));
            var result = repo.GetById(inserted.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Updated Client"));
            repo.Delete(inserted.Id);
        }

        [Test]
        public void UpdateTestUnsuccessful()
        {
            var repo = new ClientDbRepo(ConnectionString);
            Assert.DoesNotThrow(() => repo.Update(new Client(-1, "Updated Client")));
        }
    }
}