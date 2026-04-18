using Content.Domain;
using Content.Repository.Database;

namespace TestProject.Repository
{
    public class ShopDbRepoTests
    {
        private const string connectionString = "Server=.\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        [Test]
        public void AddTest()
        {
            var repo = new ShopDbRepo(connectionString);
            var shop = new Shop("Test Shop", "Retail", 1);
            repo.Add(shop);
            var result = repo.GetById(shop.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo("Test Shop"));
                Assert.That(result.Type, Is.EqualTo("Retail"));
                Assert.That(result.ManagerId, Is.EqualTo(1));
            });
            repo.Delete(shop.Id);
        }

        [Test]
        public void DeleteTestSuccesfull()
        {
            var repo = new ShopDbRepo(connectionString);
            var shop = new Shop(0, "Test Shop", "Retail", 1);
            repo.Add(shop);
            var result = repo.GetById(shop.Id);
            Assert.That(result, Is.Not.Null);
            repo.Delete(shop.Id);
            result = repo.GetById(shop.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteTestUnsuccesfull()
        {
            var repo = new ShopDbRepo(connectionString);
            var result = repo.Delete(-2);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void UpdateTestSuccesfull()
        {
            var repo = new ShopDbRepo(connectionString);
            var shop = new Shop(0, "Test Shop", "Retail", 1);
            repo.Add(shop);
            repo.Update(new Shop(shop.Id, "Updated Shop", "Retail", 1));
            var result = repo.GetById(shop.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo("Updated Shop"));
                Assert.That(result.Type, Is.EqualTo("Retail"));
                Assert.That(result.ManagerId, Is.EqualTo(1));
            });
            repo.Delete(shop.Id);
        }

        [Test]
        public void UpdateTestUnsuccesfull()
        {
            var repo = new ShopDbRepo(connectionString);
            var result = repo.Update(new Shop(-1, "Updated Shop", "Retail", 1));
            Assert.That(result, Is.Null);
        }
    }
}
