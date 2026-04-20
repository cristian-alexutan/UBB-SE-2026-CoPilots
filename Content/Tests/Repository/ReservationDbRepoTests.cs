using System;
using System.Collections.Generic;
using System.Linq;
using Content.Domain;
using Content.Repository.Database;
using Content.Repository.Interface;
using NUnit.Framework;

namespace Tests.Repository
{
    [TestFixture]
    public class ReservationDbRepoTests
    {
        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        private ReservationDbRepo _repo = null!;
        private ICartRepo _cartRepo = null!;
        private IClientRepo _clientRepo = null!;
        private IShopItemRepo _shopItemRepo = null!;

        private List<int> _createdReservationIds = null!;
        private List<int> _createdCartIds = null!;
        private List<int> _createdClientIds = null!;

        private Cart _testCart = null!;
        private Client _testClient = null!;

        [SetUp]
        public void Setup()
        {
            _clientRepo = new ClientDbRepo(ConnectionString);
            _shopItemRepo = new ShopItemDbRepo(ConnectionString);
            _cartRepo = new CartDbRepo(ConnectionString, _clientRepo, _shopItemRepo);
            _repo = new ReservationDbRepo(ConnectionString, _cartRepo);

            _createdReservationIds = new List<int>();
            _createdCartIds = new List<int>();
            _createdClientIds = new List<int>();


            var uniqueName = "Test Client " + Guid.NewGuid();
            _testClient = new Client(0, uniqueName);
            _clientRepo.Add(_testClient);

            var persistedClient = _clientRepo.GetAll().First(c => c.Name == uniqueName);
            _testClient.Id = persistedClient.Id;
            _createdClientIds.Add(_testClient.Id);

            _testCart = new Cart(0, _testClient, new Dictionary<int, CartItem>());
            _cartRepo.Add(_testCart);
            _createdCartIds.Add(_testCart.Id);
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var id in _createdReservationIds)
            {
                try { _repo.Delete(id); } catch { }
            }
            foreach (var id in _createdCartIds)
            {
                try { _cartRepo.Delete(id); } catch { }
            }
            foreach (var id in _createdClientIds)
            {
                try { _clientRepo.Delete(id); } catch { }
            }
        }

        [Test]
        public void Add_ValidReservation_AssignsIdAndPersists()
        {
            var reservation = new Reservation(_testCart, true, DateTime.Now);

            _repo.Add(reservation);
            _createdReservationIds.Add(reservation.Id);

            Assert.That(reservation.Id, Is.Not.EqualTo(0), "Id should be set after Add");
            var fetched = _repo.GetById(reservation.Id);
            Assert.That(fetched, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(fetched!.Id, Is.EqualTo(reservation.Id));
                Assert.That(fetched.Active, Is.True);
                Assert.That(fetched.ReservationCart.Id, Is.EqualTo(_testCart.Id));
            });
        }

        [Test]
        public void GetById_NonExistentId_ReturnsNull()
        {
            var result = _repo.GetById(-1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAll_ReturnsAddedReservation()
        {
            var reservation = new Reservation(_testCart, true, DateTime.Now);
            _repo.Add(reservation);
            _createdReservationIds.Add(reservation.Id);

            var all = _repo.GetAll().ToList();

            Assert.That(all.Any(r => r.Id == reservation.Id), Is.True);
        }

        [Test]
        public void Update_ChangesActiveStatus()
        {
            var reservation = new Reservation(_testCart, true, DateTime.Now);
            _repo.Add(reservation);
            _createdReservationIds.Add(reservation.Id);

            reservation.Active = false;
            _repo.Update(reservation);

            var fetched = _repo.GetById(reservation.Id);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched!.Active, Is.False);
        }

        [Test]
        public void Delete_ExistingReservation_RemovesFromDatabase()
        {
            var reservation = new Reservation(_testCart, true, DateTime.Now);
            _repo.Add(reservation);

            _repo.Delete(reservation.Id);

            Assert.That(_repo.GetById(reservation.Id), Is.Null);
        }

        [Test]
        public void Delete_NonExistentId_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _repo.Delete(-1));
        }
    }
}
