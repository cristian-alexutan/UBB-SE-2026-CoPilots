using Content.Domain;
using Content.Repository;

namespace Tests.Repository
{
    [TestFixture]
    public class CartMemoryRepoTests
    {
        private CartMemoryRepo repo;
        private Client client;

        [SetUp]
        public void Setup()
        {
            this.repo = new CartMemoryRepo();
            this.client = new Client(1, "Test Client");
        }

        private Cart MakeCart(int id)
        {
            return new Cart(id, this.client, new Dictionary<int, CartItem>());
        }

        private ShopItem MakeShopItem(int id)
        {
            return new ShopItem(id, 10, 5.0f, 1, string.Empty, "Item", "desc");
        }

        [Test]
        public void GetById_AfterAdd_ReturnsAddedCart()
        {
            this.repo.Add(MakeCart(1));

            var result = this.repo.GetById(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public void GetById_WhenCartNotAdded_ReturnsNull()
        {
            var result = this.repo.GetById(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAll_WhenTwoCartsAdded_ReturnsBoth()
        {
            this.repo.Add(MakeCart(1));
            this.repo.Add(MakeCart(2));

            Assert.That(this.repo.GetAll().Count(), Is.EqualTo(2));
        }

        [Test]
        public void Delete_WhenCartExists_CartNoLongerRetrievable()
        {
            this.repo.Add(MakeCart(1));

            this.repo.Delete(1);

            Assert.That(this.repo.GetById(1), Is.Null);
        }

        [Test]
        public void AddItemToCart_WhenCalled_AssignsNonZeroIdToItem()
        {
            this.repo.Add(MakeCart(1));
            var item = new CartItem(0, MakeShopItem(1), 1);

            this.repo.AddItemToCart(1, item);

            Assert.That(item.Id, Is.Not.EqualTo(0));
        }

        [Test]
        public void AddItemToCart_WhenTwoItemsAdded_SecondItemHasHigherId()
        {
            this.repo.Add(MakeCart(1));
            var item1 = new CartItem(0, MakeShopItem(1), 1);
            var item2 = new CartItem(0, MakeShopItem(2), 1);

            this.repo.AddItemToCart(1, item1);
            this.repo.AddItemToCart(1, item2);

            Assert.That(item2.Id, Is.GreaterThan(item1.Id));
        }

        [Test]
        public void AddItemToCart_WhenAddedToOneCart_OtherCartRemainsEmpty()
        {
            this.repo.Add(MakeCart(1));
            this.repo.Add(MakeCart(2));

            this.repo.AddItemToCart(1, new CartItem(0, MakeShopItem(1), 1));

            Assert.That(this.repo.GetById(2).CartItems, Is.Empty);
        }

        [Test]
        public void RemoveItemFromCart_WhenItemExists_ItemRemovedFromCart()
        {
            this.repo.Add(MakeCart(1));
            var item = new CartItem(0, MakeShopItem(1), 1);
            this.repo.AddItemToCart(1, item);

            this.repo.RemoveItemFromCart(1, item.Id);

            Assert.That(this.repo.GetById(1).CartItems, Is.Empty);
        }

        [Test]
        public void UpdateItemQuantity_WhenItemExists_QuantityIsUpdated()
        {
            this.repo.Add(MakeCart(1));
            var item = new CartItem(0, MakeShopItem(1), 1);
            this.repo.AddItemToCart(1, item);

            this.repo.UpdateItemQuantity(1, item.Id, 5);

            Assert.That(this.repo.GetById(1).CartItems[item.Id].Quantity, Is.EqualTo(5));
        }

        [Test]
        public void ClearCart_WhenCartHasMultipleItems_AllItemsRemoved()
        {
            this.repo.Add(MakeCart(1));
            this.repo.AddItemToCart(1, new CartItem(0, MakeShopItem(1), 1));
            this.repo.AddItemToCart(1, new CartItem(0, MakeShopItem(2), 1));

            this.repo.ClearCart(1);

            Assert.That(this.repo.GetById(1).CartItems, Is.Empty);
        }
    }
}