using Content.Domain;
using Content.Repository.Database;

namespace TestProject.Repository
{
    public class ShopDbRepoTests
    {
        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        [Test]
        public void AddTest()
        {
            ShopDbRepo repo = new ShopDbRepo(ConnectionString);
            Shop shop = new Shop("Test Shop", "Type", 1);
            repo.Add(shop);
            Shop? result = repo.GetById(shop.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo("Test Shop"));
                Assert.That(result.Type, Is.EqualTo("Type"));
                Assert.That(result.ManagerId, Is.EqualTo(1));
            });
            repo.Delete(shop.Id);
        }

        [Test]
        public void DeleteTestSuccesfull()
        {
            ShopDbRepo repo = new ShopDbRepo(ConnectionString);
            Shop shop = new Shop(0, "Test Shop", "Type", 1);
            repo.Add(shop);
            Shop? result = repo.GetById(shop.Id);
            Assert.That(result, Is.Not.Null);
            repo.Delete(shop.Id);
            result = repo.GetById(shop.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteTestUnsuccesfull()
        {
            ShopDbRepo repo = new ShopDbRepo(ConnectionString);
            Shop? result = repo.Delete(-2);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void UpdateTestSuccesfull()
        {
            ShopDbRepo repo = new ShopDbRepo(ConnectionString);
            Shop shop = new Shop(0, "Test Shop", "Type", 1);
            repo.Add(shop);
            repo.Update(new Shop(shop.Id, "Updated Shop", "Type", 1));
            Shop? result = repo.GetById(shop.Id);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo("Updated Shop"));
                Assert.That(result.Type, Is.EqualTo("Type"));
                Assert.That(result.ManagerId, Is.EqualTo(1));
            });
            repo.Delete(shop.Id);
        }

        [Test]
        public void UpdateTestUnsuccesfull()
        {
            ShopDbRepo repo = new ShopDbRepo(ConnectionString);
            Shop? result = repo.Update(new Shop(-1, "Updated Shop", "Type", 1));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAllTest()
        {
            ShopDbRepo repo = new ShopDbRepo(ConnectionString);
            int initialCount = repo.GetAll().Count();
            Shop shop1 = new Shop(0, "Test Shop 1", "Type", 1);
            Shop shop2 = new Shop(0, "Test Shop 2", "Type", 1);
            repo.Add(shop1);
            repo.Add(shop2);
            int newCount = repo.GetAll().Count();
            Assert.That(newCount, Is.EqualTo(initialCount + 2));
            repo.Delete(shop1.Id);
            repo.Delete(shop2.Id);
        }
    }
}
