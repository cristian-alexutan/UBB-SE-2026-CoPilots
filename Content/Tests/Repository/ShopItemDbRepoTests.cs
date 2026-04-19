using System;
using System.Linq;
using Content.Domain;
using Content.Repository.Database;
using NUnit.Framework;

namespace Tests.Repository
{
    [TestFixture]
    public class ShopItemDbRepoTests
    {
        private ShopItemDbRepo shopItemRepo = null!;
        private const string ConnectionString = ".\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;TrustServerCertificate=True;";

        private const int ShopId = 1;

        [SetUp]
        public void Setup()
        {
            this.shopItemRepo = new ShopItemDbRepo(ConnectionString);
        }

        [TearDown]
        public void TearDown()
        {
            var shopItems = this.shopItemRepo.GetAll().ToList();

            foreach (ShopItem shopItem in shopItems)
            {
                if (shopItem.Name.StartsWith("Test", StringComparison.OrdinalIgnoreCase))
                {
                    this.shopItemRepo.Delete(shopItem.Id);
                }
            }
        }

        [Test]
        public void GetAll_DatabaseInitialized_ReturnsNonEmptyCollection()
        {
            var shopItems = this.shopItemRepo.GetAll();

            Assert.That(shopItems.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void GetById_InvalidShopItemId_ReturnsNull()
        {
            ShopItem? shopItem = this.shopItemRepo.GetById(999);

            Assert.That(shopItem, Is.Null);
        }

        [Test]
        public void GetById_AddedShopItem_ReturnsCorrectName()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, ShopId, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);

            ShopItem? result = this.shopItemRepo.GetById(shopItem.Id);

            Assert.That(result!.Name, Is.EqualTo("Test shop item"));
        }

        [Test]
        public void Add_ValidShopItem_CanGetItemByAssignedId()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, ShopId, "placeholder.png", "Test shop item", "Test shop item description");

            this.shopItemRepo.Add(shopItem);

            ShopItem? result = this.shopItemRepo.GetById(shopItem.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Add_ShopItemWithEmptyPhotoAndDescription_PhotoIsEmptyString()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, ShopId, "", "Test shop item", "");

            this.shopItemRepo.Add(shopItem);

            Assert.That(this.shopItemRepo.GetById(shopItem.Id)!.Photo, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Delete_ExistingShopItem_ItemDeletedFromDatabase()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, ShopId, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);

            this.shopItemRepo.Delete(shopItem.Id);

            Assert.That(this.shopItemRepo.GetById(shopItem.Id), Is.Null);
        }

        [Test]
        public void Delete_InvalidShopItemId_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => this.shopItemRepo.Delete(99999));
        }

        [Test]
        public void Update_ExistingShopItem_NameUpdatedInDatabase()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, ShopId, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);
            ShopItem updatedItem = new ShopItem(shopItem.Id, 10, 20.5f, ShopId, "updated.png", "Test updated shop item", "Test updated shop item description");

            this.shopItemRepo.Update(updatedItem);

            Assert.That(this.shopItemRepo.GetById(shopItem.Id)!.Name, Is.EqualTo("Test updated shop item"));
        }

        [Test]
        public void Update_ShopItemWithEmptyPhoto_PhotoIsEmptyString()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, ShopId, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.Add(shopItem);
            ShopItem updatedItem = new ShopItem(shopItem.Id, 10, 20.5f, ShopId, "", "Test updated shop item", "");

            this.shopItemRepo.Update(updatedItem);

            Assert.That(this.shopItemRepo.GetById(shopItem.Id)!.Photo, Is.EqualTo(string.Empty));
        }
    }
}
