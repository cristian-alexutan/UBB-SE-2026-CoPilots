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
            Manager manager = new Manager(0, "Test Manager", "test@mail.com", "0700000001");
            repo.Add(manager);
            Manager? result = repo.GetById(manager.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo("Test Manager"));
                Assert.That(result.Email, Is.EqualTo("test@mail.com"));
                Assert.That(result.Phone, Is.EqualTo("0700000001"));
            });
            repo.Delete(manager.Id);
        }

        [Test]
        public void DeleteTestSuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            Manager manager = new Manager(0, "Test Manager", "test@mail.com", "0700000001");
            repo.Add(manager);
            Manager? result = repo.GetById(manager.Id);
            Assert.That(result, Is.Not.Null);
            repo.Delete(manager.Id);
            result = repo.GetById(manager.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteTestUnsuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            Manager? result = repo.Delete(-2);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void UpdateTestSuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            Manager manager = new Manager(0, "Test Manager", "test@mail.com", "0700000001");
            repo.Add(manager);
            repo.Update(new Manager(manager.Id, "Updated Manager", "updated@mail.com", "0700000099"));
            Manager? result = repo.GetById(manager.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo("Updated Manager"));
                Assert.That(result.Email, Is.EqualTo("updated@mail.com"));
                Assert.That(result.Phone, Is.EqualTo("0700000099"));
            });
            repo.Delete(manager.Id);
        }

        [Test]
        public void UpdateTestUnsuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            Manager? result = repo.Update(new Manager(-1, "Updated Manager", "updated@mail.com", "0700000099"));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAllTest()
        {
            var repo = new ManagerDbRepo(ConnectionString);
            Manager manager1 = new Manager(0, "Test Manager 1", "test1@mail.com", "0700000001");
            Manager manager2 = new Manager(0, "Test Manager 2", "test2@mail.com", "0700000002");
            repo.Add(manager1);
            repo.Add(manager2);

            IEnumerable<Manager> result = repo.GetAll();

            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));

            repo.Delete(manager1.Id);
            repo.Delete(manager2.Id);
        }

        [Test]
        public void GetByIdTestUnsuccessful()
        {
            var repo = new ManagerDbRepo(ConnectionString);

            Manager? result = repo.GetById(-1);

            Assert.That(result, Is.Null);
        }
    }
}