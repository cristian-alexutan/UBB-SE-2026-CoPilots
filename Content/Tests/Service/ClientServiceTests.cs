using Content.Repository.Interface;
using Content.Service;
using Content.Domain;
using NSubstitute;

namespace Tests;

public class ClientServiceTests
{
    private IClientRepo clientRepo = null!;
    private ClientService clientService = null!;

    [SetUp]
    public void Setup()
    {
        this.clientRepo = Substitute.For<IClientRepo>();
        this.clientService = new ClientService(this.clientRepo);
    }

    [Test]
    public void AddClient_NullClient_ThrowsException()
    {
        var exception = Assert.Catch<Exception>(() => this.clientService.AddClient(null));
        Assert.That(exception!.Message, Does.Contain("Client must not be null"));
    }

    [Test]
    public void AddClient_NullName_ThrowsException()
    {
        Client client = new Client(1, null);
        var exception = Assert.Catch<Exception>(() => this.clientService.AddClient(client));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddClient_EmptyStringName_ThrowsException()
    {
        Client client = new Client(1, string.Empty);
        var exception = Assert.Catch<Exception>(() => this.clientService.AddClient(client));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddClient_WhitespaceName_ThrowsException()
    {
        Client client = new Client(1, " ");
        var exception = Assert.Catch<Exception>(() => this.clientService.AddClient(client));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void AddClient_ValidClient_ClientAddedToRepo()
    {
        Client client = new Client(1, "Test");
        this.clientService.AddClient(client);
        this.clientRepo.Received(1).Add(client);
    }

    [Test]
    public void DeleteClient_ValidId_ClientDeletedFromRepo()
    {
        int clientId = 1;
        this.clientService.DeleteClient(clientId);
        this.clientRepo.Received(1).Delete(clientId);
    }

    [Test]
    public void UpdateClient_NullClient_ThrowsException()
    {
        var exception = Assert.Catch<Exception>(() => this.clientService.UpdateClient(null));
        Assert.That(exception!.Message, Does.Contain("Client must not be null"));
    }

    [Test]
    public void UpdateClient_NullName_ThrowsException()
    {
        Client client = new Client(1, null);
        var exception = Assert.Catch<Exception>(() => this.clientService.UpdateClient(client));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void UpdateClient_EmptyStringName_ThrowsException()
    {
        Client client = new Client(1, string.Empty);
        var exception = Assert.Catch<Exception>(() => this.clientService.UpdateClient(client));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void UpdateClient_WhitespaceName_ThrowsException()
    {
        Client client = new Client(1, " ");
        var exception = Assert.Catch<Exception>(() => this.clientService.UpdateClient(client));
        Assert.That(exception!.Message, Does.Contain("Name field must not be empty"));
    }

    [Test]
    public void UpdateClient_ValidClient_ClientUpdatedInRepo()
    {
        Client client = new Client(1, "Test");
        this.clientService.UpdateClient(client);
        this.clientRepo.Received(1).Update(client);
    }

    [Test]
    public void GetAnyClient_NoClients_ThrowsException()
    {
        this.clientRepo.GetAll().Returns(new List<Client>());
        Assert.Catch<Exception>(() => this.clientService.GetAnyClient());
    }

    [Test]
    public void GetAnyClient_ClientExists_ReturnsClient()
    {
        Client client = new Client(1, "Test");
        this.clientRepo.GetAll().Returns(new List<Client> { client });
        Client? result = this.clientService.GetAnyClient();
        Assert.That(result, Is.EqualTo(client));
    }
}

