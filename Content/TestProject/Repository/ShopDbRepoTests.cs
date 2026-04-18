using Content.Domain;
using Content.Repository.Database;

namespace TestProject.Repository
{
    [TestClass]
    public class ShopDbRepoTests
    {
        private const string connectionString = "Server=.\\SQLEXPRESS;Database=DutyFreeShops;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        [TestMethod]
        public void AddTest()
        {
            var repo = new ShopDbRepo(connectionString, null!);
            var shop = new Shop("Test Shop", "Retail", 1);
            repo.Add(shop);
            var result = repo.GetById(shop.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Shop", result.Name);
            Assert.AreEqual("Retail", result.Type);
            Assert.AreEqual(1, result.ManagerId);
        }

        [TestMethod]
        public void DeleteTestSuccesfull()
        {
            var repo = new ShopDbRepo(connectionString, null!);
            var shop = new Shop(0, "Test Shop", "Retail", 1);
            repo.Add(shop);
            var result = repo.GetById(shop.Id);
            Assert.IsNotNull(result);
            repo.Delete(shop.Id);
            result = repo.GetById(shop.Id);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeleteTestUnsuccesfull()
        {
            var repo = new ShopDbRepo(connectionString, null!);
            var result = repo.Delete(-1);
            Assert.IsNull(result);
        }
    }
}
