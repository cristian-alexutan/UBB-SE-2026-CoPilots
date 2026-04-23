using System;
using System.Collections.Generic;
using System.Linq;
using Content.Data.Service.Interface;
using Content.Domain;
using Content.Service;
using Content.User;
using Content.ViewModel;
using NSubstitute;
using NUnit.Framework;

namespace Tests.ViewModel
{
    [TestFixture]
    public class CartViewModelTests
    {
        private ICartService cartService;
        private IReservationService reservationService;
        private UserSession session;
        private CartViewModel viewModel;
        private Cart testCart;
        private Client testClient;
        private ShopItem testShopItem;

        [SetUp]
        public void Setup()
        {
            this.cartService = Substitute.For<ICartService>();
            this.reservationService = Substitute.For<IReservationService>();
            this.session = new UserSession();
            this.session.SetClient(1);

            this.testClient = new Client(1, "Test Client");
            this.testShopItem = new ShopItem(1, 10, 5.0f, 1, string.Empty, "Test Item", "desc");
            this.testCart = new Cart(1, this.testClient, new Dictionary<int, CartItem>());

            this.cartService.GetCartById(1).Returns(this.testCart);
            this.cartService.GetCartTotal(1).Returns(0);
            this.reservationService.GetAllReservations().Returns(new List<Reservation>());

            this.viewModel = new CartViewModel(this.cartService, this.reservationService, this.session);
        }

        private CartShopItem AddItemToViewModel(int cartItemId, int quantity)
        {
            var cartItem = new CartItem(cartItemId, this.testShopItem, quantity);
            this.testCart.CartItems[cartItemId] = cartItem;

            var cartShopItem = new CartShopItem
            {
                CartItemId = cartItemId,
                ShopItem = this.testShopItem,
                Quantity = quantity,
            };

            this.viewModel.CartShopItems.Add(cartShopItem);
            return cartShopItem;
        }

        [Test]
        public void Constructor_WhenCartHasItems_LoadsItemsIntoCollection()
        {
            var cartItem = new CartItem(1, this.testShopItem, 3);
            var cart = new Cart(1, this.testClient, new Dictionary<int, CartItem> { { 1, cartItem } });
            this.cartService.GetCartById(1).Returns(cart);

            var vm = new CartViewModel(this.cartService, this.reservationService, this.session);

            Assert.That(vm.CartShopItems, Has.Count.EqualTo(1));
            Assert.That(vm.CartShopItems[0].Quantity, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_WhenActiveReservationExists_SetsIsReservedTrue()
        {
            var reservation = new Reservation(this.testCart, true, DateTime.Now);
            this.reservationService.GetAllReservations().Returns(new List<Reservation> { reservation });

            var vm = new CartViewModel(this.cartService, this.reservationService, this.session);

            Assert.That(vm.IsReserved, Is.True);
        }

        [Test]
        public void ChangeQuantity_CallsServiceWithCorrectParameters()
        {
            var cartShopItem = AddItemToViewModel(1, 2);

            this.viewModel.ChangeQuantity(cartShopItem, 5);

            this.cartService.Received(1).UpdateItemQuantity(1, 1, 5);
        }

        [Test]
        public void ChangeQuantity_UpdatesItemQuantityInCollection()
        {
            var cartShopItem = AddItemToViewModel(1, 2);

            this.viewModel.ChangeQuantity(cartShopItem, 5);

            Assert.That(cartShopItem.Quantity, Is.EqualTo(5));
        }

        [Test]
        public void ChangeQuantity_UpdatesOverallTotal()
        {
            var cartShopItem = AddItemToViewModel(1, 2);
            this.cartService.GetCartTotal(1).Returns(25.0);

            this.viewModel.ChangeQuantity(cartShopItem, 5);

            Assert.That(this.viewModel.OverallTotal, Is.EqualTo("$25.00"));
        }

        [Test]
        public void RemoveShopItem_CallsServiceWithCorrectParameters()
        {
            var cartShopItem = AddItemToViewModel(1, 2);

            this.viewModel.RemoveShopItem(cartShopItem);

            this.cartService.Received(1).RemoveItemFromCart(1, 1);
        }

        [Test]
        public void RemoveShopItem_RemovesItemFromCollection()
        {
            var cartShopItem = AddItemToViewModel(1, 2);

            this.viewModel.RemoveShopItem(cartShopItem);

            Assert.That(this.viewModel.CartShopItems, Does.Not.Contain(cartShopItem));
        }

        [Test]
        public void EmptyCart_CallsServiceClearCart()
        {
            AddItemToViewModel(1, 2);

            this.viewModel.EmptyCart();

            this.cartService.Received(1).ClearCart(1);
        }

        [Test]
        public void EmptyCart_ClearsCollection()
        {
            AddItemToViewModel(1, 2);

            this.viewModel.EmptyCart();

            Assert.That(this.viewModel.CartShopItems, Is.Empty);
        }

        [Test]
        public void ReserveCart_CallsReservationService()
        {
            this.viewModel.ReserveCart();

            this.reservationService.Received(1).ReserveCart(Arg.Any<Reservation>());
        }

        [Test]
        public void ReserveCart_SetsIsReservedToTrue()
        {
            this.viewModel.ReserveCart();

            Assert.That(this.viewModel.IsReserved, Is.True);
        }

        [Test]
        public void CancelReservation_CallsReservationService()
        {
            this.viewModel.ReserveCart();

            this.viewModel.CancelReservation();

            this.reservationService.Received(1).CancelReservation(Arg.Any<int>());
        }

        [Test]
        public void CancelReservation_SetsIsReservedToFalse()
        {
            this.viewModel.ReserveCart();

            this.viewModel.CancelReservation();

            Assert.That(this.viewModel.IsReserved, Is.False);
        }

        [Test]
        public void DecreaseQuantity_CallsServiceDecreaseItemQuantity()
        {
            var cartShopItem = AddItemToViewModel(1, 3);

            this.viewModel.DecreaseQuantity(cartShopItem);

            this.cartService.Received(1).DecreaseItemQuantity(1, 1);
        }

        [Test]
        public void Reload_RefreshesCartItemsFromService()
        {
            Assert.That(this.viewModel.CartShopItems, Is.Empty);

            var cartItem = new CartItem(1, this.testShopItem, 4);
            this.testCart.CartItems[1] = cartItem;

            this.viewModel.Reload();

            Assert.That(this.viewModel.CartShopItems, Has.Count.EqualTo(1));
            Assert.That(this.viewModel.CartShopItems[0].Quantity, Is.EqualTo(4));
        }

        [Test]
        public void IsLastItem_WhenServiceReturnsTrue_ReturnsTrue()
        {
            var cartShopItem = AddItemToViewModel(1, 1);
            this.cartService.IsLastCartItem(1, 1).Returns(true);

            var result = this.viewModel.IsLastItem(cartShopItem);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsLastItem_WhenServiceReturnsFalse_ReturnsFalse()
        {
            var cartShopItem = AddItemToViewModel(1, 3);
            this.cartService.IsLastCartItem(1, 1).Returns(false);

            var result = this.viewModel.IsLastItem(cartShopItem);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CancelReservationNullCart_DoesNotThrow()
        {
            this.cartService.GetCartById(1).Returns((Cart)null!);
            Assert.DoesNotThrow(() => this.viewModel.CancelReservation());
        }
    }
}