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

        private CartDbRepo repo = null!;
        private IClientRepo clientRepo = null!;
        private IShopItemRepo shopItemRepo = null!;

        private List<int> createdCartIds = null!;
        private List<int> createdClientIds = null!;
        private List<int> createdShopItemIds = null!;

        private Client testClient = null!;
        private ShopItem testShopItem = null!;
        private Cart testCart = null!;

        [SetUp]
        public void Setup()
        {
            clientRepo = new ClientDbRepo(ConnectionString);
            shopItemRepo = new ShopItemDbRepo(ConnectionString);
            repo = new CartDbRepo(ConnectionString, clientRepo, shopItemRepo);

            createdCartIds = new List<int>();
            createdClientIds = new List<int>();
            createdShopItemIds = new List<int>();

            var uniqueName = "Test Client " + Guid.NewGuid();
            testClient = new Client(0, uniqueName);
            clientRepo.Add(testClient);
            var persisted = clientRepo.GetAll().First(c => c.Name == uniqueName);
            testClient.Id = persisted.Id;
            createdClientIds.Add(testClient.Id);

            testShopItem = new ShopItem(0, 50, 9.99f, 1, "test.jpg", "Test Item " + Guid.NewGuid(), "desc");
            shopItemRepo.Add(testShopItem);
            createdShopItemIds.Add(testShopItem.Id);

            testCart = new Cart(0, testClient, new Dictionary<int, CartItem>());
            repo.Add(testCart);
            createdCartIds.Add(testCart.Id);
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var id in createdCartIds)
            {
                try
                {
                    repo.ClearCart(id);
                }
                catch
                {
                }
                try
                {
                    repo.Delete(id);
                }
                catch
                {
                }
            }
            foreach (var id in createdShopItemIds)
            {
                try
                {
                    shopItemRepo.Delete(id);
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
        public void Add_ValidCart_AssignsIdAndPersists()
        {
            Assert.That(testCart.Id, Is.Not.EqualTo(0), "Id should be set after Add");
            var fetched = repo.GetById(testCart.Id);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched!.Id, Is.EqualTo(testCart.Id));
        }

        [Test]
        public void GetById_NonExistentId_ReturnsNull()
        {
            var result = repo.GetById(-1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAll_ReturnsAddedCart()
        {
            var all = repo.GetAll().ToList();
            Assert.That(all.Any(c => c.Id == testCart.Id), Is.True);
        }

        [Test]
        public void Delete_ExistingCart_RemovesFromDatabase()
        {
            var cart = new Cart(0, testClient, new Dictionary<int, CartItem>());
            repo.Add(cart);

            repo.Delete(cart.Id);

            Assert.That(repo.GetById(cart.Id), Is.Null);
        }

        [Test]
        public void Delete_NonExistentId_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => repo.Delete(-1));
        }

        [Test]
        public void AddItemToCart_ValidItem_AssignsIdAndPersists()
        {
            var cartItem = new CartItem(0, testShopItem, 2);

            repo.AddItemToCart(testCart.Id, cartItem);

            Assert.That(cartItem.Id, Is.Not.EqualTo(0), "CartItem Id should be assigned after insert");
            var fetched = repo.GetById(testCart.Id);
            Assert.That(fetched!.CartItems.ContainsKey(cartItem.Id), Is.True);
            Assert.That(fetched.CartItems[cartItem.Id].Quantity, Is.EqualTo(2));
        }

        [Test]
        public void RemoveItemFromCart_ExistingItem_ItemNoLongerInCart()
        {
            var cartItem = new CartItem(0, testShopItem, 1);
            repo.AddItemToCart(testCart.Id, cartItem);

            repo.RemoveItemFromCart(testCart.Id, cartItem.Id);

            var fetched = repo.GetById(testCart.Id);
            Assert.That(fetched!.CartItems.ContainsKey(cartItem.Id), Is.False);
        }

        [Test]
        public void UpdateItemQuantity_ChangesQuantityInDatabase()
        {
            var cartItem = new CartItem(0, testShopItem, 1);
            repo.AddItemToCart(testCart.Id, cartItem);

            repo.UpdateItemQuantity(testCart.Id, cartItem.Id, 5);

            var fetched = repo.GetById(testCart.Id);
            Assert.That(fetched!.CartItems[cartItem.Id].Quantity, Is.EqualTo(5));
        }

        [Test]
        public void ClearCart_RemovesAllItemsFromCart()
        {
            repo.AddItemToCart(testCart.Id, new CartItem(0, testShopItem, 1));
            repo.AddItemToCart(testCart.Id, new CartItem(0, testShopItem, 2));

            repo.ClearCart(testCart.Id);

            var fetched = repo.GetById(testCart.Id);
            Assert.That(fetched!.CartItems, Is.Empty);
        }
    }
}