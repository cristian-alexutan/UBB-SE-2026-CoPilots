using Content.Domain;
using Content.Repository;
using Content.Service;

namespace Tests.Service
{
    [TestFixture]
    public class CartServiceTests
    {
        private CartMockRepo cartRepo;
        private ShopItemMockRepo shopItemRepo;
        private ShopItemService shopItemService;
        private CartService service;
        private Client client;

        [SetUp]
        public void Setup()
        {
            this.cartRepo = new CartMockRepo();
            this.shopItemRepo = new ShopItemMockRepo();
            this.shopItemService = new ShopItemService(this.shopItemRepo);
            this.service = new CartService(this.cartRepo, this.shopItemService);
            this.client = new Client(1, "Test Client");
        }

        private ShopItem AddShopItem(string name, int quantity, float price = 10f)
        {
            var item = new ShopItem(0, quantity, price, 1, string.Empty, name, "desc");
            this.shopItemRepo.Add(item);
            return item;
        }

        private Cart AddCart(int cartId)
        {
            var cart = new Cart(cartId, this.client, new Dictionary<int, CartItem>());
            this.service.AddCart(cart);
            return cart;
        }

        [Test]
        public void GetCartById_WhenCartExists_ReturnsCart()
        {
            AddCart(1);

            var result = this.service.GetCartById(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public void GetCartById_WhenCartDoesNotExist_ReturnsNull()
        {
            var result = this.service.GetCartById(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAllCarts_WhenTwoCartsAdded_ReturnsBoth()
        {
            AddCart(1);
            AddCart(2);

            var all = this.service.GetAllCarts().ToList();

            Assert.That(all, Has.Count.EqualTo(2));
        }

        [Test]
        public void DeleteCart_WhenCalled_CartIsNoLongerRetrievable()
        {
            AddCart(1);

            this.service.DeleteCart(1);

            Assert.That(this.service.GetCartById(1), Is.Null);
        }

        [Test]
        public void AddItemToCart_WhenItemNotInCartAndStockSufficient_AddsCartItem()
        {
            var shopItem = AddShopItem("Wine", 10);
            AddCart(1);

            this.service.AddItemToCart(1, new CartItem(0, shopItem, 2));

            var cart = this.service.GetCartById(1);
            Assert.That(cart.CartItems.Values.Any(ci => ci.ShopItem.Id == shopItem.Id && ci.Quantity == 2), Is.True);
        }

        [Test]
        public void AddItemToCart_WhenItemAlreadyInCart_UpdatesQuantityInsteadOfDuplicating()
        {
            var shopItem = AddShopItem("Wine", 10);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 2));

            this.service.AddItemToCart(1, new CartItem(0, shopItem, 3));

            var cart = this.service.GetCartById(1);
            Assert.That(cart.CartItems.Values.Count(ci => ci.ShopItem.Id == shopItem.Id), Is.EqualTo(1));
            Assert.That(cart.CartItems.Values.First(ci => ci.ShopItem.Id == shopItem.Id).Quantity, Is.EqualTo(5));
        }

        [Test]
        public void AddItemToCart_WhenQuantityExceedsStock_ThrowsInvalidOperationException()
        {
            var shopItem = AddShopItem("Wine", 2);
            AddCart(1);

            Assert.Throws<InvalidOperationException>(() =>
                this.service.AddItemToCart(1, new CartItem(0, shopItem, 5)));
        }

        [Test]
        public void AddItemToCart_WhenCombinedQuantityExceedsStock_ThrowsInvalidOperationException()
        {
            var shopItem = AddShopItem("Wine", 5);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 3));

            Assert.Throws<InvalidOperationException>(() =>
                this.service.AddItemToCart(1, new CartItem(0, shopItem, 4)));
        }

        [Test]
        public void AddItemToCart_WhenStockIsZero_ThrowsInvalidOperationException()
        {
            var shopItem = AddShopItem("Wine", 0);
            AddCart(1);

            Assert.Throws<InvalidOperationException>(() =>
                this.service.AddItemToCart(1, new CartItem(0, shopItem, 1)));
        }

        [Test]
        public void UpdateItemQuantity_WhenNewQuantityWithinStock_UpdatesQuantity()
        {
            var shopItem = AddShopItem("Wine", 10);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 2));
            var cartItem = this.service.GetCartById(1).CartItems.Values.First();

