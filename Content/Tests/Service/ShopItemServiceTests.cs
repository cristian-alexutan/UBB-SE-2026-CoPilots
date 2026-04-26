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
        private IShopItemRepo shopItemRepository;
        private ShopItemService shopItemService;

        [SetUp]
        public void SetUp()
        {
            shopItemRepository = Substitute.For<IShopItemRepo>();
            shopItemService = new ShopItemService(shopItemRepository);
        }

        [Test]
        public void GetById_ItemDoesNotExist_ThrowsInvalidOperationException()
        {
            shopItemRepository.GetById(1).Returns((ShopItem)null);

            Assert.Throws<InvalidOperationException>(() => shopItemService.GetById(1));
        }

        [Test]
        public void GetById_ItemExists_ReturnsItem()
        {
            ShopItem existingShopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Item", string.Empty);
            shopItemRepository.GetById(1).Returns(existingShopItem);

            ShopItem getByIdResult = shopItemService.GetById(1);

            Assert.That(getByIdResult, Is.EqualTo(existingShopItem));
        }

        [Test]
        public void GetItemsByShopId_ItemsInSameShopExist_ReturnsMatchingItems()
        {
            ShopItem existingShopItem1 = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Item", string.Empty);
            ShopItem existingShopItem2 = new ShopItem(2, 10, 5.0f, 2, string.Empty, "Other", string.Empty);
            shopItemRepository.GetAll().Returns(new List<ShopItem> { existingShopItem1, existingShopItem2 });

            IEnumerable<ShopItem> itemsByShopIdResult = shopItemService.GetItemsByShopId(1);

            Assert.That(itemsByShopIdResult, Does.Not.Contain(existingShopItem2));
        }

        [Test]
        public void GetItemsByShopId_NoMatchingItems_ReturnsEmptyCollection()
        {
            shopItemRepository.GetAll().Returns(new List<ShopItem>());

            IEnumerable<ShopItem> emptyShopItemsCollection = shopItemService.GetItemsByShopId(1);

            Assert.That(emptyShopItemsCollection, Is.Empty);
        }

        [Test]
        public void SearchItemsByName_NullSearchText_ReturnsAllItemsInShop()
        {
            ShopItem existingShopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Item", string.Empty);
            shopItemRepository.GetAll().Returns(new List<ShopItem> { existingShopItem });

            IEnumerable<ShopItem> result = shopItemService.SearchItemsByName(1, null);

            Assert.That(result, Contains.Item(existingShopItem));
        }

        [Test]
        public void SearchItemsByName_TextMatchesItemName_ReturnsMatchingItem()
        {
            ShopItem existingShopItem1 = new ShopItem(1, 10, 5.0f, 1, string.Empty, "A Item", string.Empty);
            ShopItem existingShopItem2 = new ShopItem(2, 10, 5.0f, 1, string.Empty, "B Item", string.Empty);
            shopItemRepository.GetAll().Returns(new List<ShopItem> { existingShopItem1, existingShopItem2 });

            IEnumerable<ShopItem> result = shopItemService.SearchItemsByName(1, "A");

            Assert.That(result, Does.Not.Contain(existingShopItem2));
        }

        [Test]
        public void SearchItemsByName_TextMatchesNone_ReturnsEmptyCollection()
        {
            ShopItem existingShopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Item", string.Empty);
            shopItemRepository.GetAll().Returns(new List<ShopItem> { existingShopItem });

            IEnumerable<ShopItem> result = shopItemService.SearchItemsByName(1, "test");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void AddShopItem_ShopIdIsZero_ThrowsArgumentException()
        {
            ShopItem invalidShopItem = new ShopItem(0, 10, 5.0f, 0, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.AddShopItem(invalidShopItem));
        }

        [Test]
        public void AddShopItem_NegativeQuantity_ThrowsArgumentException()
        {
            ShopItem invalidShopItem = new ShopItem(0, -1, 5.0f, 1, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.AddShopItem(invalidShopItem));
        }

        [Test]
        public void AddShopItem_PriceIsZero_ThrowsArgumentException()
        {
            ShopItem invalidShopItem = new ShopItem(0, 10, 0.0f, 1, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.AddShopItem(invalidShopItem));
        }

        [Test]
        public void AddShopItem_EmptyName_ThrowsArgumentException()
        {
            ShopItem invalidShopItem = new ShopItem(0, 10, 5.0f, 1, string.Empty, string.Empty, string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.AddShopItem(invalidShopItem));
        }

        [Test]
        public void AddShopItem_ValidItem_CallsRepositoryAdd()
        {
            ShopItem validShopItem = new ShopItem(0, 10, 5.0f, 1, string.Empty, "Item", string.Empty);

            shopItemService.AddShopItem(validShopItem);

            shopItemRepository.Received().Add(validShopItem);
        }

        [Test]
        public void UpdateShopItem_ShopIdIsZero_ThrowsArgumentException()
        {
            ShopItem invalidShopItem = new ShopItem(1, 10, 5.0f, 0, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.UpdateShopItem(invalidShopItem));
        }

        [Test]
        public void UpdateShopItem_NegativeQuantity_ThrowsArgumentException()
        {
            ShopItem invalidShopItem = new ShopItem(1, -1, 5.0f, 1, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.UpdateShopItem(invalidShopItem));
        }

        [Test]
        public void UpdateShopItem_PriceIsZero_ThrowsArgumentException()
        {
            ShopItem invalidShopItem = new ShopItem(1, 10, 0.0f, 1, string.Empty, "Item", string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.UpdateShopItem(invalidShopItem));
        }

        [Test]
        public void UpdateShopItem_EmptyName_ThrowsArgumentException()
        {
            ShopItem invalidShopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, string.Empty, string.Empty);

            Assert.Throws<ArgumentException>(() => shopItemService.UpdateShopItem(invalidShopItem));
        }

        [Test]
        public void UpdateShopItem_ValidItem_CallsRepositoryUpdate()
        {
            ShopItem validShopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Item", string.Empty);

            shopItemService.UpdateShopItem(validShopItem);

            shopItemRepository.Received().Update(validShopItem);
        }

        [Test]
        public void RemoveShopItem_ExistingItemId_CallsRepositoryDelete()
        {
            shopItemService.RemoveShopItem(1);

            shopItemRepository.Received().Delete(1);
        }

        [Test]
        public void GetItemsSortedByPrice_NullShop_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => shopItemService.GetItemsSortedByPrice(null));
        }

        [Test]
        public void GetItemsSortedByPrice_ValidShop_ReturnsCheapestItemFirst()
        {
            ShopItem cheapShopItem = new ShopItem(1, 10, 1.0f, 1, string.Empty, "Cheap item", string.Empty);
            ShopItem expensiveShopItem = new ShopItem(2, 10, 10.0f, 1, string.Empty, "Expensive item", string.Empty);
            shopItemRepository.GetAll().Returns(new List<ShopItem> { expensiveShopItem, cheapShopItem });
            Shop shop = new Shop(1, "Shop", string.Empty, 0);

            IEnumerable<ShopItem> result = shopItemService.GetItemsSortedByPrice(shop);

            Assert.That(result.First(), Is.EqualTo(cheapShopItem));
        }

        [Test]
        public void GetItemsSortedAlphabetically_NullShop_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => shopItemService.GetItemsSortedAlphabetically(null));
        }

        [Test]
        public void GetItemsSortedAlphabetically_ValidShop_ReturnsFirstItemAlphabetically()
        {
            ShopItem secondShopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Second item", string.Empty);
            ShopItem firstShopItem = new ShopItem(2, 10, 5.0f, 1, string.Empty, "First item", string.Empty);
            shopItemRepository.GetAll().Returns(new List<ShopItem> { secondShopItem, firstShopItem });
            Shop shop = new Shop(1, "Shop", string.Empty, 0);

            IEnumerable<ShopItem> result = shopItemService.GetItemsSortedAlphabetically(shop);

            Assert.That(result.First(), Is.EqualTo(firstShopItem));
        }
    }
}