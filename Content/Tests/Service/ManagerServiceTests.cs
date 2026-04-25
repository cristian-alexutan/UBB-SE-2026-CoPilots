using Content.Domain;
using Content.Repository.Interface;
using Content.Service;
using NSubstitute;

namespace Tests;

public class ManagerServiceTests
{
    private IManagerRepo managerRepo = null!;
    private ManagerService managerService = null!;
    [SetUp]
    public void Setup()
    {
        this.managerRepo = Substitute.For<IManagerRepo>();
        this.managerService = new ManagerService(this.managerRepo);
    }

    [Test]
    public void AddManager_NullManager_ThrowsException()
    {
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(null));
        Assert.That(exception!.Message, Does.Contain("Manager must not be null"));
    }

    [Test]
    public void AddManager_NullName_ThrowsException()
    {
        Manager manager = new Manager(1, null, "test@test.com", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddManager_EmptyStringName_ThrowsException()
    {
        Manager manager = new Manager(1, string.Empty, "test@test.com", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddManager_WhitespaceName_ThrowsException()
    {
        Manager manager = new Manager(1, " ", "test@test.com", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddManager_NullEmail_ThrowsException()
    {
        Manager manager = new Manager(1, "name", null, "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Email field must not be empty"));
    }

    [Test]
    public void AddManager_EmptyStringEmail_ThrowsException()
    {
        Manager manager = new Manager(1, "name", string.Empty, "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Email field must not be empty"));
    }

    [Test]
    public void AddManager_WhitespaceEmail_ThrowsException()
    {
        Manager manager = new Manager(1, "name", " ", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Email field must not be empty"));
    }

    [Test]
    public void AddManager_InvalidEmail_ThrowsException()
    {
        Manager manager = new Manager(1, "name", "test", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Email field must be valid"));
    }

    [Test]
    public void AddManager_NullPhoneNumber_ThrowsException()
    {
        Manager manager = new Manager(1, "name", "test@test.com", null);
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Phone number field must not be empty"));
    }

    [Test]
    public void AddManager_EmptyStringPhoneNumber_ThrowsException()
    {
        Manager manager = new Manager(1, "name", "test@test.com", string.Empty);
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Phone number field must not be empty"));
    }

    [Test]
    public void AddManager_WhitespacePhoneNumber_ThrowsException()
    {
        Manager manager = new Manager(1, "name", "test@test.com", " ");
        var exception = Assert.Catch<Exception>(() => this.managerService.AddManager(manager));
        Assert.That(exception!.Message, Does.Contain("Phone number field must not be empty"));
    }

    [Test]
    public void AddManager_ValidManager_ManagerAddedToRepo()
    {
        Manager manager = new Manager(1, "name", "test@test.com", "0700000000");
        this.managerService.AddManager(manager);
        this.managerRepo.Received(1).Add(manager);
    }

    [Test]
    public void DeleteManager_ValidId_ManagerDeletedFromRepo()
    {
        int managerId = 1;
        this.managerService.DeleteManager(managerId);
        this.managerRepo.Received(1).Delete(managerId);
    }

    [Test]
    public void UpdateManager_NullManager_ThrowsException()
    {
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(null));
        Assert.That(exception!.Message, Does.Contain("Manager must not be null"));
    }

    [Test]
    public void UpdateManager_NullName_ThrowsException()
    {
        Manager manager = new Manager(1, null, "test@test.com", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void UpdateManager_EmptyStringName_ThrowsException()
    {
        Manager manager = new Manager(1, string.Empty, "test@test.com", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void UpdateManager_WhitespaceName_ThrowsException()
    {
        Manager manager = new Manager(1, " ", "test@test.com", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void UpdateManager_NullEmail_ThrowsException()
    {
        Manager manager = new Manager(1, "name", null, "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Email field must not be empty"));
    }

    [Test]
    public void UpdateManager_EmptyStringEmail_ThrowsException()
    {
        Manager manager = new Manager(1, "name", string.Empty, "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Email field must not be empty"));
    }

    [Test]
    public void UpdateManager_WhitespaceEmail_ThrowsException()
    {
        Manager manager = new Manager(1, "name", " ", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Email field must not be empty"));
    }

    [Test]
    public void UpdateManager_InvalidEmail_ThrowsException()
    {
        Manager manager = new Manager(1, "name", "test", "0700000000");
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Email field must be valid"));
    }

    [Test]
    public void UpdateManager_NullPhoneNumber_ThrowsException()
    {
        Manager manager = new Manager(1, "name", "test@test.com", null);
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Phone number field must not be empty"));
    }

    [Test]
    public void UpdateManager_EmptyStringPhoneNumber_ThrowsException()
    {
        Manager manager = new Manager(1, "name", "test@test.com", string.Empty);
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Phone number field must not be empty"));
    }

    [Test]
    public void UpdateManager_WhitespacePhoneNumber_ThrowsException()
    {
        Manager manager = new Manager(1, "name", "test@test.com", " ");
        var exception = Assert.Catch<Exception>(() => this.managerService.UpdateManager(manager));
        Assert.That(exception!.Message, Does.Contain("Phone number field must not be empty"));
    }

    [Test]
    public void UpdateManager_ValidManager_ManagerUpdatedInRepo()
    {
        Manager manager = new Manager(1, "name", "test@test.com", "0700000000");
        this.managerService.UpdateManager(manager);
        this.managerRepo.Received(1).Update(manager);
    }
}
