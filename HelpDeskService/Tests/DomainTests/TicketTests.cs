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
    [Test]
    public void ShouldCreateNewTicketWithoutSuport()
    {
        var ticketId = Guid.NewGuid();
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { Id = ticketId };
        ticket.SetClient(client);

        Assert.AreEqual(ticketId, ticket.Id);
    }
    [Test]
    public void ShouldChangeTicketStatusToWaitingSupportIfTicketStatusIsNewAndMessageActionsIsFromClientOnAddingNewComment()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.New };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        ticket.AddComment(comment, Domain.Enums.MessageAction.FromClient);
        Assert.AreEqual(Domain.Enums.TicketStatus.Waiting_Support, ticket.TicketStatus);
    }
    [Test]
    public void ShouldChangeTicketStatusToWaitingClientIfTicketStatusIsNewAndMessageActionsIsFromSupportOnAddingNewComment()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.New };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        ticket.AddComment(comment, Domain.Enums.MessageAction.FromSupport);
        Assert.AreEqual(Domain.Enums.TicketStatus.Waiting_Client, ticket.TicketStatus);
    }
    [Test]
    public void ShouldChangeTicketStatusToWaitingClientIfTicketStatusIsWaitingClientAndMessageActionsIsFromSupportOnAddingNewComment()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.Waiting_Client };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        ticket.AddComment(comment, Domain.Enums.MessageAction.FromSupport);
        Assert.AreEqual(Domain.Enums.TicketStatus.Waiting_Client, ticket.TicketStatus);
    }
    [Test]
    public void ShouldChangeTicketStatusToWaitingSupportIfTicketStatusIsWaitingClientAndMessageActionsIsFromClientOnAddingNewComment()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.Waiting_Client };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment { Client = client, ClientId = client.Id ,IsClientComment = true, Text = "New message from user" };
        ticket.AddComment(comment, Domain.Enums.MessageAction.FromClient);
        Assert.AreEqual(Domain.Enums.TicketStatus.Waiting_Client, ticket.TicketStatus);
    }
    [Test]
    public void ShouldThrowTicketCancelledExceptionIfTicketStatusIsCancelledAndActionsIsFromClientOnAddinNewMessage()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.Cancelled };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        var error = Assert.Throws<TicketCancelledException>(() =>
        {
            ticket.AddComment(comment, Domain.Enums.MessageAction.FromClient);
        });
        Assert.AreEqual(error.Message, "This ticket was cancelled");
    }
    [Test]
    public void ShouldThrowTicketCancelledExceptionIfTicketStatusIsCancelledAndActionIsFromSupportOnAddinNewMessage()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.Cancelled };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        var error = Assert.Throws<TicketCancelledException>(() =>
        {
            ticket.AddComment(comment, Domain.Enums.MessageAction.FromSupport);
        });
        Assert.AreEqual(error.Message, "This ticket was cancelled");
    }
    [Test]
    public void ShouldThrowTicketFinishedExceptionIfTicketStatusIsFinishedAndActionsIsFromClientOnAddinNewMessage()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.Finished };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        var error = Assert.Throws<TicketFinishedException>(() =>
        {
            ticket.AddComment(comment, Domain.Enums.MessageAction.FromClient);
        });
        Assert.AreEqual(error.Message, "This ticket was cancelled");
    }
    [Test]
    public void ShouldThrowTicketFinishedExceptionIfTicketStatusIsFinishedAndActionsIsFromSupportOnAddinNewMessage()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.Finished };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        var error = Assert.Throws<TicketFinishedException>(() =>
        {
            ticket.AddComment(comment, Domain.Enums.MessageAction.FromSupport);
        });
        Assert.AreEqual(error.Message, "This ticket was cancelled");
    }
}