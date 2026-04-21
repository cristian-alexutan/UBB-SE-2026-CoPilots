using System;
using System.Linq;
using Content.Domain;
using Content.Repository;
using Content.Service;

namespace Tests;

public class ManagerServiceTests
{
    [Test]
    public void GetAllManagersTest()
    {
        var repo = new ManagerMemoryRepo();
        repo.Add(new Manager(1, "Alice", "alice@mail.com", "0700000001"));
        repo.Add(new Manager(2, "Bob", "bob@mail.com", "0700000002"));
        var service = new ManagerService(repo);

        var result = service.GetAllManagers().ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(m => m.Name), Is.EquivalentTo(new[] { "Alice", "Bob" }));
    }

    [Test]
    public void GetManagerByIdTest()
    {
        var repo = new ManagerMemoryRepo();
        repo.Add(new Manager(1, "Alice", "alice@mail.com", "0700000001"));
        var service = new ManagerService(repo);

        var result = service.GetManagerById(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void GetManagerById_NonExistentId_ReturnsNull()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        var result = service.GetManagerById(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void AddManagerSuccessfulTest()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);
        var manager = new Manager(1, "Alice", "alice@mail.com", "0700000001");

        service.AddManager(manager);

        var managers = service.GetAllManagers().ToList();
        Assert.That(managers, Has.Count.EqualTo(1));
        Assert.That(managers[0].Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void AddManagerUnsuccessfulTest_NullManager()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        Assert.Throws<ArgumentNullException>(() => service.AddManager(null!));

        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerUnsuccessfulTest_EmptyName()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(1, "", "alice@mail.com", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerUnsuccessfulTest_WhiteSpaceName()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(1, "   ", "alice@mail.com", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerUnsuccessfulTest_EmptyEmail()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(1, "Alice", "", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Email is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerUnsuccessfulTest_WhiteSpaceEmail()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(1, "Alice", "   ", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Email is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerUnsuccessfulTest_EmptyPhone()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(1, "Alice", "alice@mail.com", "")));

        Assert.That(ex!.Message, Does.Contain("Phone is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerUnsuccessfulTest_WhiteSpacePhone()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(1, "Alice", "alice@mail.com", "   ")));

        Assert.That(ex!.Message, Does.Contain("Phone is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void DeleteManagerTest()
    {
        var repo = new ManagerMemoryRepo();
        repo.Add(new Manager(1, "Alice", "alice@mail.com", "0700000001"));
        var service = new ManagerService(repo);

        service.DeleteManager(1);

        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void DeleteManager_OnlyRemovesTargetManager()
    {
        var repo = new ManagerMemoryRepo();
        repo.Add(new Manager(1, "Alice", "alice@mail.com", "0700000001"));
        repo.Add(new Manager(2, "Bob", "bob@mail.com", "0700000002"));
        var service = new ManagerService(repo);

        service.DeleteManager(1);

        var remaining = service.GetAllManagers().ToList();
        Assert.That(remaining, Has.Count.EqualTo(1));
        Assert.That(remaining[0].Name, Is.EqualTo("Bob"));
    }

    [Test]
    public void UpdateManagerSuccessfulTest()
    {
        var repo = new ManagerMemoryRepo();
        repo.Add(new Manager(1, "Alice", "alice@mail.com", "0700000001"));
        var service = new ManagerService(repo);

        service.UpdateManager(new Manager(1, "Alice Updated", "new@mail.com", "0700000099"));

        var updated = service.GetManagerById(1);
        Assert.That(updated.Name, Is.EqualTo("Alice Updated"));
        Assert.That(updated.Email, Is.EqualTo("new@mail.com"));
        Assert.That(updated.Phone, Is.EqualTo("0700000099"));
    }

    [Test]
    public void GetAnyManager_WhenEmpty_ReturnsNull()
    {
        var repo = new ManagerMemoryRepo();
        var service = new ManagerService(repo);

        var result = service.GetAnyManager();

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetAnyManager_WhenManagersExist_ReturnsAManager()
    {
        var repo = new ManagerMemoryRepo();
        repo.Add(new Manager(1, "Alice", "alice@mail.com", "0700000001"));
        repo.Add(new Manager(2, "Bob", "bob@mail.com", "0700000002"));
        var service = new ManagerService(repo);

        var result = service.GetAnyManager();

        Assert.That(result, Is.Not.Null);
    }
}