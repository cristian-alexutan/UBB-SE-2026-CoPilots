using Content.Repository.Interface;
using Content.Service;
using Content.Domain;
using NSubstitute;

namespace Tests;

public class ShopServiceTests
{
    private IShopRepo shopRepo = null!;
    private ShopService shopService = null!;

    [SetUp]
    public void Setup()
    {
        this.shopRepo = Substitute.For<IShopRepo>();
        this.shopService = new ShopService(this.shopRepo);
    }

    [Test]
    public void AddShop_EmptyName_ThrowsException()
    {
        Shop shop = new Shop(" ", "none", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.AddShop(shop));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddShop_NullName_ThrowsException()
    {
        Shop shop = new Shop(null, "none", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.AddShop(shop));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddShop_EmptyStringName_ThrowsException()
    {
        Shop shop = new Shop(string.Empty, "none", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.AddShop(shop));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddShop_EmptyType_ThrowsException()
    {
        Shop shop = new Shop("Test", " ", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.AddShop(shop));
        Assert.That(exception!.Message, Does.Contain("Type field must not be empty"));
    }

    [Test]
    public void AddShop_NullType_ThrowsException()
    {
        Shop shop = new Shop("Test", null, new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.AddShop(shop));
        Assert.That(exception!.Message, Does.Contain("Type field must not be empty"));
    }
    [Test]
    public void AddShop_EmptyStringType_ThrowsException()
    {
        Shop shop = new Shop("Test", string.Empty, new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.AddShop(shop));
        Assert.That(exception!.Message, Does.Contain("Type field must not be empty"));
    }
    [Test]
    public void AddShop_DuplicateName_ThrowsException()
    {
        Shop shop = new Shop("Test", "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        this.shopRepo.GetAll().Returns(new List<Shop> { shop });
        var exception = Assert.Catch<Exception>(() => this.shopService.AddShop(shop));
        Assert.That(exception!.Message, Does.Contain("Shop name already exists"));
    }
    [Test]
    public void AddShop_ValidShop_ShopAddedToRepo()
    {
        Shop shop = new Shop("Test", "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        this.shopRepo.GetAll().Returns(new List<Shop>());
        this.shopService.AddShop(shop);
        this.shopRepo.Received(1).Add(shop);
    }
    [Test]
    public void DeleteShop_ValidId_ShopDeletedFromRepo()
    {
        int shopId = 1;
        this.shopService.DeleteShop(shopId);
        this.shopRepo.Received(1).Delete(shopId);
    }
    [Test]
    public void UpdateShop_EmptyName_ThrowsException()
    {
        Shop shop = new Shop(" ", "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.UpdateShop(shop));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }
    [Test]
    public void UpdateShop_NullName_ThrowsException()
    {
        Shop shop = new Shop(null, "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.UpdateShop(shop));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }
    [Test]
    public void UpdateShop_EmptyStringName_ThrowsException()
    {
        Shop shop = new Shop(string.Empty, "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.UpdateShop(shop));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }
    [Test]
    public void UpdateShop_EmptyType_ThrowsException()
    {
        Shop shop = new Shop("Test", " ", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.UpdateShop(shop));
        Assert.That(exception!.Message, Does.Contain("Type field must not be empty"));
    }
    [Test]
    public void UpdateShop_NullType_ThrowsException()
    {
        Shop shop = new Shop("Test", null, new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.UpdateShop(shop));
        Assert.That(exception!.Message, Does.Contain("Type field must not be empty"));
    }
    [Test]
    public void UpdateShop_EmptyStringType_ThrowsException()
    {
        Shop shop = new Shop("Test", string.Empty, new Manager(1, "Manager", "manager@test.com", "0700000000"));
        var exception = Assert.Catch<Exception>(() => this.shopService.UpdateShop(shop));
        Assert.That(exception!.Message, Does.Contain("Type field must not be empty"));
    }
    [Test]
    public void UpdateShop_DuplicateName_ThrowsException()
    {
        Shop shop1 = new Shop(1, "Test", "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        Shop shop2 = new Shop(2, "Test", "Type", new Manager(2, "Manager", "manager@test.com", "0700000000"));
        this.shopRepo.GetAll().Returns(new List<Shop> { shop1 });
        var exception = Assert.Catch<Exception>(() => this.shopService.UpdateShop(shop2));
        Assert.That(exception!.Message, Does.Contain("Shop with given name already exists"));
    }
    [Test]
    public void UpdateShop_ValidShop_ShopUpdatedInRepo()
    {
        Shop shop = new Shop("Test", "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        this.shopService.UpdateShop(shop);
        this.shopRepo.Received(1).Update(shop);
    }
    [Test]
    public void SortAlphabetically_ShopsSortedByName()
    {
        Shop shop1 = new Shop("B Shop", "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        Shop shop2 = new Shop("A Shop", "Type", new Manager(2, "Manager", "manager@test.com", "0700000000"));
        IEnumerable<Shop> shops = new List<Shop> { shop1, shop2 };
        var sortedShops = this.shopService.SortAlphabetically(shops).ToList();
        Assert.That(sortedShops[0].Name, Is.EqualTo("A Shop"));
        Assert.That(sortedShops[1].Name, Is.EqualTo("B Shop"));
    }
    [Test]
    public void SearchShopsByName_MatchingShopsReturned()
    {
        Shop shop1 = new Shop("Test Shop", "Type", new Manager(1, "Manager", "manager@test.com", "0700000000"));
        Shop shop2 = new Shop("Another Shop", "Type", new Manager(2, "Manager", "manager@test.com", "0700000000"));
        IEnumerable<Shop> shops = new List<Shop> { shop1, shop2 };
        this.shopRepo.GetAll().Returns(shops);
        var matchingShops = this.shopService.SearchByName("Test").ToList();
        Assert.That(matchingShops.Count, Is.EqualTo(1));
        Assert.That(matchingShops[0].Name, Is.EqualTo("Test Shop"));
    }
}
