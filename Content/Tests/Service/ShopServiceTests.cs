using Content.Domain;
using Content.Repository;
using Content.Service;

namespace Tests;

public class ShopServiceTests
{
    [Test]
    public void GetAllAvailableShopsTest()
    {
        var repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "Alpha", "Type", 1));
        repo.Add(new Shop(2, "Beta", "Type", 2));
        var service = new ShopService(repo);
        var result = service.GetAllAvailableShops().ToList();
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(s => s.Name), Is.EquivalentTo(new[] { "Alpha", "Beta" }));
    }

    [Test]
    public void AddShopSuccessfulTest()
    {
        var repo = new ShopMemoryRepo();
        var service = new ShopService(repo);
        var shop = new Shop(1, "New Shop", "Type", 1);

        service.AddShop(shop);

        var shops = service.GetAllAvailableShops().ToList();
        Assert.That(shops, Has.Count.EqualTo(1));
        Assert.That(shops[0].Name, Is.EqualTo("New Shop"));
    }

    [Test]
    public void AddShopUnsuccessfulTest()
    {
        var repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "Existing", "Type", 1));
        var service = new ShopService(repo);

        var ex = Assert.Throws<Exception>(() => service.AddShop(new Shop(2, "Existing", "Type", 2)));

        Assert.That(ex!.Message, Is.EqualTo("Shop name already exists"));
        Assert.That(service.GetAllAvailableShops().Count(), Is.EqualTo(1));
    }

    [Test]
    public void DeleteShopTest()
    {
        var repo = new ShopMemoryRepo();
        repo.Add(new Shop(7, "ToDelete", "Type", 1));
        var service = new ShopService(repo);

        service.DeleteShop(7);

        Assert.That(service.GetAllAvailableShops(), Is.Empty);
    }

    [Test]
    public void SortAlphabeticallyTest()
    {
        var repo = new ShopMemoryRepo();
        var service = new ShopService(repo);
        var shops = new List<Shop>
        {
            new(1, "Zulu", "Type", 1),
            new(2, "Alpha", "Type", 1),
            new(3, "Mike", "Type", 1),
        };

        var result = service.SortAlphabetically(shops).ToList();

        Assert.That(result.Select(s => s.Name), Is.EqualTo(new[] { "Alpha", "Mike", "Zulu" }));
    }

    [Test]
    public void UpdateShopUnsuccessfulTest_EmptyName()
    {
        var repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "Original", "Type", 1));
        var service = new ShopService(repo);

        var ex = Assert.Throws<Exception>(() => service.UpdateShop(new Shop(1, "   ", "Type", 1)));

        Assert.That(ex!.Message, Is.EqualTo("Name field must not be empty"));
        Assert.That(service.GetAllAvailableShops().Single(s => s.Id == 1).Name, Is.EqualTo("Original"));
    }

    [Test]
    public void UpdateShopUnsuccessfulTest_DuplicateName()
    {
        var repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "A", "Type", 1));
        repo.Add(new Shop(2, "B", "Type", 2));
        var service = new ShopService(repo);

        var ex = Assert.Throws<Exception>(() => service.UpdateShop(new Shop(2, "A", "Type", 2)));

        Assert.That(ex!.Message, Is.EqualTo("Shop with given name already exists"));
        Assert.That(service.GetAllAvailableShops().Single(s => s.Id == 2).Name, Is.EqualTo("B"));
    }

    [Test]
    public void UpdateShopSuccessfulTest()
    {
        var repo = new ShopMemoryRepo();
        repo.Add(new Shop(1, "Original", "Type", 1));
        var service = new ShopService(repo);
        var updated = new Shop(1, "Updated", "Type", 1);

        service.UpdateShop(updated);

        Assert.That(service.GetAllAvailableShops().Single(s => s.Id == 1).Name, Is.EqualTo("Updated"));
    }
}
