using Domain.DomainExceptions;
using Domain.Entities;

namespace DomainTests;

public class TicketTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ShouldNotCreateSetSupportIfTicketAlreadyHaveOne()
    {
        var support = new Support { Id = Guid.NewGuid() };
        var other_support = new Support { Id = Guid.NewGuid() };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket();
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var error = Assert.Throws<TicketAlreadyHaveASupportException>(() =>
        {
            ticket.SetSupport(other_support);
        });

        Assert.AreEqual("This ticket already have a support attendant", error.Message);
    }

    [Test]
    public void ShouldNotCreateSetSupportIfUserIsASuport()
    {
        var support = new Support { Id = Guid.NewGuid() };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Support };

        var ticket = new Ticket();
        var error = Assert.Throws<SupportCannotCreateNewTicketException>(() =>
        {
            ticket.SetClient(client);
        });

        Assert.AreEqual("Supports cannot create new tickets, only users", error.Message);
    }
    [Test]
    public void ShouldCreateNewTicketWithSuport()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket();
        ticket.SetClient(client);
        ticket.SetSupport(support);

        Assert.AreEqual(supportId, support.Id);
    }
}