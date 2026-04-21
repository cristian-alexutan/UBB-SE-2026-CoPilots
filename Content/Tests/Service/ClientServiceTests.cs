using System;
using System.Linq;
using Content.Domain;
using Content.Repository;
using Content.Service;

namespace Tests;

public class ClientServiceTests
{
    [Test]
    public void GetAllClientsTest()
    {
        var repo = new ClientMemoryRepo();
        repo.Add(new Client(1, "Alice"));
        repo.Add(new Client(2, "Bob"));
        var service = new ClientService(repo);

        var result = service.GetAllClients().ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Alice", "Bob" }));
    }

    [Test]
    public void GetClientByIdTest()
    {
        var repo = new ClientMemoryRepo();
        repo.Add(new Client(1, "Alice"));
        var service = new ClientService(repo);

        var result = service.GetClientById(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void GetClientById_NonExistentId_ReturnsNull()
    {
        var repo = new ClientMemoryRepo();
        var service = new ClientService(repo);

        var result = service.GetClientById(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void AddClientSuccessfulTest()
    {
        var repo = new ClientMemoryRepo();
        var service = new ClientService(repo);
        var client = new Client(1, "Alice");

        service.AddClient(client);

        var clients = service.GetAllClients().ToList();
        Assert.That(clients, Has.Count.EqualTo(1));
        Assert.That(clients[0].Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void AddClientUnsuccessfulTest_NullClient()
    {
        var repo = new ClientMemoryRepo();
        var service = new ClientService(repo);

        Assert.Throws<ArgumentNullException>(() => service.AddClient(null!));

        Assert.That(service.GetAllClients(), Is.Empty);
    }

    [Test]
    public void AddClientUnsuccessfulTest_EmptyName()
    {
        var repo = new ClientMemoryRepo();
        var service = new ClientService(repo);

        var ex = Assert.Throws<ArgumentException>(() => service.AddClient(new Client(1, "")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
        Assert.That(service.GetAllClients(), Is.Empty);
    }

    [Test]
    public void AddClientUnsuccessfulTest_WhiteSpaceName()
    {
        var repo = new ClientMemoryRepo();
        var service = new ClientService(repo);

        var ex = Assert.Throws<ArgumentException>(() => service.AddClient(new Client(1, "   ")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
        Assert.That(service.GetAllClients(), Is.Empty);
    }

    [Test]
    public void DeleteClientTest()
    {
        var repo = new ClientMemoryRepo();
        repo.Add(new Client(1, "Alice"));
        var service = new ClientService(repo);

        service.DeleteClient(1);

        Assert.That(service.GetAllClients(), Is.Empty);
    }

    [Test]
    public void DeleteClient_OnlyRemovesTargetClient()
    {
        var repo = new ClientMemoryRepo();
        repo.Add(new Client(1, "Alice"));
        repo.Add(new Client(2, "Bob"));
        var service = new ClientService(repo);

        service.DeleteClient(1);

        var remaining = service.GetAllClients().ToList();
        Assert.That(remaining, Has.Count.EqualTo(1));
        Assert.That(remaining[0].Name, Is.EqualTo("Bob"));
    }

    [Test]
    public void UpdateClientSuccessfulTest()
    {
        var repo = new ClientMemoryRepo();
        repo.Add(new Client(1, "Alice"));
        var service = new ClientService(repo);

        service.UpdateClient(new Client(1, "Alice Updated"));

        Assert.That(service.GetClientById(1).Name, Is.EqualTo("Alice Updated"));
    }

    [Test]
    public void GetAnyClient_WhenEmpty_ReturnsNull()
    {
        var repo = new ClientMemoryRepo();
        var service = new ClientService(repo);

        var result = service.GetAnyClient();

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetAnyClient_WhenClientsExist_ReturnsAClient()
    {
        var repo = new ClientMemoryRepo();
        repo.Add(new Client(1, "Alice"));
        repo.Add(new Client(2, "Bob"));
        var service = new ClientService(repo);

        var result = service.GetAnyClient();

        Assert.That(result, Is.Not.Null);
    }
}