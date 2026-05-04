using System.Collections.Generic;
using Content.Domain;
using Content.Repository.Interface;
using Content.Service;
using Content.Data.Service.Interface;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Service;

[TestFixture]
public class CartServiceTests
{
    private ICartRepo cartRepo = null!;
    private IShopItemService shopItemService = null!;
    private CartService cartService = null!;

    private static readonly Manager TestManager = new Manager(1, "Manager", "manager@test.com", "0700000000");
    private static readonly Shop TestShop = new Shop(1, "Test Shop", "Type", TestManager);

    [SetUp]
    public void Setup()
    {
        this.cartRepo = Substitute.For<ICartRepo>();
        this.shopItemService = Substitute.For<IShopItemService>();
        this.cartService = new CartService(this.cartRepo, this.shopItemService);
    }

    [Test]
    public void GetOrCreateCart_CartAlreadyExists_ReturnsExistingCart()
    {
        Cart existingCart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem>());
        this.cartRepo.GetById(1).Returns(existingCart);

        Cart result = this.cartService.GetOrCreateCart(1);

        Assert.That(result, Is.EqualTo(existingCart));
    }

    [Test]
    public void GetOrCreateCart_CartDoesNotExist_CreatesAndAddsNewCart()
    {
        this.cartRepo.GetById(1).Returns((Cart)null);

        this.cartService.GetOrCreateCart(1);

        this.cartRepo.Received(1).Add(Arg.Any<Cart>());
    }

    [Test]
    public void AddItemToCart_ItemNotInCartAndStockSufficient_AddsItemToRepo()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem>());
        CartItem cartItem = new CartItem(0, shopItem, 2);
        this.cartRepo.GetById(1).Returns(cart);
        this.shopItemService.GetById(1).Returns(shopItem);

        this.cartService.AddItemToCart(1, cartItem);

        this.cartRepo.Received(1).AddItemToCart(1, cartItem);
    }

    [Test]
    public void AddItemToCart_ItemAlreadyInCart_UpdatesCombinedQuantityInRepo()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem existingCartItem = new CartItem(1, shopItem, 3);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, existingCartItem } });
        CartItem newCartItem = new CartItem(0, shopItem, 2);
        this.cartRepo.GetById(1).Returns(cart);
        this.shopItemService.GetById(1).Returns(shopItem);

        this.cartService.AddItemToCart(1, newCartItem);

        this.cartRepo.Received(1).UpdateItemQuantity(1, 1, 5);
    }

    [Test]
    public void AddItemToCart_QuantityExceedsStock_ThrowsInvalidOperationException()
    {
        ShopItem shopItem = new ShopItem(1, 2, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem>());
        CartItem cartItem = new CartItem(0, shopItem, 5);
        this.cartRepo.GetById(1).Returns(cart);
        this.shopItemService.GetById(1).Returns(shopItem);

        var exception = Assert.Catch<InvalidOperationException>(() => this.cartService.AddItemToCart(1, cartItem));

        Assert.That(exception!.Message, Does.Contain("Not enough stock"));
    }

    [Test]
    public void AddItemToCart_CombinedQuantityExceedsStock_ThrowsInvalidOperationException()
    {
        ShopItem shopItem = new ShopItem(1, 5, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem existingCartItem = new CartItem(1, shopItem, 3);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, existingCartItem } });
        CartItem newCartItem = new CartItem(0, shopItem, 4);
        this.cartRepo.GetById(1).Returns(cart);
        this.shopItemService.GetById(1).Returns(shopItem);

        var exception = Assert.Catch<InvalidOperationException>(() => this.cartService.AddItemToCart(1, newCartItem));

        Assert.That(exception!.Message, Does.Contain("Not enough stock"));
    }

    [Test]
    public void AddItemToCart_ShopItemNotFound_ThrowsInvalidOperationException()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem>());
        CartItem cartItem = new CartItem(0, shopItem, 2);
        this.cartRepo.GetById(1).Returns(cart);
        this.shopItemService.GetById(1).Returns((ShopItem)null);

        var exception = Assert.Catch<InvalidOperationException>(() => this.cartService.AddItemToCart(1, cartItem));

        Assert.That(exception!.Message, Does.Contain("Not enough stock"));
    }

    [Test]
    public void UpdateItemQuantity_NewQuantityWithinStock_UpdatesQuantityInRepo()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem existingCartItem = new CartItem(1, shopItem, 2);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, existingCartItem } });
        this.cartRepo.GetById(1).Returns(cart);
        this.shopItemService.GetById(1).Returns(shopItem);

        this.cartService.UpdateItemQuantity(1, 1, 5);

        this.cartRepo.Received(1).UpdateItemQuantity(1, 1, 5);
    }

    [Test]
    public void UpdateItemQuantity_NewQuantityExceedsStock_ThrowsInvalidOperationException()
    {
        ShopItem shopItem = new ShopItem(1, 5, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem existingCartItem = new CartItem(1, shopItem, 2);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, existingCartItem } });
        this.cartRepo.GetById(1).Returns(cart);
        this.shopItemService.GetById(1).Returns(shopItem);

        var exception = Assert.Catch<InvalidOperationException>(() => this.cartService.UpdateItemQuantity(1, 1, 10));

        Assert.That(exception!.Message, Does.Contain("Not enough stock"));
    }

    [Test]
    public void GetCartTotal_CartNotFound_ReturnsZero()
    {
        this.cartRepo.GetById(1).Returns((Cart)null);

        double total = this.cartService.GetCartTotal(1);

        Assert.That(total, Is.EqualTo(0));
    }

    [Test]
    public void GetCartTotal_CartWithItems_ReturnsCorrectTotal()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem cartItem = new CartItem(1, shopItem, 3);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, cartItem } });
        this.cartRepo.GetById(1).Returns(cart);

        double total = this.cartService.GetCartTotal(1);

        Assert.That(total, Is.EqualTo(15.0f).Within(0.001));
    }

    [Test]
    public void DecreaseItemQuantity_QuantityGreaterThanOne_DecreasesQuantityByOneInRepo()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem cartItem = new CartItem(1, shopItem, 3);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, cartItem } });
        this.cartRepo.GetById(1).Returns(cart);

        this.cartService.DecreaseItemQuantity(1, 1);

        this.cartRepo.Received(1).UpdateItemQuantity(1, 1, 2);
    }

    [Test]
    public void DecreaseItemQuantity_QuantityIsOne_RemovesItemFromRepo()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem cartItem = new CartItem(1, shopItem, 1);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, cartItem } });
        this.cartRepo.GetById(1).Returns(cart);

        this.cartService.DecreaseItemQuantity(1, 1);

        this.cartRepo.Received(1).RemoveItemFromCart(1, 1);
    }

    [Test]
    public void DecreaseItemQuantity_CartItemNotFound_NoRepoCallsMade()
    {
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem>());
        this.cartRepo.GetById(1).Returns(cart);

        this.cartService.DecreaseItemQuantity(1, 999);

        this.cartRepo.DidNotReceive().UpdateItemQuantity(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>());
        this.cartRepo.DidNotReceive().RemoveItemFromCart(Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public void GetCartItems_CartNotFound_ReturnsEmptyCollection()
    {
        this.cartRepo.GetById(1).Returns((Cart)null);

        IEnumerable<CartItem> result = this.cartService.GetCartItems(1);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetCartItems_CartHasNoItems_ReturnsEmptyCollection()
    {
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem>());
        this.cartRepo.GetById(1).Returns(cart);

        IEnumerable<CartItem> result = this.cartService.GetCartItems(1);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetCartItems_CartWithItems_ReturnsAllCartItems()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem cartItem1 = new CartItem(1, shopItem, 2);
        CartItem cartItem2 = new CartItem(2, shopItem, 3);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, cartItem1 }, { 2, cartItem2 } });
        this.cartRepo.GetById(1).Returns(cart);

        IEnumerable<CartItem> result = this.cartService.GetCartItems(1);

        Assert.That(result, Is.EquivalentTo(new[] { cartItem1, cartItem2 }));
    }

    [Test]
    public void IsLastCartItem_QuantityIsOne_ReturnsTrue()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem cartItem = new CartItem(1, shopItem, 1);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, cartItem } });
        this.cartRepo.GetById(1).Returns(cart);

        bool result = this.cartService.IsLastCartItem(1, 1);

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsLastCartItem_QuantityGreaterThanOne_ReturnsFalse()
    {
        ShopItem shopItem = new ShopItem(1, 10, 5.0f, TestShop, string.Empty, "Test Item", "desc");
        CartItem cartItem = new CartItem(1, shopItem, 3);
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem> { { 1, cartItem } });
        this.cartRepo.GetById(1).Returns(cart);

        bool result = this.cartService.IsLastCartItem(1, 1);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsLastCartItem_CartNotFound_ReturnsFalse()
    {
        this.cartRepo.GetById(1).Returns((Cart)null);

        bool result = this.cartService.IsLastCartItem(1, 1);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsLastCartItem_CartItemNotFound_ReturnsFalse()
    {
        Cart cart = new Cart(1, new Client(1, "Test Client"), new Dictionary<int, CartItem>());
        this.cartRepo.GetById(1).Returns(cart);

        bool result = this.cartService.IsLastCartItem(1, 999);

        Assert.That(result, Is.False);
    }
}