using Content.Data.Service.Interface;
using Content.Domain;
using Content.Repository.Interface;
using Content.Service;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class ReservationServiceTests
    {
        private IReservationRepo reservationRepo = null!;
        private IShopItemService shopItemService = null!;
        private ICartService cartService = null!;
        private ReservationService reservationService = null!;

        [SetUp]
        public void SetUp()
        {
            reservationRepo = Substitute.For<IReservationRepo>();
            shopItemService = Substitute.For<IShopItemService>();
            cartService = Substitute.For<ICartService>();
            reservationService = new ReservationService(reservationRepo, shopItemService, cartService);
        }

        private static Cart BuildCart(int cartId, params CartItem[] cartItems)
        {
            Dictionary<int, CartItem> cartItemsDictionary = new Dictionary<int, CartItem>();
            foreach (CartItem cartItem in cartItems)
            {
                cartItemsDictionary[cartItem.Id] = cartItem;
            }

            return new Cart(cartId, new Client(1, "TestClient"), cartItemsDictionary);
        }

        private static readonly Manager TestManager = new Manager(1, "Manager", "manager@test.com", "0700000000");
        private static readonly Shop TestShop = new Shop(1, "Test Shop", "Type", TestManager);

        private static ShopItem BuildShopItem(int id, int quantity)
        {
            return new ShopItem(id, quantity, 5.0f, TestShop, string.Empty, "Item" + id, string.Empty);
        }

        [Test]
        public void ReserveCart_InsufficientStock_ThrowsException()
        {
            ShopItem shopItem = BuildShopItem(1, 2);
            CartItem cartItem = new CartItem(1, shopItem, 5);
            Cart cart = BuildCart(10, cartItem);
            Reservation reservation = new Reservation(cart, true, DateTime.Now);
            shopItemService.GetById(1).Returns(BuildShopItem(1, 2));

            Assert.Throws<Exception>(() => reservationService.ReserveCart(reservation));
        }

        [Test]
        public void ReserveCart_InsufficientStock_DoesNotCallRepoAdd()
        {
            ShopItem shopItem = BuildShopItem(1, 2);
            CartItem cartItem = new CartItem(1, shopItem, 5);
            Cart cart = BuildCart(10, cartItem);
            Reservation reservation = new Reservation(cart, true, DateTime.Now);
            shopItemService.GetById(1).Returns(BuildShopItem(1, 2));

            try
            {
                reservationService.ReserveCart(reservation);
            }
            catch (Exception)
            {
            }

            reservationRepo.DidNotReceive().Add(Arg.Any<Reservation>());
        }

        [Test]
        public void ReserveCart_InsufficientStockOnSecondItem_DoesNotMutateFirstItem()
        {
            ShopItem cartShopItem1 = BuildShopItem(1, 5);
            ShopItem cartShopItem2 = BuildShopItem(2, 1);
            CartItem cartItem1 = new CartItem(1, cartShopItem1, 3);
            CartItem cartItem2 = new CartItem(2, cartShopItem2, 10);
            Cart cart = BuildCart(10, cartItem1, cartItem2);
            Reservation reservation = new Reservation(cart, true, DateTime.Now);
            shopItemService.GetById(1).Returns(BuildShopItem(1, 5));
            shopItemService.GetById(2).Returns(BuildShopItem(2, 1));

            try
            {
                reservationService.ReserveCart(reservation);
            }
            catch (Exception)
            {
            }

            shopItemService.DidNotReceive().UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void ReserveCart_SufficientStock_CallsRepoAdd()
        {
            ShopItem shopItem = BuildShopItem(1, 10);
            CartItem cartItem = new CartItem(1, shopItem, 3);
            Cart cart = BuildCart(10, cartItem);
            Reservation reservation = new Reservation(cart, true, DateTime.Now);
            shopItemService.GetById(1).Returns(BuildShopItem(1, 10));

            reservationService.ReserveCart(reservation);

            reservationRepo.Received().Add(reservation);
        }

        [Test]
        public void ReserveCart_SufficientStock_DecrementsShopItemQuantity()
        {
            ShopItem shopItem = BuildShopItem(1, 10);
            CartItem cartItem = new CartItem(1, shopItem, 3);
            Cart cart = BuildCart(10, cartItem);
            Reservation reservation = new Reservation(cart, true, DateTime.Now);
            ShopItem fetchedShopItem = BuildShopItem(1, 10);
            shopItemService.GetById(1).Returns(fetchedShopItem);

            reservationService.ReserveCart(reservation);

            Assert.That(fetchedShopItem.Quantity, Is.EqualTo(7));
        }

        [Test]
        public void ReserveCart_SufficientStock_CallsUpdateShopItemForEachCartItem()
        {
            ShopItem firstShopItem = BuildShopItem(1, 10);
            ShopItem secondShopItem = BuildShopItem(2, 10);
            CartItem firstCartItem = new CartItem(1, firstShopItem, 2);
            CartItem secondCartItem = new CartItem(2, secondShopItem, 4);
            Cart cart = BuildCart(10, firstCartItem, secondCartItem);
            Reservation reservation = new Reservation(cart, true, DateTime.Now);
            shopItemService.GetById(1).Returns(BuildShopItem(1, 10));
            shopItemService.GetById(2).Returns(BuildShopItem(2, 10));

            reservationService.ReserveCart(reservation);

            shopItemService.Received(2).UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void ReserveCart_EmptyCart_CallsRepoAddWithoutMutation()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(cart, true, DateTime.Now);

            reservationService.ReserveCart(reservation);

            reservationRepo.Received().Add(reservation);
            shopItemService.DidNotReceive().UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void CancelReservation_ReservationInactive_DoesNotCallRepoUpdate()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(1, cart, false, DateTime.Now);
            reservationRepo.GetById(1).Returns(reservation);

            reservationService.CancelReservation(1);

            reservationRepo.DidNotReceive().Update(Arg.Any<Reservation>());
        }

        [Test]
        public void CancelReservation_ReservationInactive_DoesNotRestockItems()
        {
            ShopItem shopItem = BuildShopItem(1, 5);
            CartItem cartItem = new CartItem(1, shopItem, 3);
            Cart cart = BuildCart(10, cartItem);
            Reservation reservation = new Reservation(1, cart, false, DateTime.Now);
            reservationRepo.GetById(1).Returns(reservation);

            reservationService.CancelReservation(1);

            shopItemService.DidNotReceive().UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void CancelReservation_ReservationInactive_DoesNotClearCart()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(1, cart, false, DateTime.Now);
            reservationRepo.GetById(1).Returns(reservation);

            reservationService.CancelReservation(1);

            cartService.DidNotReceive().ClearCart(Arg.Any<int>());
        }

        [Test]
        public void CancelReservation_ReservationActive_RestocksShopItems()
        {
            ShopItem shopItem = BuildShopItem(1, 2);
            CartItem cartItem = new CartItem(1, shopItem, 3);
            Cart cart = BuildCart(10, cartItem);
            Reservation reservation = new Reservation(1, cart, true, DateTime.Now);
            reservationRepo.GetById(1).Returns(reservation);
            ShopItem fetchedShopItem = BuildShopItem(1, 2);
            shopItemService.GetById(1).Returns(fetchedShopItem);

            reservationService.CancelReservation(1);

            Assert.That(fetchedShopItem.Quantity, Is.EqualTo(5));
        }

        [Test]
        public void CancelReservation_ReservationActive_CallsUpdateShopItemForEachCartItem()
        {
            ShopItem firstShopItem = BuildShopItem(1, 2);
            ShopItem secondShopItem = BuildShopItem(2, 4);
            CartItem firstCartItem = new CartItem(1, firstShopItem, 1);
            CartItem secondCartItem = new CartItem(2, secondShopItem, 2);
            Cart cart = BuildCart(10, firstCartItem, secondCartItem);
            Reservation reservation = new Reservation(1, cart, true, DateTime.Now);
            reservationRepo.GetById(1).Returns(reservation);
            shopItemService.GetById(1).Returns(BuildShopItem(1, 2));
            shopItemService.GetById(2).Returns(BuildShopItem(2, 4));

            reservationService.CancelReservation(1);

            shopItemService.Received(2).UpdateShopItem(Arg.Any<ShopItem>());
        }

        [Test]
        public void CancelReservation_ReservationActive_ClearsReservedCart()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(1, cart, true, DateTime.Now);
            reservationRepo.GetById(1).Returns(reservation);

            reservationService.CancelReservation(1);

            cartService.Received().ClearCart(10);
        }

        [Test]
        public void CancelReservation_ReservationActive_SetsReservationInactive()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(1, cart, true, DateTime.Now);
            reservationRepo.GetById(1).Returns(reservation);

            reservationService.CancelReservation(1);

            Assert.That(reservation.Active, Is.False);
        }

        [Test]
        public void CancelReservation_ReservationActive_CallsRepoUpdate()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(1, cart, true, DateTime.Now);
            reservationRepo.GetById(1).Returns(reservation);

            reservationService.CancelReservation(1);

            reservationRepo.Received().Update(reservation);
        }

        [Test]
        public void GetActiveReservationForCart_NoReservationsExist_ReturnsNull()
        {
            reservationRepo.GetAll().Returns(new List<Reservation>());

            Reservation result = reservationService.GetActiveReservationForCart(10);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetActiveReservationForCart_NoReservationMatchesCartId_ReturnsNull()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(1, cart, true, DateTime.Now);
            reservationRepo.GetAll().Returns(new List<Reservation> { reservation });

            Reservation result = reservationService.GetActiveReservationForCart(99);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetActiveReservationForCart_ReservationMatchesCartButInactive_ReturnsNull()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(1, cart, false, DateTime.Now);
            reservationRepo.GetAll().Returns(new List<Reservation> { reservation });

            Reservation result = reservationService.GetActiveReservationForCart(10);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetActiveReservationForCart_ActiveReservationForCart_ReturnsReservation()
        {
            Cart cart = BuildCart(10);
            Reservation reservation = new Reservation(1, cart, true, DateTime.Now);
            reservationRepo.GetAll().Returns(new List<Reservation> { reservation });

            Reservation result = reservationService.GetActiveReservationForCart(10);

            Assert.That(result, Is.EqualTo(reservation));
        }

        [Test]
        public void GetActiveReservationForCart_MultipleReservations_ReturnsActiveMatchingOne()
        {
            Cart cart1 = BuildCart(10);
            Cart cart2 = BuildCart(20);
            Reservation inactiveReservation = new Reservation(1, cart1, false, DateTime.Now);
            Reservation activeReservation = new Reservation(2, cart1, true, DateTime.Now);
            Reservation otherCartReservation = new Reservation(3, cart2, true, DateTime.Now);
            reservationRepo.GetAll().Returns(new List<Reservation> { inactiveReservation, activeReservation, otherCartReservation });

            Reservation result = reservationService.GetActiveReservationForCart(10);

            Assert.That(result, Is.EqualTo(activeReservation));
        }
    }
}
