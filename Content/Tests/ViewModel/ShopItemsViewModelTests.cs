using System;
using System.Linq;
using Content.Domain;
using Content.Repository;
using Content.Service;
using Content.User;
using Content.ViewModel;
using NUnit.Framework;

namespace Tests.ViewModel
{
    [TestFixture]
    public class ShopItemsViewModelTests
    {
        private ShopItemMockRepo shopItemRepo;
        private ShopItemService shopItemService;
        private CartMockRepo cartRepo;
        private CartService cartService;
        private UserSession session;
        private Shop shop;

        [SetUp]
        public void SetUp()
        {
            this.shopItemRepo = new ShopItemMockRepo();
            this.shopItemService = new ShopItemService(this.shopItemRepo);
            this.cartRepo = new CartMockRepo();
            this.cartService = new CartService(this.cartRepo, this.shopItemService);
            this.session = new UserSession();
            this.shop = new Shop(1, "shop1", "type1", 1);
        }

        private ShopItemsViewModel CreateAdminViewModel(int adminId = 1)
        {
            this.session.SetAdmin(adminId);
            return new ShopItemsViewModel(this.shopItemService, this.cartService, this.session, this.shop);
        }

        private ShopItemsViewModel CreateClientViewModel(int clientId = 1)
        {
            this.session.SetClient(clientId);
            return new ShopItemsViewModel(this.shopItemService, this.cartService, this.session, this.shop);
        }

        [Test]
        public void IsAdminTestAdminSession()
        {
            ShopItemsViewModel viewModel = this.CreateAdminViewModel(5);
            Assert.That(viewModel.IsAdmin, Is.True);
        }

        [Test]
        public void CanAddItemTestClientSession()
        {
            ShopItemsViewModel viewModel = this.CreateClientViewModel(3);

            Assert.That(viewModel.CanAddItem, Is.False);
        }

        [Test]
        public void CanAddItemTestAdminSession()
        {
            ShopItemsViewModel viewModel = this.CreateAdminViewModel(5);

            Assert.That(viewModel.CanAddItem, Is.True);
        }

        [Test]
        public void IsCartEnabledTestClientSession()
        {
            ShopItemsViewModel viewModel = this.CreateClientViewModel(3);

            Assert.That(viewModel.IsCartEnabled, Is.True);
        }

        [Test]
        public void IsCartEnabledTestAdminSession()
        {
            ShopItemsViewModel viewModel = this.CreateAdminViewModel(5);

            Assert.That(viewModel.IsCartEnabled, Is.False);
        }

        [Test]
        public void LoadItemsTestOnlyCurrentShopItemsLoaded()
        {
            this.shopItemRepo.Add(new ShopItem(1, 10f, 1, "img1.png", "item1", "desc1"));
            this.shopItemRepo.Add(new ShopItem(1, 20f, 2, "img2.png", "item2", "desc2"));
            ShopItemsViewModel viewModel = this.CreateClientViewModel();

            viewModel.LoadItems();

            Assert.That(viewModel.Items.Count, Is.EqualTo(1));
            Assert.That(viewModel.Items[0].Name, Is.EqualTo("item1"));
        }

        [Test]
        public void AddItemTestEmptyNameThrowsArgumentException()
        {
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();
            Assert.Throws<ArgumentException>(
                () => viewModel.AddItem(" ", "desc1", "10", "5", "img1.png"));
        }

        [Test]
        public void AddItemTestInvalidPriceThrowsArgumentException()
        {
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();
            Assert.Throws<ArgumentException>(
                () => viewModel.AddItem("item1", "desc1", "notanumber", "5", "img1.png"));
        }

        [Test]
        public void AddItemTestInvalidQuantityThrowsArgumentException()
        {
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();

            Assert.Throws<ArgumentException>(
                () => viewModel.AddItem("item1", "desc1", "10", "notanumber", "img1.png"));
        }

        [Test]
        public void AddItemTestValidData()
        {
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();

            viewModel.AddItem("item1", "desc1", "25", "10", "img1.png");

            Assert.That(viewModel.Items.Count, Is.EqualTo(1));
            Assert.That(viewModel.Items[0].Name, Is.EqualTo("item1"));
        }

        [Test]
        public void UpdateItemTestEmptyNameThrowsArgumentException()
        {
            ShopItem existingItem = new ShopItem(5, 10f, 1, string.Empty, "item1", "desc1");
            this.shopItemRepo.Add(existingItem);
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();

            Assert.Throws<ArgumentException>(
                () => viewModel.UpdateItem(existingItem, string.Empty, "desc2", "10", "5", "img2.png"));
        }

        [Test]
        public void UpdateItemTestInvalidPriceThrowsArgumentException()
        {
            ShopItem existingItem = new ShopItem(5, 10f, 1, string.Empty, "item1", "desc1");
            this.shopItemRepo.Add(existingItem);
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();

            Assert.Throws<ArgumentException>(
                () => viewModel.UpdateItem(existingItem, "item2", "desc2", "notanumber", "5", "img2.png"));
        }

        [Test]
        public void UpdateItemTestInvalidQuantityThrowsArgumentException()
        {
            ShopItem existingItem = new ShopItem(5, 10f, 1, string.Empty, "item1", "desc1");
            this.shopItemRepo.Add(existingItem);
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();

            Assert.Throws<ArgumentException>(
                () => viewModel.UpdateItem(existingItem, "item2", "desc2", "20", "notanumber", "img2.png"));
        }

