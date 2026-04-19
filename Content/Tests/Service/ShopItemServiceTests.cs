using System;
using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Interface;
using Content.Service;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class ShopItemServiceTests
    {
        private IShopItemRepo shopItemRepo = null!;
        private ShopItemService shopItemService = null!;

        [SetUp]
        public void Setup()
        {
            this.shopItemRepo = Substitute.For<IShopItemRepo>();
            this.shopItemService = new ShopItemService(this.shopItemRepo);
        }

        [Test]
        public void GetAll_EmptyRepo_ReturnsEmptyCollection()
        {
            this.shopItemRepo.GetAll().Returns(new List<ShopItem>());

            IEnumerable<ShopItem> shopItems = this.shopItemService.GetAll();

            Assert.That(shopItems, Is.Empty);
        }

        [Test]
        public void GetAll_TwoShopItemsInRepo_ReturnsTwoItems()
        {
            ShopItem shopItem1 = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item 1", "Test shop item description 1");
            ShopItem shopItem2 = new ShopItem(10, 20.5f, 1, "placeholder.png", "Test shop item 2", "Test shop item description 2");
            this.shopItemRepo.GetAll().Returns(new List<ShopItem> { shopItem1, shopItem2 });

            IEnumerable<ShopItem> shopItems = this.shopItemService.GetAll();

            Assert.That(shopItems.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetById_ExistingShopItem_ReturnsCorrectItemName()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");
            this.shopItemRepo.GetById(shopItem.Id).Returns(shopItem);

            ShopItem result = this.shopItemService.GetById(shopItem.Id);

            Assert.That(result.Name, Is.EqualTo("Test shop item"));
        }

        [Test]
        public void GetById_InvalidShopItemId_ThrowsInvalidOperationException()
        {
            this.shopItemRepo.GetById(999).Returns((ShopItem?)null);

            var exception = Assert.Catch<InvalidOperationException>(() => this.shopItemService.GetById(999));

            Assert.That(exception!.Message, Does.Contain("999"));
        }

        [Test]
        public void GetItemsByShopId_ShopHasTwoItems_ReturnsTwoItems()
        {
            ShopItem shopItem1 = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item 1", "Test shop item description 1");
            ShopItem shopItem2 = new ShopItem(10, 20.5f, 1, "placeholder.png", "Test shop item 2", "Test shop item description 2");
            ShopItem shopItem3 = new ShopItem(15, 30.5f, 2, "placeholder.png", "Test shop item 3", "Test shop item description 3");
            this.shopItemRepo.GetAll().Returns(new List<ShopItem> { shopItem1, shopItem2, shopItem3 });

            IEnumerable<ShopItem> result = this.shopItemService.GetItemsByShopId(1);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void SearchItemsByName_PartialNameMatch_ReturnsMatchingItems()
        {
            ShopItem shopItem1 = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description 1");
            ShopItem shopItem2 = new ShopItem(10, 20.5f, 1, "placeholder.png", "Another test shop item", "Test shop item description 2");
            ShopItem shopItem3 = new ShopItem(15, 30.5f, 1, "placeholder.png", "Shop item", "Test shop item description 3");
            this.shopItemRepo.GetAll().Returns(new List<ShopItem> { shopItem1, shopItem2, shopItem3 });

            IEnumerable<ShopItem> result = this.shopItemService.SearchItemsByName(1, "test");

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void SearchItemsByName_NullSearchText_ReturnsAllItems()
        {
            ShopItem shopItem1 = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description 1");
            ShopItem shopItem2 = new ShopItem(10, 20.5f, 1, "placeholder.png", "Another test shop item", "Test shop item description 2");
            ShopItem shopItem3 = new ShopItem(15, 30.5f, 1, "placeholder.png", "Shop item", "Test shop item description 3");
            this.shopItemRepo.GetAll().Returns(new List<ShopItem> { shopItem1, shopItem2, shopItem3 });

            IEnumerable<ShopItem> result = this.shopItemService.SearchItemsByName(1, null!);

            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public void SearchItemsByName_NoMatchingItems_ReturnsEmptyCollection()
        {
            ShopItem shopItem1 = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description 1");
            ShopItem shopItem2 = new ShopItem(10, 20.5f, 1, "placeholder.png", "Another test shop item", "Test shop item description 2");
            ShopItem shopItem3 = new ShopItem(15, 30.5f, 1, "placeholder.png", "Shop item", "Test shop item description 3");
            this.shopItemRepo.GetAll().Returns(new List<ShopItem> { shopItem1, shopItem2, shopItem3 });

            IEnumerable<ShopItem> result = this.shopItemService.SearchItemsByName(1, "unmatched");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void RemoveShopItem_ExistingShopItem_CallsRepoDelete()
        {
            this.shopItemService.RemoveShopItem(1);

            this.shopItemRepo.Received(1).Delete(1);
        }

        [Test]
        public void AddShopItem_ValidShopItem_CallsRepoAdd()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            this.shopItemService.AddShopItem(shopItem);

            this.shopItemRepo.Received(1).Add(shopItem);
        }

        [Test]
        public void AddShopItem_ZeroPrice_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(5, 0f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            var exception = Assert.Catch<ArgumentException>(() => this.shopItemService.AddShopItem(shopItem));

            Assert.That(exception!.Message, Does.Contain("Price"));
        }

        [Test]
        public void AddShopItem_NegativeStock_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(-5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            var exception = Assert.Catch<ArgumentException>(() => this.shopItemService.AddShopItem(shopItem));

            Assert.That(exception!.Message, Does.Contain("Quantity"));
        }

        [Test]
        public void AddShopItem_InvalidShopId_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(0, 10.5f, -1, "placeholder.png", "Test shop item", "Test shop item description");

            var exception = Assert.Catch<ArgumentException>(() => this.shopItemService.AddShopItem(shopItem));

            Assert.That(exception!.Message, Does.Contain("shop id"));
        }

        [Test]
        public void AddShopItem_WhitespaceName_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "   ", "Test shop item description");

            var exception = Assert.Catch<ArgumentException>(() => this.shopItemService.AddShopItem(shopItem));

            Assert.That(exception!.Message, Does.Contain("name"));
        }

        [Test]
        public void UpdateShopItem_ValidShopItem_CallsRepoUpdate()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            this.shopItemService.UpdateShopItem(shopItem);

            this.shopItemRepo.Received(1).Update(shopItem);
        }

        [Test]
        public void UpdateShopItem_ZeroPrice_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(5, 0f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            var exception = Assert.Catch<ArgumentException>(() => this.shopItemService.UpdateShopItem(shopItem));

            Assert.That(exception!.Message, Does.Contain("Price"));
        }

        [Test]
        public void UpdateShopItem_NegativeStock_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(-5, 10.5f, 1, "placeholder.png", "Test shop item", "Test shop item description");

            var exception = Assert.Catch<ArgumentException>(() => this.shopItemService.UpdateShopItem(shopItem));

            Assert.That(exception!.Message, Does.Contain("Quantity"));
        }

        [Test]
        public void UpdateShopItem_InvalidShopId_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, -1, "placeholder.png", "Test shop item", "Test shop item description");

            var exception = Assert.Catch<ArgumentException>(() => this.shopItemService.UpdateShopItem(shopItem));

            Assert.That(exception!.Message, Does.Contain("shop id"));
        }

        [Test]
        public void UpdateShopItem_WhitespaceName_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(5, 10.5f, 1, "placeholder.png", "   ", "Test shop item description");

            var exception = Assert.Catch<ArgumentException>(() => this.shopItemService.UpdateShopItem(shopItem));

            Assert.That(exception!.Message, Does.Contain("name"));
        }

        [Test]
        public void GetItemsSortedByPrice_ShopHasItems_ReturnsItemsSortedByPrice()
        {
            Shop shop = new Shop(1, "Test Shop", "Retail", 1);
            ShopItem shopItem1 = new ShopItem(5, 20.5f, 1, "placeholder.png", "Test shop item 1", "Test shop item description 1");
            ShopItem shopItem2 = new ShopItem(10, 10.5f, 1, "placeholder.png", "Test shop item 2", "Test shop item description 2");
            this.shopItemRepo.GetAll().Returns(new List<ShopItem> { shopItem1, shopItem2 });

            IEnumerable<ShopItem> result = this.shopItemService.GetItemsSortedByPrice(shop);

            Assert.That(result.ToList(), Is.EqualTo(new List<ShopItem> { shopItem2, shopItem1 }));
        }

        [Test]
        public void GetItemsSortedAlphabetically_ShopHasItems_ReturnsItemsSortedAlphabetically()
        {
            Shop shop = new Shop(1, "Test Shop", "Retail", 1);
            ShopItem shopItem1 = new ShopItem(5, 20.5f, 1, "placeholder.png", "B Item", "Test shop item description 1");
            ShopItem shopItem2 = new ShopItem(10, 10.5f, 1, "placeholder.png", "A Item", "Test shop item description 2");
            this.shopItemRepo.GetAll().Returns(new List<ShopItem> { shopItem1, shopItem2 });

            IEnumerable<ShopItem> result = this.shopItemService.GetItemsSortedAlphabetically(shop);

            Assert.That(result.ToList(), Is.EqualTo(new List<ShopItem> { shopItem2, shopItem1 }));
        }
    }
}