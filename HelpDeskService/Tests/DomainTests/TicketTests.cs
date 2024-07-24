using Domain.DomainExceptions;
using Domain.Entities;
using Domain.Enums;

namespace DomainTests;

public class TicketTests
{
    [Test]
    public void ShouldNotCreateSetSupportIfTicketAlreadyHaveOne()
    {
        var support = new Support { Id = Guid.NewGuid() };
        var other_support = new Support { Id = Guid.NewGuid() };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket();
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var error = Assert.Throws<TicketAlreadyHaveASupportException>(() => { ticket.SetSupport(other_support); });

        Assert.AreEqual("This ticket already have a support attendant", error.Message);
    }

    [Test]
    public void ShouldNotCreateSetSupportIfUserIsASuport()
    {
        var support = new Support { Id = Guid.NewGuid() };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Support };

        var ticket = new Ticket();
        var error = Assert.Throws<SupportCannotCreateNewTicketException>(() => { ticket.SetClient(client); });

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
    public void
        ShouldChangeTicketStatusToWaitingSupportIfTicketStatusIsNewAndTicketActionIsFromClientOnAddingNewComment()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.New };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment
            { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        ticket.AddComment(comment, Domain.Enums.TicketAction.FromClient);
        Assert.AreEqual(Domain.Enums.TicketStatus.Waiting_Support, ticket.TicketStatus);
    }

    [Test]
    public void
        ShouldChangeTicketStatusToWaitingClientIfTicketStatusIsNewAndTicketActionIsFromSupportOnAddingNewComment()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.New };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment
        {
            IsClientComment = true,
            Client = client,
            ClientId = client.Id,
            Text = "New message from user"
        };

        ticket.AddComment(comment, Domain.Enums.TicketAction.FromSupport);
        Assert.AreEqual(Domain.Enums.TicketStatus.Waiting_Client, ticket.TicketStatus);
    }

    [Test]
    public void
        ShouldChangeTicketStatusToWaitingClientIfTicketStatusIsWaitingClientAndTicketActionIsFromSupportOnAddingNewComment()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.Waiting_Client };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment
            { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        ticket.AddComment(comment, Domain.Enums.TicketAction.FromSupport);
        Assert.AreEqual(Domain.Enums.TicketStatus.Waiting_Client, ticket.TicketStatus);
    }

    [Test]
    public void
        ShouldChangeTicketStatusToWaitingSupportIfTicketStatusIsWaitingClientAndTicketActionIsFromClientOnAddingNewComment()
    {
        var supportId = Guid.NewGuid();
        var support = new Support { Id = supportId };
        var client = new Client { Id = Guid.NewGuid(), Role = Domain.Enums.UserRole.Client };

        var ticket = new Ticket { TicketStatus = Domain.Enums.TicketStatus.Waiting_Client };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        var comment = new Comment
            { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        ticket.AddComment(comment, Domain.Enums.TicketAction.FromClient);
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

        var comment = new Comment
            { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        var error = Assert.Throws<TicketCancelledException>(() =>
        {
            ticket.AddComment(comment, Domain.Enums.TicketAction.FromClient);
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

        var comment = new Comment
            { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        var error = Assert.Throws<TicketCancelledException>(() =>
        {
            ticket.AddComment(comment, Domain.Enums.TicketAction.FromSupport);
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

        var comment = new Comment
            { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        var error = Assert.Throws<TicketFinishedException>(() =>
        {
            ticket.AddComment(comment, Domain.Enums.TicketAction.FromClient);
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

        var comment = new Comment
            { Client = client, ClientId = client.Id, IsClientComment = true, Text = "New message from user" };
        var error = Assert.Throws<TicketFinishedException>(() =>
        {
            ticket.AddComment(comment, Domain.Enums.TicketAction.FromSupport);
        });
        Assert.AreEqual(error.Message, "This ticket was cancelled");
    }

    [Test]
    public void ShouldFinishTheTicketIfTicketActionIsFromClientAndTicketStatusIsWaitingClient()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Waiting_Client,
            Title = "Title"
        };
        ticket.FinishTicket(TicketAction.FromClient);

        Assert.AreEqual(ticket.TicketStatus, TicketStatus.Finished);
    }

    [Test]
    public void ShouldFinishTheTicketIfTicketActionIsFromClientAndTicketStatusIsWaitingSupport()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Waiting_Support,
            Title = "Title"
        };
        ticket.FinishTicket(TicketAction.FromClient);

        Assert.AreEqual(ticket.TicketStatus, TicketStatus.Finished);
    }

    [Test]
    public void ShouldFinishTheTicketIfTicketActionIsFromSupportAndTicketStatusIsWaitingClient()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Waiting_Client,
            Title = "Title"
        };
        ticket.FinishTicket(TicketAction.FromSupport);

        Assert.AreEqual(ticket.TicketStatus, TicketStatus.Finished);
    }

    [Test]
    public void ShouldFinishTheTicketIfTicketActionIsFromSupportAndTicketStatusIsWaitingSupport()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Waiting_Support,
            Title = "Title"
        };
        ticket.FinishTicket(TicketAction.FromSupport);

        Assert.AreEqual(ticket.TicketStatus, TicketStatus.Finished);
    }

    [Test]
    public void ShouldNotFinishTicketIfTicketActionIsFromClientAndTicketStatusIsFinished()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Finished,
            Title = "Title"
        };
        var error = Assert.Throws<TicketAlreadyFinishedException>(() =>
        {
            ticket.FinishTicket(TicketAction.FromClient);
        });
        Assert.AreEqual(error.Message, "The ticket was already finished");
    }

    [Test]
    public void ShouldNotFinishTicketIfTicketActionIsFromSupportAndTicketStatusIsFinished()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Finished,
            Title = "Title"
        };
        var error = Assert.Throws<TicketAlreadyFinishedException>(() =>
        {
            ticket.FinishTicket(TicketAction.FromSupport);
        });
        Assert.AreEqual(error.Message, "The ticket was already finished");
    }

    [Test]
    public void ShouldNotFinishTicketIfTicketActionIsFromClientAndTicketStatusIsCancelled()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Cancelled,
            Title = "Title"
        };
        var error = Assert.Throws<TicketAlreadyCancelledException>(() =>
        {
            ticket.FinishTicket(TicketAction.FromClient);
        });
        Assert.AreEqual(error.Message, "The ticket was already cancelled");
    }

    [Test]
    public void ShouldNotFinishTicketIfTicketActionIsFromSupportAndTicketStatusIsCancelled()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Cancelled,
            Title = "Title"
        };
        var error = Assert.Throws<TicketAlreadyCancelledException>(() =>
        {
            ticket.FinishTicket(TicketAction.FromSupport);
        });
        Assert.AreEqual(error.Message, "The ticket was already cancelled");
    }

    [Test]
    public void ShouldCancelTicketIfTicketActionsIsFromClientAndTicketStatusIsWaitingClient()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Waiting_Client,
            Title = "Title"
        };
        ticket.CancelTicket(TicketAction.FromClient);
        Assert.AreEqual(ticket.TicketStatus, TicketStatus.Cancelled);
    }

    [Test]
    public void ShouldCancelTicketIfTicketActionsIsFromClientAndTicketStatusIsWaitingSupport()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Waiting_Support,
            Title = "Title"
        };
        ticket.CancelTicket(TicketAction.FromClient);
        Assert.AreEqual(ticket.TicketStatus, TicketStatus.Cancelled);
    }

    [Test]
    public void ShouldCancelTicketIfTicketActionsIsFromSupportAndTicketStatusIsWaitingClient()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Waiting_Client,
            Title = "Title"
        };
        ticket.CancelTicket(TicketAction.FromSupport);
        Assert.AreEqual(ticket.TicketStatus, TicketStatus.Cancelled);
    }

    [Test]
    public void ShouldCancelTicketIfTicketActionsIsFromSupportAndTicketStatusIsWaitingSupport()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Waiting_Support,
            Title = "Title"
        };
        ticket.CancelTicket(TicketAction.FromSupport);
        Assert.AreEqual(ticket.TicketStatus, TicketStatus.Cancelled);
    }

    [Test]
    public void ShouldNotCancelTicketIfTicketActionsIsFromClientAndTicketStatusIsFinished()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Finished,
            Title = "Title"
        };
        var error = Assert.Throws<TicketAlreadyFinishedException>(() =>
        {
            ticket.CancelTicket(TicketAction.FromClient);
        });
        Assert.AreEqual(error.Message, "The ticket was already finished");
    } 
    [Test]
    public void ShouldNotCancelTicketIfTicketActionsIsFromSupportAndTicketStatusIsFinished()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Finished,
            Title = "Title"
        };
        var error = Assert.Throws<TicketAlreadyFinishedException>(() =>
        {
            ticket.CancelTicket(TicketAction.FromSupport);
        });
        Assert.AreEqual(error.Message, "The ticket was already finished");
    }
    [Test]
    public void ShouldNotCancelTicketIfTicketActionsIsFromClientAndTicketStatusIsCancelled()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Cancelled,
            Title = "Title"
        };
        var error = Assert.Throws<TicketAlreadyCancelledException>(() =>
        {
            ticket.CancelTicket(TicketAction.FromClient);
        });
        Assert.AreEqual(error.Message, "The ticket was already cancelled");
    } 
    [Test]
    public void ShouldNotCancelTicketIfTicketActionsIsFromSupportAndTicketStatusIsCancelled()
    {
        var ticket = new Ticket
        {
            TicketStatus = TicketStatus.Cancelled,
            Title = "Title"
        };
        var error = Assert.Throws<TicketAlreadyCancelledException>(() =>
        {
            ticket.CancelTicket(TicketAction.FromSupport);
        });
        Assert.AreEqual(error.Message, "The ticket was already cancelled");
    }
}