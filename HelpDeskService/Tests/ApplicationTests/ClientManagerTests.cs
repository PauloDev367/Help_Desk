using Application.Client;
using Application.Client.Request;
using Application.Dto;
using Domain.Entities;
using Domain.Ports;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

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
        var isValid = Validator.TryValidateObject(clientRequest, validationContext, validationResults, validateAllProperties: true);

        Assert.AreEqual(false, isValid);
    }
    [TestCase("Nome", "email@email.com", "12345678")]
    [TestCase("Maria", "maria@email.com", "asdalkl123asd")]
    [TestCase("José", "joseh_email@email.com", "askdljlk12jel")]
    [TestCase("Tonho", "abc_123@email.com", "askd21ljlkajs")]
    public void ShouldCreateNewClientIfAllIsRigth(string box,string passwod,string email)
    {
        var clientRequest = new CreateClientRequest
        {
            Name = "abcd",
            Password = "123456789",
            Email = email
        };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(clientRequest, serviceProvider: null, items: null);
        var isValid = Validator.TryValidateObject(clientRequest, validationContext, validationResults, validateAllProperties: true);

        Assert.AreEqual(false, isValid);
    }
}