            this.service.UpdateItemQuantity(1, cartItem.Id, 5);

            var updated = this.service.GetCartById(1).CartItems.Values.First();
            Assert.That(updated.Quantity, Is.EqualTo(5));
        }

        [Test]
        public void UpdateItemQuantity_WhenNewQuantityExceedsStock_ThrowsInvalidOperationException()
        {
            var shopItem = AddShopItem("Wine", 5);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 2));
            var cartItem = this.service.GetCartById(1).CartItems.Values.First();

            Assert.Throws<InvalidOperationException>(() =>
                this.service.UpdateItemQuantity(1, cartItem.Id, 10));
        }

        [Test]
        public void RemoveItemFromCart_WhenItemExists_ItemNoLongerInCart()
        {
            var shopItem = AddShopItem("Wine", 10);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 2));
            var cartItem = this.service.GetCartById(1).CartItems.Values.First();

            this.service.RemoveItemFromCart(1, cartItem.Id);

            Assert.That(this.service.GetCartById(1).CartItems, Is.Empty);
        }

        [Test]
        public void ClearCart_WhenCartHasMultipleItems_AllItemsRemoved()
        {
            var shopItemA = AddShopItem("Wine", 10);
            var shopItemB = AddShopItem("Beer", 10);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItemA, 1));
            this.service.AddItemToCart(1, new CartItem(0, shopItemB, 1));

            this.service.ClearCart(1);

            Assert.That(this.service.GetCartById(1).CartItems, Is.Empty);
        }
        [Test]
        public void DecreaseItemQuantity_WhenQuantityGreaterThanOne_DecreasesQuantityByOne()
        {
            var shopItem = AddShopItem("Wine", 10);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 3));
            var cartItem = this.service.GetCartById(1).CartItems.Values.First();

            this.service.DecreaseItemQuantity(1, cartItem.Id);

            Assert.That(this.service.GetCartById(1).CartItems.Values.First().Quantity, Is.EqualTo(2));
        }

        [Test]
        public void DecreaseItemQuantity_WhenQuantityIsOne_RemovesItemFromCart()
        {
            var shopItem = AddShopItem("Wine", 10);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 1));
            var cartItem = this.service.GetCartById(1).CartItems.Values.First();

            this.service.DecreaseItemQuantity(1, cartItem.Id);

            Assert.That(this.service.GetCartById(1).CartItems, Is.Empty);
        }

        [Test]
        public void DecreaseItemQuantity_WhenCartItemDoesNotExist_DoesNothing()
        {
            AddCart(1);

            Assert.DoesNotThrow(() => this.service.DecreaseItemQuantity(1, 999));

            Assert.That(this.service.GetCartById(1).CartItems, Is.Empty);
        }

        [Test]
        public void GetCartTotal_WhenCartIsEmpty_ReturnsZero()
        {
            AddCart(1);

            var total = this.service.GetCartTotal(1);

            Assert.That(total, Is.EqualTo(0));
        }

        [Test]
        public void GetCartTotal_WhenCartHasItems_ReturnsCorrectSum()
        {
            var shopItem = AddShopItem("Wine", 10, 5.0f);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 3));

            var total = this.service.GetCartTotal(1);

            Assert.That(total, Is.EqualTo(15.0f).Within(0.001));
        }

        [Test]
        public void IsLastCartItem_WhenQuantityIsOne_ReturnsTrue()
        {
            var shopItem = AddShopItem("Wine", 10);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 1));
            var cartItem = this.service.GetCartById(1).CartItems.Values.First();

            var result = this.service.IsLastCartItem(1, cartItem.Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsLastCartItem_WhenQuantityGreaterThanOne_ReturnsFalse()
        {
            var shopItem = AddShopItem("Wine", 10);
            AddCart(1);
            this.service.AddItemToCart(1, new CartItem(0, shopItem, 3));
            var cartItem = this.service.GetCartById(1).CartItems.Values.First();

            var result = this.service.IsLastCartItem(1, cartItem.Id);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsLastCartItem_WhenCartItemDoesNotExist_ReturnsFalse()
        {
            AddCart(1);

            var result = this.service.IsLastCartItem(1, 999);

            Assert.That(result, Is.False);
        }
    }
}