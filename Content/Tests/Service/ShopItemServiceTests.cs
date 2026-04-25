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
        private IShopItemRepo shopItemRepo;
        private ShopItemService shopItemService;

        [SetUp]
        public void SetUp()
        {
            shopItemRepo = Substitute.For<IShopItemRepo>();
            shopItemService = new ShopItemService(shopItemRepo);
        }

        [Test]
        public void GetById_ItemDoesNotExist_ThrowsInvalidOperationException()
        {
            shopItemRepo.GetById(1).Returns((ShopItem)null);

            Assert.Throws<InvalidOperationException>(() => shopItemService.GetById(1));
        }

        [Test]
        public void GetById_ItemExists_ReturnsItem()
        {
            ShopItem shopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Item", string.Empty);
            shopItemRepo.GetById(1).Returns(shopItem);

            ShopItem result = shopItemService.GetById(1);

            Assert.That(result, Is.EqualTo(shopItem));
        }

        [Test]
        public void GetItemsByShopId_ItemsWithMatchingShopExist_ReturnsOnlyMatchingItems()
        {
            ShopItem item1 = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Item", string.Empty);
            ShopItem item2 = new ShopItem(2, 10, 5.0f, 2, string.Empty, "Other", string.Empty);
            shopItemRepo.GetAll().Returns(new List<ShopItem> { item1, item2 });

            IEnumerable<ShopItem> result = shopItemService.GetItemsByShopId(1);

            Assert.That(result, Does.Not.Contain(item2));
        }

        [Test]
        public void GetItemsByShopId_NoMatchingItems_ReturnsEmptyCollection()
        {
            shopItemRepo.GetAll().Returns(new List<ShopItem>());

            IEnumerable<ShopItem> result = shopItemService.GetItemsByShopId(1);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void SearchItemsByName_NullSearchText_ReturnsAllItemsForShop()
        {
            ShopItem item1 = new ShopItem(1, 10, 5.0f, 1, string.Empty, "A Item", string.Empty);
            shopItemRepo.GetAll().Returns(new List<ShopItem> { item1 });

            IEnumerable<ShopItem> result = shopItemService.SearchItemsByName(1, null);

            Assert.That(result, Contains.Item(item1));
        }

        [Test]
        public void SearchItemsByName_TextMatchesItemName_ReturnsMatchingItem()
        {
            ShopItem item1 = new ShopItem(1, 10, 5.0f, 1, string.Empty, "A Item", string.Empty);
            ShopItem item2 = new ShopItem(2, 10, 5.0f, 1, string.Empty, "B Item", string.Empty);
            shopItemRepo.GetAll().Returns(new List<ShopItem> { item1, item2 });

            IEnumerable<ShopItem> result = shopItemService.SearchItemsByName(1, "A");

            Assert.That(result, Does.Not.Contain(item2));
        }

        [Test]
        public void SearchItemsByName_TextMatchesNone_ReturnsEmptyCollection()
        {
            ShopItem item1 = new ShopItem(1, 10, 5.0f, 1, string.Empty, "A Item", string.Empty);
            shopItemRepo.GetAll().Returns(new List<ShopItem> { item1 });

            IEnumerable<ShopItem> result = shopItemService.SearchItemsByName(1, "xyz");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void AddShopItem_ShopIdIsZero_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(0, 10, 5.0f, 0, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.AddShopItem(shopItem));
        }

        [Test]
        public void AddShopItem_NegativeQuantity_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(0, -1, 5.0f, 1, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.AddShopItem(shopItem));
        }

        [Test]
        public void AddShopItem_PriceIsZero_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(0, 10, 0.0f, 1, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.AddShopItem(shopItem));
        }

        [Test]
        public void AddShopItem_EmptyName_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(0, 10, 5.0f, 1, string.Empty, string.Empty, string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.AddShopItem(shopItem));
        }

        [Test]
        public void AddShopItem_ValidItem_CallsRepositoryAdd()
        {
            ShopItem shopItem = new ShopItem(0, 10, 5.0f, 1, string.Empty, "Item", string.Empty);

            shopItemService.AddShopItem(shopItem);

            shopItemRepo.Received().Add(shopItem);
        }

        [Test]
        public void UpdateShopItem_ShopIdIsZero_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(1, 10, 5.0f, 0, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.UpdateShopItem(shopItem));
        }

        [Test]
        public void UpdateShopItem_NegativeQuantity_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(1, -1, 5.0f, 1, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.UpdateShopItem(shopItem));
        }

        [Test]
        public void UpdateShopItem_PriceIsZero_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(1, 10, 0.0f, 1, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.UpdateShopItem(shopItem));
        }

        [Test]
        public void UpdateShopItem_EmptyName_ThrowsArgumentException()
        {
            ShopItem shopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, string.Empty, string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.UpdateShopItem(shopItem));
        }

        [Test]
        public void UpdateShopItem_ValidItem_CallsRepositoryUpdate()
        {
            ShopItem shopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Item", string.Empty);

            shopItemService.UpdateShopItem(shopItem);

            shopItemRepo.Received().Update(shopItem);
        }

        [Test]
        public void RemoveShopItem_ExistingItemId_CallsRepositoryDelete()
        {
            shopItemService.RemoveShopItem(1);

            shopItemRepo.Received().Delete(1);
        }

        [Test]
        public void GetItemsSortedByPrice_NullShop_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => shopItemService.GetItemsSortedByPrice(null));
        }

        [Test]
        public void GetItemsSortedByPrice_ValidShop_ReturnsCheapestItemFirst()
        {
            ShopItem item1 = new ShopItem(1, 10, 1.0f, 1, string.Empty, "A Item", string.Empty);
            ShopItem item2 = new ShopItem(2, 10, 10.0f, 1, string.Empty, "B Item", string.Empty);
            shopItemRepo.GetAll().Returns(new List<ShopItem> { item2, item1 });
            Shop shop = new Shop(1, "Shop", string.Empty, 0);

            IEnumerable<ShopItem> result = shopItemService.GetItemsSortedByPrice(shop);

            Assert.That(result.First(), Is.EqualTo(item1));
        }

        [Test]
        public void GetItemsSortedAlphabetically_NullShop_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => shopItemService.GetItemsSortedAlphabetically(null));
        }

        [Test]
        public void GetItemsSortedAlphabetically_ValidShop_ReturnsFirstItemAlphabetically()
        {
            ShopItem item1 = new ShopItem(1, 10, 5.0f, 1, string.Empty, "B Item", string.Empty);
            ShopItem item2 = new ShopItem(2, 10, 5.0f, 1, string.Empty, "A Item", string.Empty);
            shopItemRepo.GetAll().Returns(new List<ShopItem> { item1, item2 });
            Shop shop = new Shop(1, "Shop", string.Empty, 0);

            IEnumerable<ShopItem> result = shopItemService.GetItemsSortedAlphabetically(shop);

            Assert.That(result.First(), Is.EqualTo(item2));
        }
    }
}