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
    public class ShopPageViewModelTests
    {
        private IShopService shopService = null!;
        private ITicketService ticketService = null!;
        private UserSession session = null!;

        [SetUp]
        public void Setup()
        {
            shopService = Substitute.For<IShopService>();
            ticketService = Substitute.For<ITicketService>();
            session = new UserSession();

            shopService.GetAllAvailableShops().Returns(new List<Shop>());
        }

        private ShopPageViewModel CreateVm() =>
            new ShopPageViewModel(shopService, ticketService, session);

        [Test]
        public void Ctor_NullShopService_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ShopPageViewModel(null!, ticketService, session));
        }

        [Test]
        public void Ctor_NullTicketService_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ShopPageViewModel(shopService, null!, session));
        }

        [Test]
        public void Ctor_NullSession_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ShopPageViewModel(shopService, ticketService, null!));
        }

        [Test]
        public void Ctor_LoadsShopsFromService()
        {
            var shops = new List<Shop>
            {
                new Shop(1, "A", "t", 5),
                new Shop(2, "B", "t", 5),
            };
            shopService.GetAllAvailableShops().Returns(shops);

            var vm = CreateVm();

            Assert.That(vm.Shops.Count, Is.EqualTo(2));
            Assert.That(vm.Shops[0].Name, Is.EqualTo("A"));
            Assert.That(vm.Shops[1].Name, Is.EqualTo("B"));
        }

        [Test]
        public void AdminSession_ExposesAdminFlags()
        {
            session.SetAdmin(7);
            var vm = CreateVm();

            Assert.That(vm.IsAdmin, Is.True);
            Assert.That(vm.CanAddShop, Is.True);
            Assert.That(vm.IsCartEnabled, Is.False);
            Assert.That(vm.CartOpacity, Is.EqualTo(0.4));
        }

        [Test]
        public void ClientSession_ExposesClientFlags()
        {
            session.SetClient(3);
            var vm = CreateVm();

            Assert.That(vm.IsAdmin, Is.False);
            Assert.That(vm.CanAddShop, Is.False);
            Assert.That(vm.IsCartEnabled, Is.True);
            Assert.That(vm.CartOpacity, Is.EqualTo(1.0));
        }

        [Test]
        public void AddShop_EmptyName_ThrowsAndDoesNotCallService()
        {
            var vm = CreateVm();
            shopService.ClearReceivedCalls();

            Assert.Throws<ArgumentException>(() => vm.AddShop(string.Empty, "type"));
            shopService.DidNotReceive().AddShop(Arg.Any<Shop>());
        }

        [Test]
        public void AddShop_EmptyType_ThrowsAndDoesNotCallService()
        {
            var vm = CreateVm();
            shopService.ClearReceivedCalls();

            Assert.Throws<ArgumentException>(() => vm.AddShop("name", "  "));
            shopService.DidNotReceive().AddShop(Arg.Any<Shop>());
        }

        [Test]
        public void AddShop_ValidInput_CallsServiceAndReloads()
        {
            session.SetAdmin(42);
            var vm = CreateVm();
            shopService.ClearReceivedCalls();
            shopService.GetAllAvailableShops().Returns(new List<Shop>
            {
                new Shop(1, "new", "type", 42),
            });

            vm.AddShop("new", "type");

            shopService.Received(1).AddShop(Arg.Is<Shop>(s =>
                s.Name == "new" && s.Type == "type" && s.ManagerId == 42));
            shopService.Received().GetAllAvailableShops();
            Assert.That(vm.Shops.Count, Is.EqualTo(1));
        }

        [Test]
        public void EditShop_NullShop_Throws()
        {
            var vm = CreateVm();
            Assert.Throws<ArgumentNullException>(
                () => vm.EditShop(null!, "n", "t"));
        }

        [Test]
        public void EditShop_EmptyName_Throws()
        {
            var vm = CreateVm();
            var shop = new Shop(1, "old", "oldT", 1);
            Assert.Throws<ArgumentException>(
                () => vm.EditShop(shop, string.Empty, "t"));
        }

        [Test]
        public void EditShop_EmptyType_Throws()
        {
            var vm = CreateVm();
            var shop = new Shop(1, "old", "oldT", 1);
            Assert.Throws<ArgumentException>(
                () => vm.EditShop(shop, "n", string.Empty));
        }

        [Test]
        public void EditShop_ValidInput_CallsUpdateShop()
        {
            session.SetAdmin(10);
            var vm = CreateVm();
            shopService.ClearReceivedCalls();

            var existing = new Shop(5, "old", "oldT", 10);
            vm.EditShop(existing, "newName", "newType");

            shopService.Received(1).UpdateShop(Arg.Is<Shop>(s =>
                s.Id == 5 &&
                s.Name == "newName" &&
                s.Type == "newType" &&
                s.ManagerId == 10));
        }

        [Test]
        public void DeleteShop_Null_Throws()
        {
            var vm = CreateVm();
            Assert.Throws<ArgumentNullException>(
                () => vm.DeleteShop(null!));
        }

        [Test]
        public void DeleteShop_Valid_CallsServiceWithId()
        {
            var vm = CreateVm();
            shopService.ClearReceivedCalls();

            vm.DeleteShop(new Shop(9, "x", "y", 1));

            shopService.Received(1).DeleteShop(9);
        }

        [Test]
        public void Search_EmptyQuery_ReloadsAllShops()
        {
            var vm = CreateVm();
            shopService.ClearReceivedCalls();

            vm.Search("   ");

            shopService.Received().GetAllAvailableShops();
            shopService.DidNotReceive().SearchByName(Arg.Any<string>());
        }

        [Test]
        public void Search_WithQuery_DelegatesToSearchByName()
        {
            var vm = CreateVm();
            var matches = new List<Shop> { new Shop(1, "match", "t", 1) };
            shopService.SearchByName("ma").Returns(matches);

            vm.Search("ma");

            shopService.Received(1).SearchByName("ma");
            Assert.That(vm.Shops.Count, Is.EqualTo(1));
            Assert.That(vm.Shops[0].Name, Is.EqualTo("match"));
        }

        [Test]
        public void SortByReviews_OrdersShopsByAscendingTicketCount()
        {
            var shops = new List<Shop>
            {
                new Shop(1, "ShopA", "t", 1),
                new Shop(2, "ShopB", "t", 1),
                new Shop(3, "ShopC", "t", 1),
            };
            shopService.GetAllAvailableShops().Returns(shops);
            ticketService.CountTicketsBySubcategory("ShopA").Returns(5);
            ticketService.CountTicketsBySubcategory("ShopB").Returns(1);
            ticketService.CountTicketsBySubcategory("ShopC").Returns(3);

            var vm = CreateVm();
            vm.SortByReviews();

            Assert.That(vm.Shops[0].Name, Is.EqualTo("ShopB"));
            Assert.That(vm.Shops[1].Name, Is.EqualTo("ShopC"));
            Assert.That(vm.Shops[2].Name, Is.EqualTo("ShopA"));
        }

        [Test]
        public void SortAlphabetically_DelegatesToShopService()
        {
            var vm = CreateVm();
            var sorted = new List<Shop> { new Shop(1, "A", "t", 1) };
            shopService
                .SortAlphabetically(Arg.Any<IEnumerable<Shop>>())
                .Returns(sorted);

            vm.SortAlphabetically();

            shopService.Received(1).SortAlphabetically(Arg.Any<IEnumerable<Shop>>());
            Assert.That(vm.Shops.Count, Is.EqualTo(1));
            Assert.That(vm.Shops[0].Name, Is.EqualTo("A"));
        }
    }
}
