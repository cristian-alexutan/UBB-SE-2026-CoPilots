using System.Linq;
using Content.Domain;
using Content.Repository;
using NUnit.Framework;

namespace Tests.Repository
{
    [TestFixture]
    public class ShopItemMemoryRepoTests
    {
        private ShopItemMemoryRepo shopItemRepo;

        [SetUp]
        public void Setup()
        {
            this.shopItemRepo = new ShopItemMemoryRepo();
        }

        [Test]
        public void Add_NewShopItem_ItemAppearsInGetAll()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            this.shopItemRepo.Add(shopItem);

            Assert.That(this.shopItemRepo.GetAll().Count(), Is.EqualTo(1));
        }

        [Test]
        public void Add_NewShopItem_ItemsHasId1()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            this.shopItemRepo.Add(shopItem);

            Assert.That(shopItem.Id, Is.EqualTo(1));
        }

        [Test]
        public void Delete_ExistingShopItem_ItemNoLongerInRepo()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);

            this.shopItemRepo.Delete(shopItem.Id);

            Assert.That(this.shopItemRepo.GetById(shopItem.Id), Is.Null);
        }

        [Test]
        public void Delete_InvalidShopItemId_RepoUnaffected()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);

            this.shopItemRepo.Delete(2);

            Assert.That(this.shopItemRepo.GetAll().Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetAll_NoShopItems_HasNoItems()
        {
            int shopItemsCount = this.shopItemRepo.GetAll().Count();

            Assert.That(shopItemsCount, Is.EqualTo(0));
        }

        [Test]
        public void GetAll_TwoShopItems_ReturnsBothItems()
        {
            ShopItem shopItem1 = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item 1", "Test shop item description 1");
            ShopItem shopItem2 = new ShopItem(10, 20.5f, 1, "placeholder.png", "Test shop item 2", "Test shop item description 2");

            this.shopItemRepo.Add(shopItem1);
            this.shopItemRepo.Add(shopItem2);

            int shopItemsCount = this.shopItemRepo.GetAll().Count();

            Assert.That(shopItemsCount, Is.EqualTo(2));
        }

        [Test]
        public void GetById_InvalidShopItemId_ReturnsNull()
        {
            ShopItem? shopItem = this.shopItemRepo.GetById(1);

            Assert.That(shopItem, Is.Null);
        }

        [Test]
        public void GetById_ValidShopItem_ReturnsCorrectItem()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);

            ShopItem? result = this.shopItemRepo.GetById(shopItem.Id);

            Assert.That(result!.Name, Is.EqualTo("Test shop item"));
        }

        [Test]
        public void Update_ExistingShopItem_NameIsUpdated()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);

            ShopItem updatedItem = new ShopItem(shopItem.Id, 10, 20.5f, 2, "updated.png", "Updated shop item", "Updated shop item description");

            this.shopItemRepo.Update(updatedItem);

            Assert.That(this.shopItemRepo.GetById(shopItem.Id)!.Name, Is.EqualTo("Updated shop item"));
        }

        [Test]
        public void Update_ExistingShopItem_PriceIsUpdated()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);

            ShopItem updatedItem = new ShopItem(shopItem.Id, 10, 20.5f, 2, "updated.png", "Updated shop item", "Updated shop item description");

            this.shopItemRepo.Update(updatedItem);

            Assert.That(this.shopItemRepo.GetById(shopItem.Id)!.Price, Is.EqualTo(20.5f));
        }

        [Test]
        public void Update_InvalidShopItemId_RepoNotChanged()
        {
            ShopItem updatedItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            this.shopItemRepo.Update(updatedItem);

            Assert.That(this.shopItemRepo.GetAll(), Is.Empty);
        }
    }
}