        [Test]
        public void UpdateItemTestValidData()
        {
            ShopItem existingItem = new ShopItem(5, 10f, 1, string.Empty, "item1", "desc1");
            this.shopItemRepo.Add(existingItem);
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();

            viewModel.UpdateItem(existingItem, "item2", "desc2", "50", "3", "img2.png");

            Assert.That(viewModel.Items.Count, Is.EqualTo(1));
            Assert.That(viewModel.Items[0].Name, Is.EqualTo("item2"));
            Assert.That(viewModel.Items[0].Price, Is.EqualTo(50f));
        }

        [Test]
        public void DeleteItemTestSuccessful()
        {
            ShopItem existingItem = new ShopItem(5, 10f, 1, string.Empty, "item1", "desc1");
            this.shopItemRepo.Add(existingItem);
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();

            viewModel.DeleteItem(existingItem);

            Assert.That(viewModel.Items, Is.Empty);
        }

        [Test]
        public void AddToCartTestSufficientStockAddsItem()
        {
            ShopItem shopItem = new ShopItem(10, 15f, 1, string.Empty, "item1", "desc1");
            this.shopItemRepo.Add(shopItem);
            ShopItemsViewModel viewModel = this.CreateClientViewModel(2);

            viewModel.AddToCart(viewModel.Items[0], 3);

            Cart cart = this.cartService.GetCartById(2);
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.CartItems.Values.Any(ci => ci.ShopItem.Id == shopItem.Id && ci.Quantity == 3), Is.True);
        }

        [Test]
        public void AddToCartTestInsufficientStockThrowsInvalidOperationException()
        {
            ShopItem shopItem = new ShopItem(2, 15f, 1, string.Empty, "item1", "desc1");
            this.shopItemRepo.Add(shopItem);
            ShopItemsViewModel viewModel = this.CreateClientViewModel(2);

            Assert.Throws<InvalidOperationException>(
                () => viewModel.AddToCart(viewModel.Items[0], 10));
        }

        [Test]
        public void SortByNameTest()
        {
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item3", "desc3"));
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item1", "desc1"));
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item2", "desc2"));
            ShopItemsViewModel viewModel = this.CreateClientViewModel();

            viewModel.SortByName();

            Assert.That(viewModel.Items[0].Name, Is.EqualTo("item1"));
            Assert.That(viewModel.Items[1].Name, Is.EqualTo("item2"));
            Assert.That(viewModel.Items[2].Name, Is.EqualTo("item3"));
        }

        [Test]
        public void SortByPriceTest()
        {
            this.shopItemRepo.Add(new ShopItem(3, 30f, 1, string.Empty, "item3", "desc3"));
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item1", "desc1"));
            this.shopItemRepo.Add(new ShopItem(3, 15f, 1, string.Empty, "item2", "desc2"));
            ShopItemsViewModel viewModel = this.CreateClientViewModel();

            viewModel.SortByPrice();

            Assert.That(viewModel.Items[0].Name, Is.EqualTo("item1"));
            Assert.That(viewModel.Items[1].Name, Is.EqualTo("item2"));
            Assert.That(viewModel.Items[2].Name, Is.EqualTo("item3"));
        }

        [Test]
        public void SearchTest()
        {
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item1", "desc1"));
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item11", "desc11"));
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item2", "desc2"));
            ShopItemsViewModel viewModel = this.CreateClientViewModel();

            viewModel.Search("item1");

            Assert.That(viewModel.Items.Count, Is.EqualTo(2));
            Assert.That(viewModel.Items.All(i => i.Name.Contains("item1", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        [Test]
        public void SearchTestNoMatchingQueryReturnsEmpty()
        {
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item1", "desc1"));
            this.shopItemRepo.Add(new ShopItem(3, 5f, 1, string.Empty, "item2", "desc2"));
            ShopItemsViewModel viewModel = this.CreateClientViewModel();

            viewModel.Search("item9");

            Assert.That(viewModel.Items, Is.Empty);
        }

        [Test]
        public void DeleteItemNull()
        {
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();
            Assert.Throws<ArgumentNullException>(
                () => viewModel.DeleteItem(null!));
        }

        [Test]
        public void EditItemNull()
        {
            ShopItem existingItem = new ShopItem(5, 10f, 1, string.Empty, "item1", "desc1");
            this.shopItemRepo.Add(existingItem);
            ShopItemsViewModel viewModel = this.CreateAdminViewModel();
            Assert.Throws<ArgumentNullException>(
                () => viewModel.UpdateItem(null!, "item2", "desc2", "20", "3", "img2.png"));
        }

        [Test]
        public void SearchEmpty()
        {
            ShopItemsViewModel viewModel = this.CreateClientViewModel();
            int countAll = viewModel.Items.Count;
            viewModel.Search(string.Empty);
            Assert.That(viewModel.Items.Count, Is.EqualTo(countAll));
        }

        [Test]
        public void CreateViewModelNullShopItemService()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ShopItemsViewModel(null!, this.cartService, this.session, this.shop));
        }

        [Test]
        public void CreateViewModelNullCartService()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ShopItemsViewModel(this.shopItemService, null!, this.session, this.shop));
        }

        [Test]
        public void CreateViewModelNullSession()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ShopItemsViewModel(this.shopItemService, this.cartService, null!, this.shop));
        }

        [Test]
        public void CreateViewModelNullShop()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ShopItemsViewModel(this.shopItemService, this.cartService, this.session, null!));
        }
    }
}
