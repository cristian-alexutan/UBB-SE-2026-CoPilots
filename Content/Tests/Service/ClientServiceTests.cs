using System;
using System.Linq;
using Content.Domain;
using Content.Repository;
using Content.Service;

namespace Tests;

public class ClientServiceTests
{
    [Test]
    public void GetAllClientsTestSuccessful()
    {
        var repo = new ClientMockRepo();
        var client1 = new Client(0, "Alice");
        var client2 = new Client(0, "Bob");
        repo.Add(client1);
        repo.Add(client2);
        var service = new ClientService(repo);

        var result = service.GetAllClients().ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(c => c.Name), Is.EquivalentTo(new[] { "Alice", "Bob" }));
    }

    [Test]
    public void GetClientByIdTestSuccessful()
    {
        var repo = new ClientMockRepo();
        var client = new Client(0, "Alice");
        repo.Add(client);
        var service = new ClientService(repo);

        var result = service.GetClientById(client.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(client.Id));
        Assert.That(result.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void GetClientByIdTestUnsuccessful_IdDoesntExist()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        var result = service.GetClientById(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void AddClientTestSuccessful()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);
        var client = new Client(0, "Alice");

        service.AddClient(client);

        var clients = service.GetAllClients().ToList();
        Assert.That(clients, Has.Count.EqualTo(1));
        Assert.That(clients[0].Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void AddClientTestUnsuccessful_ClientIsNull()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        Assert.Throws<ArgumentNullException>(() => service.AddClient(null!));

        Assert.That(service.GetAllClients(), Is.Empty);
    }

    [Test]
    public void AddClientTestUnsuccessful_NameIsEmpty()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        var ex = Assert.Throws<ArgumentException>(() => service.AddClient(new Client(0, string.Empty)));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
        Assert.That(service.GetAllClients(), Is.Empty);
    }

    [Test]
    public void AddClientTestUnsuccessful_NameIsWhiteSpace()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        var ex = Assert.Throws<ArgumentException>(() => service.AddClient(new Client(0, "   ")));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
        Assert.That(service.GetAllClients(), Is.Empty);
    }

    [Test]
    public void DeleteClientTestSuccessful()
    {
        var repo = new ClientMockRepo();
        var client = new Client(0, "Alice");
        repo.Add(client);
        var service = new ClientService(repo);

        service.DeleteClient(client.Id);

        Assert.That(service.GetAllClients(), Is.Empty);
    }

    [Test]
    public void DeleteClientTestUnsuccessful_IdDoesntExist()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        Client? result = service.DeleteClient(-1);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void UpdateClientTestSuccessful()
    {
        var repo = new ClientMockRepo();
        var client = new Client(0, "Alice");
        repo.Add(client);
        var service = new ClientService(repo);

        service.UpdateClient(new Client(client.Id, "Alice Updated"));

        Assert.That(service.GetClientById(client.Id).Name, Is.EqualTo("Alice Updated"));
    }

    [Test]
    public void UpdateClientTestUnsuccessful_IdDoesntExist()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        Client? result = service.UpdateClient(new Client(-1, "Alice Updated"));

        Assert.That(result, Is.Null);
    }

    [Test]
    public void UpdateClientTestUnsuccessful_ClientIsNull()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        Assert.Throws<ArgumentNullException>(() => service.UpdateClient(null!));
    }

    [Test]
    public void UpdateClientTestUnsuccessful_NameIsEmpty()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        var ex = Assert.Throws<ArgumentException>(() => service.UpdateClient(new Client(0, string.Empty)));

        Assert.That(ex!.Message, Does.Contain("Name is required"));
    }

    [Test]
    public void GetAnyClientSuccessfulTest()
    {
        var repo = new ClientMockRepo();
        var client = new Client(0, "Alice");
        repo.Add(client);
        var service = new ClientService(repo);

        var result = service.GetAnyClient();

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetAnyClientTestUnsuccessful_ClientIsNull()
    {
        var repo = new ClientMockRepo();
        var service = new ClientService(repo);

        var result = service.GetAnyClient();

        Assert.That(result, Is.Null);
    }
}