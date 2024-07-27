using Application.Auth.Request;
using Application.Support;
using Application.Support.Request;
using Domain.Entities;
using Domain.Ports;
using Moq;
using System.ComponentModel.DataAnnotations;
using Application.Dto;
using Application.Exceptions;

namespace ApplicationTests;

public class SupportManagerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase("as")]
    [TestCase("12")]
    [TestCase("ad")]
    [TestCase("2s")]
    public void ShouldNotCreateNewSupportIfNameIsLessThen3(string name)
    {
        var supportRequest = new CreateSupportRequest { Name = name };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(supportRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(supportRequest, validationContext, validationResults);

        Assert.AreEqual(false, isValid);
    }

    [TestCase("as")]
    [TestCase("12")]
    [TestCase("ad")]
    [TestCase("2s")]
    public void ShouldNotCreateNewSupportIfPasswordIsLessThen8(string password)
    {
        var supportRequest = new CreateSupportRequest { Name = "abcd", Password = password };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(supportRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(supportRequest, validationContext, validationResults);

        Assert.AreEqual(false, isValid);
    }

    [TestCase("as")]
    [TestCase("12")]
    [TestCase("aasdas.com")]
    [TestCase("2sasd@")]
    [TestCase("2sasd@")]
    [TestCase("2s123s")]
    public void ShouldNotCreateNewSupportIfEmailIsNotAnEmail(string email)
    {
        var supportRequest = new CreateSupportRequest
        {
            Name = "abcd",
            Password = "123456789",
            Email = email
        };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(supportRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(supportRequest, validationContext, validationResults,
            validateAllProperties: true);

        Assert.AreEqual(false, isValid);
    }

    [TestCase("Nome", "email@email.com", "12345678")]
    [TestCase("Maria", "maria@email.com", "asdalkl123asd")]
    [TestCase("Jos�", "joseh_email@email.com", "askdljlk12jel")]
    [TestCase("Tonho", "abc_123@email.com", "askd21ljlkajs")]
    public void ShouldAllowToCreateNewSupportIfAllIsRigth(string box, string passwod, string email)
    {
        var supportRequest = new CreateSupportRequest
        {
            Name = "abcd",
            Password = "123456789",
            Email = email
        };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(supportRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(supportRequest, validationContext, validationResults,
            validateAllProperties: true);

        Assert.AreEqual(false, isValid);
    }

    [Test]
    public async Task ShouldThrowAnExceptionIfSupportDoesntExistOnDelete()
    {
        try
        {
            var supportRepository = new Mock<ISupportRepository>();
            supportRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Domain.Entities.Support>()))
                .Returns(Task.FromResult((object)null));

            supportRepository.Setup(repo => repo.GetOneByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<Domain.Entities.Support>(null));

            var authService = new Mock<IAuthUserService>();
            authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
                .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());

            var supportManager = new SupportManager(supportRepository.Object, authService.Object);

            await supportManager.DeleteAsync(Guid.NewGuid());
        }
        catch (Exception ex)
        {
            Assert.AreEqual(ex.Message, "Support was not founded!");
        }
    }

    [Test]
    public async Task ShouldDeleteSupport()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId, Email = "email@email.com", Name = "Name" };
        
        var supportRepository = new Mock<ISupportRepository>();
        supportRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Support>()))
            .Returns(Task.FromResult((object)null));

        supportRepository.Setup(repo => repo.GetOneByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(support);

        var authService = new Mock<IAuthUserService>();
        authService.Setup(au => au.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => new UserDto { Email = support.Email, Id = support.Id, Name = support.Name });
        authService.Setup(au => au.DeleteAsync(It.IsAny<Support>()))
            .Returns(() => Task.FromResult((Support)null));
        
        var supportManager = new SupportManager(supportRepository.Object, authService.Object);
        await supportManager.DeleteAsync(Guid.NewGuid());
        Assert.Pass();
    }

    [Test]
    public async Task ShouldUpdateSupport()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId, Email = "email@email.com", Name = "Name" };

        var supportRepository = new Mock<ISupportRepository>();
        supportRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Support>()))
            .ReturnsAsync((Support support) => support);

        supportRepository.Setup(repo => repo.GetOneByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(support);

        var authService = new Mock<IAuthUserService>();
        authService.Setup(au => au.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => new UserDto { Email = support.Email, Id = support.Id, Name = support.Name });

        var supportManager = new SupportManager(supportRepository.Object, authService.Object);
        var updateRequest = new UpdateSupportRequest { Email = "new@email.com", Name = "New Name" };
        var updated = await supportManager.UpdateAsync(updateRequest, supportId);

        Assert.AreEqual(updateRequest.Email, updated.Email);
        Assert.AreEqual(updateRequest.Name, updated.Name);
    }

    [Test]
    public async Task ShouldNotUpdateIfAuthSupportIsNotFounded()
    {
        var supportId = Guid.NewGuid();
        var support = (Support)null;

        var supportRepository = new Mock<ISupportRepository>();
        supportRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Support>()))
            .ReturnsAsync((Support support) => support);

        supportRepository.Setup(repo => repo.GetOneByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(support);

        var authService = new Mock<IAuthUserService>();
        authService.Setup(au => au.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => (UserDto)null);

        var supportManager = new SupportManager(supportRepository.Object, authService.Object);
        var updateRequest = new UpdateSupportRequest { Email = "new@email.com", Name = "New Name" };
        var error = Assert.ThrowsAsync<SupportNotFoundedException>(async () =>
        {
            await supportManager.UpdateAsync(updateRequest, supportId);
        });
        Assert.AreEqual(error.Message, "Support was not founded!");
    }

    [Test]
    public async Task ShouldNotUpdateSupportIfSystemSupportIsNotFounded()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId, Email = "email@email.com", Name = "Name" };

        var supportRepository = new Mock<ISupportRepository>();
        supportRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Support>()))
            .ReturnsAsync((Support support) => support);

        supportRepository.Setup(repo => repo.GetOneByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Support)null);

        var authService = new Mock<IAuthUserService>();
        authService.Setup(au => au.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => new UserDto { Email = support.Email, Id = support.Id, Name = support.Name });

        var supportManager = new SupportManager(supportRepository.Object, authService.Object);
        var updateRequest = new UpdateSupportRequest { Email = "new@email.com", Name = "New Name" };
        var error = Assert.ThrowsAsync<SupportNotFoundedException>(async () =>
        {
            await supportManager.UpdateAsync(updateRequest, supportId);
        });
        Assert.AreEqual(error.Message, "Support was not founded!");
    }
    
    [Test]
    public async Task ShouldThrownAnExceptionIfSupportIsNotFounded()
    {
        try
        {
            var supportRepository = new Mock<ISupportRepository>();
            supportRepository.Setup(repo => repo.GetOneByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<Domain.Entities.Support>(null));

            var authService = new Mock<IAuthUserService>();
            authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
                .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());

            var supportManager = new SupportManager(supportRepository.Object, authService.Object);
            var updated = await supportManager.GetOneAsync(Guid.NewGuid());
        }
        catch (Exception ex)
        {
            Assert.AreEqual(ex.Message, "User was not foundend!");
        }
    }

    [Test]
    public async Task ShouldReturnSupportIfTheyIsFounded()
    {
        var supportId = Guid.NewGuid();
        var supportRepository = new Mock<ISupportRepository>();
        supportRepository.Setup(repo => repo.GetOneByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult<Support>(new Support { Id = supportId }));

        var authService = new Mock<IAuthUserService>();
        authService.Setup(au => au.RegisterAsync(It.IsAny<RegisterUserRequest>()))
            .ReturnsAsync(new Application.Auth.Response.RegisteredUserResponse());

        var supportManager = new SupportManager(supportRepository.Object, authService.Object);
        var support = await supportManager.GetOneAsync(Guid.NewGuid());
        Assert.AreEqual(support.Id, supportId);
    }
}