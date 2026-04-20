using System;
using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository;
using Content.Service;

namespace Tests.Service
{
    [TestFixture]
    public class ReservationServiceTests
    {
        private ReservationMemoryRepo _reservationRepo = null!;
        private ShopItemMemoryRepo _shopItemRepo = null!;
        private CartMemoryRepo _cartRepo = null!;

        private ShopItemService _shopItemService = null!;
        private CartService _cartService = null!;
        private ReservationService _service = null!;

        private Client _client = null!;

        [SetUp]
        public void Setup()
        {
            _reservationRepo = new ReservationMemoryRepo();
            _shopItemRepo = new ShopItemMemoryRepo();
            _cartRepo = new CartMemoryRepo();

            _shopItemService = new ShopItemService(_shopItemRepo);
            _cartService = new CartService(_cartRepo);
            _service = new ReservationService(_reservationRepo, _shopItemService, _cartService);

            _client = new Client(1, "Test Client");
        }

        private ShopItem AddShopItem(string name, int quantity, float price = 10f)
        {
            var item = new ShopItem(0, quantity, price, 1, "photo.jpg", name, "desc");
            _shopItemRepo.Add(item);
            return item;
        }

        private Cart BuildCart(int cartId, params (ShopItem item, int qty)[] lines)
        {
            var cartItems = new Dictionary<int, CartItem>();
            int nextCartItemId = 1;
            foreach (var (item, qty) in lines)
            {
                var ci = new CartItem(nextCartItemId, item, qty);
                cartItems[ci.Id] = ci;
                nextCartItemId++;
            }

            var cart = new Cart(cartId, _client, cartItems);
            _cartRepo.Add(cart);
            return cart;
        }

        [Test]
        public void GetAllReservations_ReturnsEveryAddedReservation()
        {
            var itemA = AddShopItem("A", 10);
            var itemB = AddShopItem("B", 10);

            var cart1 = BuildCart(1, (itemA, 1));
            var cart2 = BuildCart(2, (itemB, 2));

            _service.ReserveCart(new Reservation(cart1, true, DateTime.Now));
            _service.ReserveCart(new Reservation(cart2, true, DateTime.Now));

            var all = _service.GetAllReservations().ToList();

            Assert.That(all, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetReservationById_ReturnsTheMatchingReservation()
        {
            var item = AddShopItem("A", 10);
            var cart = BuildCart(1, (item, 2));
            var reservation = new Reservation(cart, true, DateTime.Now);

            _service.ReserveCart(reservation);

            var fetched = _service.GetReservationById(reservation.Id);

            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched.Id, Is.EqualTo(reservation.Id));
            Assert.That(fetched.ReservationCart.Id, Is.EqualTo(cart.Id));
        }

        [Test]
        public void ReserveCart_HappyPath_DecrementsStockAndPersists()
        {
            var item = AddShopItem("Whisky", 10);
            var cart = BuildCart(1, (item, 3));
            var reservation = new Reservation(cart, true, DateTime.Now);

            _service.ReserveCart(reservation);

          
            Assert.That(reservation.Id, Is.Not.EqualTo(0), "Id should be assigned by repo");
            Assert.That(_shopItemService.GetById(item.Id).Quantity, Is.EqualTo(7));
            Assert.That(_service.GetAllReservations().Count(), Is.EqualTo(1));
           
        }

        [Test]
        public void ReserveCart_ThrowsWhenStockIsInsufficient()
        {
            var item = AddShopItem("Perfume", 2);
            var cart = BuildCart(1, (item, 5));
            var reservation = new Reservation(cart, true, DateTime.Now);

            var ex = Assert.Throws<Exception>(() => _service.ReserveCart(reservation));

            Assert.That(ex!.Message, Does.Contain("Not enough stock"));
        }

        [Test]
        public void ReserveCart_AllOrNothing_DoesNotModifyStockIfOneLineFails()
        {
            var itemA = AddShopItem("A", 10);
            var itemB = AddShopItem("B", 1);
            var cart = BuildCart(1, (itemA, 3), (itemB, 5));
            var reservation = new Reservation(cart, true, DateTime.Now);

            Assert.Throws<Exception>(() => _service.ReserveCart(reservation));

                        
            Assert.That(_shopItemService.GetById(itemA.Id).Quantity, Is.EqualTo(10),
                "Stock for item A must not be touched if the whole reservation fails");
            Assert.That(_shopItemService.GetById(itemB.Id).Quantity, Is.EqualTo(1));
            Assert.That(_service.GetAllReservations(), Is.Empty);
            
        }

        [Test]
        public void DeleteReservation_RemovesReservationFromRepo()
        {
            var item = AddShopItem("A", 10);
            var cart = BuildCart(1, (item, 1));
            var reservation = new Reservation(cart, true, DateTime.Now);
            _service.ReserveCart(reservation);

            _service.DeleteReservation(reservation.Id);

            Assert.That(_service.GetAllReservations(), Is.Empty);
        }

        [Test]
        public void CancelReservation_WhenActive_RestoresStockClearsCartAndDeactivates()
        {
            var item = AddShopItem("A", 10);
            var cart = BuildCart(1, (item, 4));
            var reservation = new Reservation(cart, true, DateTime.Now);
            _service.ReserveCart(reservation);

            Assume.That(_shopItemService.GetById(item.Id).Quantity, Is.EqualTo(6));

            _service.cancelReservation(reservation.Id);

         
            Assert.That(_shopItemService.GetById(item.Id).Quantity, Is.EqualTo(10),
                "Stock should be restored after cancellation");
            Assert.That(_cartRepo.GetById(cart.Id).CartItems, Is.Empty,
                "Cart should be cleared after cancellation");
            Assert.That(_service.GetReservationById(reservation.Id).Active, Is.False,
                "Reservation should be marked inactive");
           
        }

        [Test]
        public void CancelReservation_WhenAlreadyInactive_DoesNothing()
        {
            var item = AddShopItem("A", 10);
            var cart = BuildCart(1, (item, 3));

            var reservation = new Reservation(cart, false, DateTime.Now);
            _reservationRepo.Add(reservation);

            _service.cancelReservation(reservation.Id);


            Assert.That(_shopItemService.GetById(item.Id).Quantity, Is.EqualTo(10),
                "Stock must not change for an already-inactive reservation");
            Assert.That(_cartRepo.GetById(cart.Id).CartItems, Has.Count.EqualTo(1),
                "Cart must not be cleared for an already-inactive reservation");
            Assert.That(_service.GetReservationById(reservation.Id).Active, Is.False);
            
        }
    }
}
