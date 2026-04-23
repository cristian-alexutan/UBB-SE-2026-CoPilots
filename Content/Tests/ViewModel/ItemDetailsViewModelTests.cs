using System;
using System.Collections.Generic;
using Content.Domain;
using Content.Service;
using Content.User;
using Content.ViewModel;
using NSubstitute;
using NUnit.Framework;

namespace Tests.ViewModel
{
    [TestFixture]
    public class ItemDetailsViewModelTests
    {
        private ICartService cartService = null!;
        private IShopItemService shopItemService = null!;
        private UserSession session = null!;
        private Shop shop = null!;

        [SetUp]
        public void SetUp()
        {
            this.cartService = Substitute.For<ICartService>();
            this.shopItemService = Substitute.For<IShopItemService>();
            this.session = new UserSession();
            this.session.SetClient(7);
            this.shop = new Shop(1, "Shop", "Type", 1);
        }

        private ShopItem MakeItem(string name = "Item", int quantity = 10, float price = 20f)
        {
            return new ShopItem(1, quantity, price, this.shop.Id, "photo.jpg", name, "description");
        }

        private ItemDetailsViewModel CreateViewModel(ShopItem item)
        {
            return new ItemDetailsViewModel(this.cartService, this.shopItemService, this.session, item, this.shop);
        }

        [Test]
        public void SetQuantity_ValidNumber_Works()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());

            viewModel.SetQuantityFromText("5");

