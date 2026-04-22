using System.Linq;
using Content.Domain;
using Content.Repository.Database;

namespace TestProject.Repository
{
    public class ManagerDbRepoTests
    {
        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        [Test]
        public void AddTest()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            repo.Add(new Manager(0, "Test Manager", "test@mail.com", "0700000001"));
            var inserted = repo.GetAll().FirstOrDefault(m => m.Name == "Test Manager");
            Assert.That(inserted, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(inserted.Name, Is.EqualTo("Test Manager"));
                Assert.That(inserted.Email, Is.EqualTo("test@mail.com"));
                Assert.That(inserted.Phone, Is.EqualTo("0700000001"));
            });
            repo.Delete(inserted.Id);
        }

        [Test]
        public void DeleteTestSuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            repo.Add(new Manager(0, "Test Manager", "test@mail.com", "0700000001"));
            var inserted = repo.GetAll().FirstOrDefault(m => m.Name == "Test Manager");
            Assert.That(inserted, Is.Not.Null);
            repo.Delete(inserted.Id);
            var result = repo.GetById(inserted.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteTestUnsuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            Assert.DoesNotThrow(() => repo.Delete(-2));
        }

        [Test]
        public void UpdateTestSuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            repo.Add(new Manager(0, "Test Manager", "test@mail.com", "0700000001"));
            var inserted = repo.GetAll().FirstOrDefault(m => m.Name == "Test Manager");
            Assert.That(inserted, Is.Not.Null);
            repo.Update(new Manager(inserted.Id, "Updated Manager", "updated@mail.com", "0700000099"));
            var result = repo.GetById(inserted.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo("Updated Manager"));
                Assert.That(result.Email, Is.EqualTo("updated@mail.com"));
                Assert.That(result.Phone, Is.EqualTo("0700000099"));
            });
            repo.Delete(inserted.Id);
        }

        [Test]
        public void UpdateTestUnsuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            Assert.DoesNotThrow(() => repo.Update(new Manager(-1, "Updated Manager", "updated@mail.com", "0700000099")));
        }
    }
}