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

        private ReservationDbRepo repo = null!;
        private ICartRepo cartRepo = null!;
        private IClientRepo clientRepo = null!;
        private IShopItemRepo shopItemRepo = null!;

        private List<int> createdReservationIds = null!;
        private List<int> createdCartIds = null!;
        private List<int> createdClientIds = null!;

        private Cart testCart = null!;
        private Client testClient = null!;

        [SetUp]
        public void Setup()
        {
            clientRepo = new ClientDbRepo(ConnectionString);
            shopItemRepo = new ShopItemDbRepo(ConnectionString);
            cartRepo = new CartDbRepo(ConnectionString, clientRepo, shopItemRepo);
            repo = new ReservationDbRepo(ConnectionString, cartRepo);

            createdReservationIds = new List<int>();
            createdCartIds = new List<int>();
            createdClientIds = new List<int>();

            var uniqueName = "Test Client " + Guid.NewGuid();
            testClient = new Client(0, uniqueName);
            clientRepo.Add(testClient);

            var persistedClient = clientRepo.GetAll().First(c => c.Name == uniqueName);
            testClient.Id = persistedClient.Id;
            createdClientIds.Add(testClient.Id);

            testCart = new Cart(0, testClient, new Dictionary<int, CartItem>());
            cartRepo.Add(testCart);
            createdCartIds.Add(testCart.Id);
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var id in createdReservationIds)
            {
                try
                {
                    repo.Delete(id);
                }
                catch
                {
                }
            }
            foreach (var id in createdCartIds)
            {
                try
                {
                    cartRepo.Delete(id);
                }
                catch
                {
                }
            }
            foreach (var id in createdClientIds)
            {
                try
                {
                    clientRepo.Delete(id);
                }
                catch
                {
                }
            }
        }

        [Test]
        public void Add_ValidReservation_AssignsIdAndPersists()
        {
            var reservation = new Reservation(testCart, true, DateTime.Now);

            repo.Add(reservation);
            createdReservationIds.Add(reservation.Id);

            Assert.That(reservation.Id, Is.Not.EqualTo(0), "Id should be set after Add");
            var fetched = repo.GetById(reservation.Id);
            Assert.That(fetched, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(fetched!.Id, Is.EqualTo(reservation.Id));
                Assert.That(fetched.Active, Is.True);
                Assert.That(fetched.ReservationCart.Id, Is.EqualTo(testCart.Id));
            });
        }

        [Test]
        public void GetById_NonExistentId_ReturnsNull()
        {
            var result = repo.GetById(-1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAll_ReturnsAddedReservation()
        {
            var reservation = new Reservation(testCart, true, DateTime.Now);
            repo.Add(reservation);
            createdReservationIds.Add(reservation.Id);

            var all = repo.GetAll().ToList();

            Assert.That(all.Any(r => r.Id == reservation.Id), Is.True);
        }

        [Test]
        public void Update_ChangesActiveStatus()
        {
            var reservation = new Reservation(testCart, true, DateTime.Now);
            repo.Add(reservation);
            createdReservationIds.Add(reservation.Id);

            reservation.Active = false;
            repo.Update(reservation);

            var fetched = repo.GetById(reservation.Id);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched!.Active, Is.False);
        }

        [Test]
        public void Delete_ExistingReservation_RemovesFromDatabase()
        {
            var reservation = new Reservation(testCart, true, DateTime.Now);
            repo.Add(reservation);

            repo.Delete(reservation.Id);

            Assert.That(repo.GetById(reservation.Id), Is.Null);
        }

        [Test]
        public void Delete_NonExistentId_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => repo.Delete(-1));
        }
    }
}
