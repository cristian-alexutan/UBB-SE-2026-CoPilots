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
    public class CartDbRepoTests
    {
        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=DutyFreeShops_Test;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        private CartDbRepo _repo = null!;
        private IClientRepo _clientRepo = null!;
        private IShopItemRepo _shopItemRepo = null!;

        private List<int> _createdCartIds = null!;
        private List<int> _createdClientIds = null!;
        private List<int> _createdShopItemIds = null!;

        private Client _testClient = null!;
        private ShopItem _testShopItem = null!;
        private Cart _testCart = null!;

        [SetUp]
        public void Setup()
        {
            _clientRepo = new ClientDbRepo(ConnectionString);
            _shopItemRepo = new ShopItemDbRepo(ConnectionString);
            _repo = new CartDbRepo(ConnectionString, _clientRepo, _shopItemRepo);

            _createdCartIds = new List<int>();
            _createdClientIds = new List<int>();
            _createdShopItemIds = new List<int>();

            var uniqueName = "Test Client " + Guid.NewGuid();
            _testClient = new Client(0, uniqueName);
            _clientRepo.Add(_testClient);
            var persisted = _clientRepo.GetAll().First(c => c.Name == uniqueName);
            _testClient.Id = persisted.Id;
            _createdClientIds.Add(_testClient.Id);

            _testShopItem = new ShopItem(0, 50, 9.99f, 1, "test.jpg", "Test Item " + Guid.NewGuid(), "desc");
            _shopItemRepo.Add(_testShopItem);
            _createdShopItemIds.Add(_testShopItem.Id);

            _testCart = new Cart(0, _testClient, new Dictionary<int, CartItem>());
            _repo.Add(_testCart);
            _createdCartIds.Add(_testCart.Id);
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var id in _createdCartIds)
            {
                try { _repo.ClearCart(id); } catch { }
                try { _repo.Delete(id); } catch { }
            }
            foreach (var id in _createdShopItemIds)
            {
                try { _shopItemRepo.Delete(id); } catch { }
            }
            foreach (var id in _createdClientIds)
            {
                try { _clientRepo.Delete(id); } catch { }
            }
        }

        [Test]
        public void Add_ValidCart_AssignsIdAndPersists()
        {
            Assert.That(_testCart.Id, Is.Not.EqualTo(0), "Id should be set after Add");
            var fetched = _repo.GetById(_testCart.Id);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched!.Id, Is.EqualTo(_testCart.Id));
        }

        [Test]
        public void GetById_NonExistentId_ReturnsNull()
        {
            var result = _repo.GetById(-1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAll_ReturnsAddedCart()
        {
            var all = _repo.GetAll().ToList();
            Assert.That(all.Any(c => c.Id == _testCart.Id), Is.True);
        }

        [Test]
        public void Delete_ExistingCart_RemovesFromDatabase()
        {
            var cart = new Cart(0, _testClient, new Dictionary<int, CartItem>());
            _repo.Add(cart);

            _repo.Delete(cart.Id);

            Assert.That(_repo.GetById(cart.Id), Is.Null);
        }

        [Test]
        public void Delete_NonExistentId_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _repo.Delete(-1));
        }

        [Test]
        public void AddItemToCart_ValidItem_AssignsIdAndPersists()
        {
            var cartItem = new CartItem(0, _testShopItem, 2);

            _repo.AddItemToCart(_testCart.Id, cartItem);

            Assert.That(cartItem.Id, Is.Not.EqualTo(0), "CartItem Id should be assigned after insert");
            var fetched = _repo.GetById(_testCart.Id);
            Assert.That(fetched!.CartItems.ContainsKey(cartItem.Id), Is.True);
            Assert.That(fetched.CartItems[cartItem.Id].Quantity, Is.EqualTo(2));
        }

        [Test]
        public void RemoveItemFromCart_ExistingItem_ItemNoLongerInCart()
        {
            var cartItem = new CartItem(0, _testShopItem, 1);
            _repo.AddItemToCart(_testCart.Id, cartItem);

            _repo.RemoveItemFromCart(_testCart.Id, cartItem.Id);

            var fetched = _repo.GetById(_testCart.Id);
            Assert.That(fetched!.CartItems.ContainsKey(cartItem.Id), Is.False);
        }

        [Test]
        public void UpdateItemQuantity_ChangesQuantityInDatabase()
        {
            var cartItem = new CartItem(0, _testShopItem, 1);
            _repo.AddItemToCart(_testCart.Id, cartItem);

            _repo.UpdateItemQuantity(_testCart.Id, cartItem.Id, 5);

            var fetched = _repo.GetById(_testCart.Id);
            Assert.That(fetched!.CartItems[cartItem.Id].Quantity, Is.EqualTo(5));
        }

        [Test]
        public void ClearCart_RemovesAllItemsFromCart()
        {
            _repo.AddItemToCart(_testCart.Id, new CartItem(0, _testShopItem, 1));
            _repo.AddItemToCart(_testCart.Id, new CartItem(0, _testShopItem, 2));

            _repo.ClearCart(_testCart.Id);

            var fetched = _repo.GetById(_testCart.Id);
            Assert.That(fetched!.CartItems, Is.Empty);
        }
    }
}