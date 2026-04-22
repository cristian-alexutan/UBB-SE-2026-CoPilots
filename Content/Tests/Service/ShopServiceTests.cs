using Content.Domain;
using Content.Repository;
using Content.Service;

namespace Tests;

public class ShopServiceTests
{
    [Test]
    public void AddShopSuccessfulTest()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        ShopService service = new ShopService(repo);
        Shop shop = new Shop(1, "New Shop", "Type", 1);
        service.AddShop(shop);
        List<Shop> shops = service.GetAllAvailableShops().ToList();
        Assert.That(shops, Has.Count.EqualTo(1));
        Assert.That(shops[0].Name, Is.EqualTo("New Shop"));
    }

    [Test]
    public void AddShopUnsuccessfulTest_DuplicateName()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "Existing", "Type", 1));
        ShopService service = new ShopService(repo);
        var ex = Assert.Catch<Exception>(() => service.AddShop(new Shop(2, "Existing", "Type", 2)));
        Assert.That(ex!.Message, Is.EqualTo("Shop name already exists"));
        Assert.That(service.GetAllAvailableShops().Count(), Is.EqualTo(1));
    }

    [Test]
    public void AddShopUnsuccessfulTest_EmptyName()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        ShopService service = new ShopService(repo);
        var ex = Assert.Catch<Exception>(() => service.AddShop(new Shop(1, "   ", "Type", 1)));
        Assert.That(ex!.Message, Is.EqualTo("Name field must not be empty"));
        Assert.That(service.GetAllAvailableShops(), Is.Empty);
    }

    [Test]
    public void DeleteShopTest()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        repo.Add(new Shop(7, "ToDelete", "Type", 1));
        ShopService service = new ShopService(repo);
        service.DeleteShop(7);
        Assert.That(service.GetAllAvailableShops(), Is.Empty);
    }

    [Test]
    public void SortAlphabeticallyTest()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        ShopService service = new ShopService(repo);
        List<Shop> shops = new List<Shop>
        {
            new (1, "B", "Type", 1),
            new (2, "A", "Type", 1),
            new (3, "C", "Type", 1),
        };
        List<Shop> result = service.SortAlphabetically(shops).ToList();
        Assert.That(result.Select(s => s.Name), Is.EqualTo(new[] { "A", "B", "C" }));
    }

    [Test]
    public void UpdateShopUnsuccessfulTest_EmptyName()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "Original", "Type", 1));
        ShopService service = new ShopService(repo);
        var ex = Assert.Catch<Exception>(() => service.UpdateShop(new Shop(1, "   ", "Type", 1)));
        Assert.That(ex!.Message, Is.EqualTo("Name field must not be empty"));
        Assert.That(service.GetAllAvailableShops().Single(s => s.Id == 1).Name, Is.EqualTo("Original"));
    }

    [Test]
    public void UpdateShopUnsuccessfulTest_DuplicateName()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "A", "Type", 1));
        repo.Add(new Shop(2, "B", "Type", 2));
        ShopService service = new ShopService(repo);
        var ex = Assert.Catch<Exception>(() => service.UpdateShop(new Shop(2, "A", "Type", 2)));
        Assert.That(ex!.Message, Is.EqualTo("Shop with given name already exists"));
        Assert.That(service.GetAllAvailableShops().Single(s => s.Id == 2).Name, Is.EqualTo("B"));
    }

    [Test]
    public void UpdateShopSuccessfulTest()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "Original", "Type", 1));
        ShopService service = new ShopService(repo);
        Shop updated = new Shop(1, "Updated", "Type", 1);
        service.UpdateShop(updated);
        Assert.That(service.GetAllAvailableShops().Single(s => s.Id == 1).Name, Is.EqualTo("Updated"));
    }

    [Test]
    public void SearchByNameTest()
    {
        ShopMemoryRepo repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "Alpha", "Type", 1));
        repo.Add(new Shop(2, "Bet", "Type", 2));
        repo.Add(new Shop(3, "Gamma", "Type", 3));
        ShopService service = new ShopService(repo);
        List<Shop> result = service.SearchByName("a").ToList();
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(s => s.Name), Is.EquivalentTo(new[] { "Alpha", "Gamma" }));
    }
}