            Assert.That(viewModel.Quantity, Is.EqualTo(5));
        }

        [Test]
        public void SetQuantity_Text_DefaultsToOne()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());

            viewModel.SetQuantityFromText("abc");

            Assert.That(viewModel.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void SetQuantity_NegativeNumber_DefaultsToOne()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());

            viewModel.SetQuantityFromText("-3");

            Assert.That(viewModel.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void AddToCart_LowStock_Error()
        {
            var item = this.MakeItem(quantity: 2);
            var viewModel = this.CreateViewModel(item);
            viewModel.SetQuantityFromText("5");

            string errorMessage = null;
            viewModel.ErrorOccurred += (_, message) => errorMessage = message;

            viewModel.AddToCartCommand.Execute(null);

            Assert.That(errorMessage, Does.Contain("only 2 are available"));
        }

        [Test]
        public void AddToCart_LowStock_Stops()
        {
            var item = this.MakeItem(quantity: 2);
            var viewModel = this.CreateViewModel(item);
            viewModel.SetQuantityFromText("5");

            viewModel.AddToCartCommand.Execute(null);

            this.cartService.DidNotReceive().AddItemToCart(Arg.Any<int>(), Arg.Any<CartItem>());
        }

        [Test]
        public void AddToCart_GoodStock_AddsItem()
        {
            var item = this.MakeItem(quantity: 10);
            var existingCart = new Cart(this.session.UserId, new Client(this.session.UserId, "Client"), new Dictionary<int, CartItem>());
            this.cartService.GetCartById(this.session.UserId).Returns(existingCart);

            var viewModel = this.CreateViewModel(item);
            viewModel.SetQuantityFromText("3");

            viewModel.AddToCartCommand.Execute(null);

            this.cartService.Received(1).AddItemToCart(Arg.Is(existingCart.Id), Arg.Is<CartItem>(ci => ci.Quantity == 3));
        }

        [Test]
        public void AddToCart_GoodStock_Success()
        {
            var item = this.MakeItem(quantity: 10);
            var existingCart = new Cart(this.session.UserId, new Client(this.session.UserId, "Client"), new Dictionary<int, CartItem>());
            this.cartService.GetCartById(this.session.UserId).Returns(existingCart);

            var viewModel = this.CreateViewModel(item);
            viewModel.SetQuantityFromText("3");

            var eventFired = false;
            viewModel.AddedToCartSuccessfully += (_, _) => eventFired = true;

            viewModel.AddToCartCommand.Execute(null);

            Assert.That(eventFired, Is.True);
        }

        [Test]
        public void AddToCart_MissingCart_CreatesOne()
        {
            var item = this.MakeItem(quantity: 10);
            this.cartService.GetCartById(this.session.UserId).Returns((Cart)null);

            var viewModel = this.CreateViewModel(item);
            viewModel.SetQuantityFromText("2");

            viewModel.AddToCartCommand.Execute(null);

            this.cartService.Received(1).AddCart(Arg.Is<Cart>(c => c.Id == this.session.UserId));
        }

        [Test]
        public void AddToCart_Crash_Error()
        {
            var item = this.MakeItem(quantity: 5);
            var existingCart = new Cart(this.session.UserId, new Client(this.session.UserId, "Client"), new Dictionary<int, CartItem>());
            this.cartService.GetCartById(this.session.UserId).Returns(existingCart);
            this.cartService
                .When(service => service.AddItemToCart(Arg.Any<int>(), Arg.Any<CartItem>()))
                .Do(_ => throw new InvalidOperationException("Not enough stock."));

            var viewModel = this.CreateViewModel(item);
            viewModel.SetQuantityFromText("2");

            string errorMessage = null;
            viewModel.ErrorOccurred += (_, message) => errorMessage = message;

            viewModel.AddToCartCommand.Execute(null);

            Assert.That(errorMessage, Is.EqualTo("Not enough stock."));
        }

        [Test]
        public void AddToCart_Crash_NoSuccess()
        {
            var item = this.MakeItem(quantity: 5);
            var existingCart = new Cart(this.session.UserId, new Client(this.session.UserId, "Client"), new Dictionary<int, CartItem>());
            this.cartService.GetCartById(this.session.UserId).Returns(existingCart);
            this.cartService
                .When(service => service.AddItemToCart(Arg.Any<int>(), Arg.Any<CartItem>()))
                .Do(_ => throw new InvalidOperationException("Not enough stock."));

            var viewModel = this.CreateViewModel(item);
            viewModel.SetQuantityFromText("2");

            var successEventFired = false;
            viewModel.AddedToCartSuccessfully += (_, _) => successEventFired = true;

            viewModel.AddToCartCommand.Execute(null);

            Assert.That(successEventFired, Is.False);
        }

        [Test]
        public void Save_NegativeStock_Stops()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());
            viewModel.EditPrice = "10";
            viewModel.EditStock = "-1";

            viewModel.SaveChangesCommand.Execute(null);

            this.shopItemService.DidNotReceive().UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void Save_TextStock_Stops()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());
            viewModel.EditPrice = "10";
            viewModel.EditStock = "abc";

            viewModel.SaveChangesCommand.Execute(null);

            this.shopItemService.DidNotReceive().UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void Save_ZeroPrice_Stops()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());
            viewModel.EditPrice = "0";
            viewModel.EditStock = "10";

            viewModel.SaveChangesCommand.Execute(null);

            this.shopItemService.DidNotReceive().UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void Save_TextPrice_Stops()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());
            viewModel.EditPrice = "abc";
            viewModel.EditStock = "10";

            viewModel.SaveChangesCommand.Execute(null);

            this.shopItemService.DidNotReceive().UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void Save_NullPrice_Stops()
        {
            var viewModel = this.CreateViewModel(this.MakeItem(quantity: 10, price: 20f));
            viewModel.EditName = "Updated Name";
            viewModel.EditDescription = "Updated description";
            viewModel.EditPrice = null;
            viewModel.EditStock = "4";

            viewModel.SaveChangesCommand.Execute(null);

            this.shopItemService.DidNotReceive().UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void Save_GoodData_UpdatesItem()
        {
            var viewModel = this.CreateViewModel(this.MakeItem(quantity: 10, price: 20f));
            viewModel.EditName = "Updated Name";
            viewModel.EditDescription = "Updated Description";
            viewModel.EditPrice = "12.5";
            viewModel.EditStock = "4";

            viewModel.SaveChangesCommand.Execute(null);

            this.shopItemService.Received(1).UpdateShopItem(Arg.Is<ShopItem>(si =>
                si.Name == "Updated Name" &&
                si.Price == 12.5f &&
                si.Quantity == 4));
        }

        [Test]
        public void Save_GoodData_CleansInput()
        {
            var viewModel = this.CreateViewModel(this.MakeItem(quantity: 10, price: 20f));
            viewModel.EditName = "  Updated Name  ";
            viewModel.EditDescription = null;
            viewModel.EditPrice = " 12.5 ";
            viewModel.EditStock = "4";

            viewModel.SaveChangesCommand.Execute(null);

            this.shopItemService.Received(1).UpdateShopItem(Arg.Is<ShopItem>(si =>
                si.Name == "Updated Name" &&
                si.Description == string.Empty &&
                si.Price == 12.5f &&
                si.Quantity == 4));
        }

        [Test]
        public void Save_NullName_MakesEmpty()
        {
            var viewModel = this.CreateViewModel(this.MakeItem(quantity: 10, price: 20f));
            viewModel.EditName = null;
            viewModel.EditDescription = "Updated description";
            viewModel.EditPrice = "12.5";
            viewModel.EditStock = "4";

            viewModel.SaveChangesCommand.Execute(null);

            this.shopItemService.Received(1).UpdateShopItem(Arg.Is<ShopItem>(si =>
                si.Name == string.Empty &&
                si.Description == "Updated description" &&
                si.Price == 12.5f &&
                si.Quantity == 4));
        }

        [Test]
        public void Increment_AddsOne()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());
            viewModel.SetQuantityFromText("1");

            viewModel.IncrementQuantityCommand.Execute(null);

            Assert.That(viewModel.Quantity, Is.EqualTo(2));
        }

        [Test]
        public void Decrement_SubtractsOne()
        {
            var viewModel = this.CreateViewModel(this.MakeItem());
            viewModel.SetQuantityFromText("2");

            viewModel.DecrementQuantityCommand.Execute(null);

            Assert.That(viewModel.Quantity, Is.EqualTo(1));
        }
    }
}