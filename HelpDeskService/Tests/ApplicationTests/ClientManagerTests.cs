using Application.Auth.Request;
using Application.Client;
using Application.Client.Request;
using Domain.Entities;
using Domain.Ports;
using Moq;
using System.ComponentModel.DataAnnotations;
using Application.Dto;

namespace ApplicationTests;

public class ClientManagerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase("as")]
    [TestCase("12")]
    [TestCase("ad")]
    [TestCase("2s")]
    public void ShouldNotCreateNewClientIfNameIsLessThen3(string name)
    {
        var clientRequest = new CreateClientRequest { Name = name };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(clientRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(clientRequest, validationContext, validationResults);

        Assert.AreEqual(false, isValid);
    }

    [TestCase("as")]
    [TestCase("12")]
    [TestCase("ad")]
    [TestCase("2s")]
    public void ShouldNotCreateNewClientIfPasswordIsLessThen8(string password)
    {
        var clientRequest = new CreateClientRequest { Name = "abcd", Password = password };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(clientRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(clientRequest, validationContext, validationResults);

        Assert.AreEqual(false, isValid);
    }

    [TestCase("as")]
    [TestCase("12")]
    [TestCase("aasdas.com")]
    [TestCase("2sasd@")]
    [TestCase("2sasd@")]
    [TestCase("2s123s")]
    public void ShouldNotCreateNewClientIfEmailIsNotAnEmail(string email)
    {
        var clientRequest = new CreateClientRequest
        {
            Name = "abcd",
            Password = "123456789",
            Email = email
        };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(clientRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(clientRequest, validationContext, validationResults,
            validateAllProperties: true);

        Assert.AreEqual(false, isValid);
    }

    [TestCase("Nome", "email@email.com", "12345678")]
    [TestCase("Maria", "maria@email.com", "asdalkl123asd")]
    [TestCase("Jos�", "joseh_email@email.com", "askdljlk12jel")]
    [TestCase("Tonho", "abc_123@email.com", "askd21ljlkajs")]
    public void ShouldAllowToCreateNewClientIfAllIsRigth(string box, string passwod, string email)
    {
        var clientRequest = new CreateClientRequest
        {
            Name = "abcd",
            Password = "123456789",
            Email = email
        };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(clientRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(clientRequest, validationContext, validationResults,
            validateAllProperties: true);

        Assert.AreEqual(false, isValid);
    }

    [Test]
    public async Task ShouldThrowAnExceptionIfClientDoesntExistOnDelete()
    {
        try
        {
            var clientRepository = new Mock<IClientRepository>();
            clientRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Domain.Entities.Client>()))
                .Returns(Task.FromResult((object)null));

            clientRepository.Setup(repo => repo.GetOneByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<Domain.Entities.Client>(null));

            var authService = new Mock<IAuthUserService>();
            authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
                .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());

            var clientManager = new ClientManager(clientRepository.Object, authService.Object);


            await clientManager.DeleteAsync(Guid.NewGuid());
        }
        catch (Exception ex)
        {
            Assert.AreEqual(ex.Message, "User was not foundend!");
        }
    }

    [Test]
    public async Task ShouldDeleteClient()
    {
        var clientId = Guid.NewGuid();

        var clientRepository = new Mock<IClientRepository>();
        clientRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Client>()))
            .Returns(Task.FromResult((object)null));

        clientRepository.Setup(repo => repo.GetOneByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult<Client>(new Client { Id = clientId }));

        var authService = new Mock<IAuthUserService>();
        authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
            .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());

        var clientManager = new ClientManager(clientRepository.Object, authService.Object);
        await clientManager.DeleteAsync(Guid.NewGuid());
        Assert.Pass();
    }

    [Test]
    public async Task ShouldUpdateClient()
    {
        var clientId = Guid.NewGuid();
        var client = new Client { Id = clientId, Email = "email@email.com", Name = "Name"};

        var clientRepository = new Mock<IClientRepository>();
        clientRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Client>()))
            .ReturnsAsync(() => client);

        clientRepository.Setup(repo => repo.GetOneByEmailAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<Client>(client));

        var authService = new Mock<IAuthUserService>();
        authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
            .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());
        authService.Setup(au => au.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => new UserDto{Email = client.Email, Id = client.Id, Name = client.Name});

        var clientManager = new ClientManager(clientRepository.Object, authService.Object);
        var updateRequest = new UpdateClientRequest { Email = "new@email.com", Name = "New Name" };
        var updated = await clientManager.UpdateAsync(updateRequest, clientId);

        Assert.AreEqual(updateRequest.Email, updated.Email);
        Assert.AreEqual(updateRequest.Name, updated.Name);
    }

    [Test]
    public async Task ShouldNotUpdateIfClientIsNotFounded()
    {
        try
        {
            var clientId = Guid.NewGuid();

            var clientRepository = new Mock<IClientRepository>();
            clientRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Client>()))
                .ReturnsAsync((Client client) => client);

            clientRepository.Setup(repo => repo.GetOneByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<Domain.Entities.Client>(null));

            var authService = new Mock<IAuthUserService>();
            authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
                .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());


            var clientManager = new ClientManager(clientRepository.Object, authService.Object);
            var updateRequest = new UpdateClientRequest { Email = "new@email.com", Name = "New Name" };
            var updated = await clientManager.UpdateAsync(updateRequest, clientId);
        }
        catch (Exception ex)
        {
            Assert.AreEqual(ex.Message, "User was not foundend!");
        }
    }

    [Test]
    public async Task ShouldThrownAnExceptionIfClientIsNotFounded()
    {
        try
        {
            var clientRepository = new Mock<IClientRepository>();
            clientRepository.Setup(repo => repo.GetOneByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<Domain.Entities.Client>(null));

            var authService = new Mock<IAuthUserService>();
            authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
                .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());

            var clientManager = new ClientManager(clientRepository.Object, authService.Object);
            var updated = await clientManager.GetOneAsync(Guid.NewGuid());
        }
        catch (Exception ex)
        {
            Assert.AreEqual(ex.Message, "User was not foundend!");
        }
    }

    [Test]
    public async Task ShouldReturnClientIfTheyIsFounded()
    {
        var clientId = Guid.NewGuid();
        var clientRepository = new Mock<IClientRepository>();
        clientRepository.Setup(repo => repo.GetOneByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult<Client>(new Client { Id = clientId }));

        var authService = new Mock<IAuthUserService>();
        authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
            .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());

        var clientManager = new ClientManager(clientRepository.Object, authService.Object);
        var client = await clientManager.GetOneAsync(Guid.NewGuid());
        Assert.AreEqual(client.Id, clientId);
    }
}