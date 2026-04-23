using System;
using System.Linq;
using Content.Domain;
using Content.Repository;
using Content.Service;

namespace Tests;

public class ManagerServiceTests
{
    [Test]
    public void GetAllManagersTestSuccessful()
    {
        var repo = new ManagerMockRepo();
        var manager1 = new Manager(0, "Alice", "alice@mail.com", "0700000001");
        var manager2 = new Manager(0, "Bob", "bob@mail.com", "0700000002");
        repo.Add(manager1);
        repo.Add(manager2);
        var service = new ManagerService(repo);

        var result = service.GetAllManagers().ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(m => m.Name), Is.EquivalentTo(new[] { "Alice", "Bob" }));
    }

    [Test]
    public void GetManagerByIdTestSuccessful()
    {
        var repo = new ManagerMockRepo();
        var manager = new Manager(0, "Alice", "alice@mail.com", "0700000001");
        repo.Add(manager);
        var service = new ManagerService(repo);

        var result = service.GetManagerById(manager.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(manager.Id));
        Assert.That(result.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void GetManagerByIdUnsuccessfull_IdDoesntExist()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var result = service.GetManagerById(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void AddManagerTestSuccessful()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);
        var manager = new Manager(0, "Alice", "alice@mail.com", "0700000001");

        service.AddManager(manager);

        var managers = service.GetAllManagers().ToList();
        Assert.That(managers, Has.Count.EqualTo(1));
        Assert.That(managers[0].Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void AddManagerTestUnsuccessful_ManagerIsNull()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        Assert.Throws<ArgumentNullException>(() => service.AddManager(null!));

        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerTestUnsuccessful_NameIsEmpty()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(0, string.Empty, "alice@mail.com", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerTestUnsuccessful_NameIsWhiteSpace()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(0, "   ", "alice@mail.com", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerTestUnsuccessful_EmailIsEmpty()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(0, "Alice", string.Empty, "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Email is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerTestUnsuccessful_EmailIsWhiteSpace()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(0, "Alice", "   ", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Email is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerTestUnsuccessful_EmailIsInvalid()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(0, "Alice", "notanemail", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Email is invalid"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerTestUnsuccessful_PhoneNumberIsEmpty()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(0, "Alice", "alice@mail.com", string.Empty)));

        Assert.That(ex!.Message, Does.Contain("Phone number is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void AddManagerTestUnsuccessful_PhoneNumberIsWhiteSpace()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddManager(new Manager(0, "Alice", "alice@mail.com", "   ")));

        Assert.That(ex!.Message, Does.Contain("Phone is required"));
        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void DeleteManagerTestSuccessful()
    {
        var repo = new ManagerMockRepo();
        var manager = new Manager(0, "Alice", "alice@mail.com", "0700000001");
        repo.Add(manager);
        var service = new ManagerService(repo);

        service.DeleteManager(manager.Id);

        Assert.That(service.GetAllManagers(), Is.Empty);
    }

    [Test]
    public void DeleteManagerTestUnsuccessful()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        Manager? result = service.DeleteManager(-1);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void UpdateManagerTestSuccessful()
    {
        var repo = new ManagerMockRepo();
        var manager = new Manager(0, "Alice", "alice@mail.com", "0700000001");
        repo.Add(manager);
        var service = new ManagerService(repo);

        service.UpdateManager(new Manager(manager.Id, "Alice Updated", "new@mail.com", "0700000099"));

        var updated = service.GetManagerById(manager.Id);
        Assert.That(updated.Name, Is.EqualTo("Alice Updated"));
        Assert.That(updated.Email, Is.EqualTo("new@mail.com"));
        Assert.That(updated.Phone, Is.EqualTo("0700000099"));
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_IdDoesntExist()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        Manager? result = service.UpdateManager(new Manager(-1, "Alice Updated", "new@mail.com", "0700000099"));

        Assert.That(result, Is.Null);
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_ManagerIsNull()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        Assert.Throws<ArgumentNullException>(() => service.UpdateManager(null!));
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_NameIsEmpty()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateManager(new Manager(0, string.Empty, "alice@mail.com", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_EmailIsInvalid()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateManager(new Manager(0, "Alice", "notanemail", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Email is invalid"));
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_PhoneNumberIsEmpty()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateManager(new Manager(0, "Alice", "alice@mail.com", string.Empty)));

        Assert.That(ex!.Message, Does.Contain("Phone number is required"));
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_EmailIsEmpty()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateManager(new Manager(0, "Alice", string.Empty, "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Email is required"));
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_EmailIsWhiteSpace()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateManager(new Manager(0, "Alice", "   ", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Email is required"));
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_NameIsWhiteSpace()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateManager(new Manager(0, "   ", "alice@mail.com", "0700000001")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
    }

    [Test]
    public void UpdateManagerTestUnsuccessful_PhoneNumberIsWhiteSpace()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateManager(new Manager(0, "Alice", "alice@mail.com", "   ")));

        Assert.That(ex!.Message, Does.Contain("Phone is required"));
    }

    [Test]
    public void GetAnyManagerTestSuccessful()
    {
        var repo = new ManagerMockRepo();
        var manager = new Manager(0, "Alice", "alice@mail.com", "0700000001");
        repo.Add(manager);
        var service = new ManagerService(repo);

        var result = service.GetAnyManager();

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetAnyManagerTestUnsuccessful_ManagerIsNull()
    {
        var repo = new ManagerMockRepo();
        var service = new ManagerService(repo);

        var result = service.GetAnyManager();

        Assert.That(result, Is.Null);
    }
}