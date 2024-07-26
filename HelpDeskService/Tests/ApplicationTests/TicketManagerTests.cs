using Application.Exceptions;
using Application.Ticket;
using Application.Ticket.Request;
using Domain.Entities;
using Domain.Enums;
using Domain.Ports;
using Moq;

namespace ApplicationTests;

public class TicketManagerTests
{
    private Mock<IClientRepository> _clientRepository;
    private Mock<ISupportRepository> _supportRepository;
    private Mock<ITicketRepository> _ticketRepository;

    [SetUp]
    public void Init()
    {
        _clientRepository = new Mock<IClientRepository>();
        _supportRepository = new Mock<ISupportRepository>();
        _ticketRepository = new Mock<ITicketRepository>();
    }

    [Test]
    public async Task ShouldCreateNewTicket()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var title = "Ticket Title";
        var request = new CreateTicketRequest { Title = title };
        request.SetClientId(clientId);
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid clientId) => new Client { Id = clientId });

        _ticketRepository.Setup(t => t.CreateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() =>
                new Ticket
                {
                    Id = ticketId,
                    Title = title,
                    TicketStatus = TicketStatus.New,
                }
            );

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var created = await ticketManager.CreateAsync(request);

        Assert.AreEqual(created.Id, ticketId);
    }

    [Test]
    public async Task ShouldNotCreateNewTicketIfClientIsNotFound()
    {
        try
        {
            var clientId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var title = "Ticket Title";
            var request = new CreateTicketRequest { Title = title };
            request.SetClientId(clientId);
            _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Client)null);

            _ticketRepository.Setup(t => t.CreateAsync(It.IsAny<Ticket>()))
                .ReturnsAsync(() =>
                    new Ticket
                    {
                        Id = ticketId,
                        Title = title,
                        TicketStatus = TicketStatus.New,
                    }
                );

            var ticketManager =
                new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

            await ticketManager.CreateAsync(request);

            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.AreEqual("Client was not founded", e.Message);
        }
    }

    [Test]
    public async Task ShouldNotCreateNewTicketIfClientRoleIsDifferentThanClient()
    {
        try
        {
            var clientId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var title = "Ticket Title";
            var request = new CreateTicketRequest { Title = title };
            request.SetClientId(clientId);
            _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid clientId) => new Client { Id = clientId, Role = UserRole.Support });

            _ticketRepository.Setup(t => t.CreateAsync(It.IsAny<Ticket>()))
                .ReturnsAsync(() =>
                    new Ticket
                    {
                        Id = ticketId,
                        Title = title,
                        TicketStatus = TicketStatus.New,
                    }
                );

            var ticketManager =
                new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);
            await ticketManager.CreateAsync(request);
            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.AreEqual(e.Message, "Supports cannot create new tickets, only users");
        }
    }

    [Test]
    public async Task ShouldGetAllClientTickets()
    {
        var clientId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => client);

        _ticketRepository.Setup(t => t.GetAllFromUserAsync(
            It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<string>(), It.IsAny<string>()
        )).ReturnsAsync((Guid userId, int perPage, int page, string orderBy, string order) =>
            {
                var ticket1 = new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Ticket Title", TicketStatus = TicketStatus.New,
                    Comments = new List<Comment>()
                };
                ticket1.SetClient(client);
                ticket1.SetSupport(support);
                var ticket2 = new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Ticket Title", TicketStatus = TicketStatus.New,
                    Comments = new List<Comment>()
                };
                ticket2.SetClient(client);
                ticket2.SetSupport(support);

                var ticketList = new List<Ticket>();
                ticketList.Add(ticket1);
                ticketList.Add(ticket2);

                return ticketList;
            }
        );

        var request = new GetTicketFromUserRequest
        {
            PerPage = 10, Page = 0, OrderBy = "id,desc"
        };

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var data = await ticketManager.GetClientTicketsAsync(request, clientId);
        Assert.AreEqual(2, data.Tickets.Count);
    }

    [Test]
    public async Task ShoulNotGetAllClientTicketsfClientIsNotFound()
    {
        try
        {
            var clientId = Guid.NewGuid();
            var client = (Client)null;

            _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => client);

            _ticketRepository.Setup(t => t.GetAllFromUserAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>()
            )).ReturnsAsync((Guid userId, int perPage, int page, string orderBy, string order) =>
                new List<Ticket>()
            );

            var request = new GetTicketFromUserRequest
            {
                PerPage = 10, Page = 0, OrderBy = "id,desc"
            };

            var ticketManager =
                new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

            await ticketManager.GetClientTicketsAsync(request, clientId);
            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.AreEqual(e.Message, "Client was not founded");
        }
    }

    [Test]
    public async Task ShouldGetOneTicket()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        var ticket = new Ticket { Id = ticketId, TicketStatus = TicketStatus.New, Title = "Title" };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var ticketSearch = await ticketManager.GetOneAsync(ticketId);
        Assert.AreEqual(ticketId, ticketSearch.Id);
    }

    [Test]
    public async Task ShouldNotGetOneTicketIfTicketIsNotFounded()
    {
        var ticketId = Guid.NewGuid();

        var ticket = (Ticket)null;
        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var error = Assert.ThrowsAsync<TicketNotFoundedException>(async () =>
            await ticketManager.GetOneAsync(ticketId));
        Assert.AreEqual(error.Message, "Ticket was not founded");
    }

    [Test]
    public async Task ShouldGetOneTicketFromClient()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => client);

        var ticket = new Ticket { Id = ticketId, TicketStatus = TicketStatus.New, Title = "Title" };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var ticketSearch = await ticketManager.GetOneFromClientAsync(ticketId, clientId);
        Assert.AreEqual(ticketId, ticketSearch.Id);
        Assert.AreEqual(clientId, ticketSearch.Client.Id);
    }

    [Test]
    public async Task ShouldThrownAnExceptionIfClientTicketIsNotFounded()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var ticket = (Ticket)null;

        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var error = Assert.ThrowsAsync<TicketNotFoundedException>(async () =>
            await ticketManager.GetOneFromClientAsync(ticketId, clientId));
        Assert.AreEqual(error.Message, "Ticket was not founded");
    }

    [Test]
    public async Task ShouldAddNewCommentToTicketWhenActionsIsFromClient()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => client);

        var ticket = new Ticket { Id = ticketId, TicketStatus = TicketStatus.New, Title = "Title" };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket ticket) => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);
        var request =
            new AddCommentToTicketRequest(ticketId, "Comment Message", TicketAction.FromClient, clientId, null);
        var update = await ticketManager.AddCommentAsync(request);

        _ticketRepository.Verify(t => t.UpdateAsync(ticket), Times.Once);
        Assert.IsNotNull(update);
    }

    [Test]
    public async Task ShouldNotAddNewCommentToTicketIfTicketIsNotFoundWhenActionIsFromClient()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => client);

        var ticket = (Ticket)null;

        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket ticket) => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);
        var request =
            new AddCommentToTicketRequest(ticketId, "Comment Message", TicketAction.FromClient, clientId, null);
        var error = Assert.ThrowsAsync<TicketNotFoundedException>(async () =>
            await ticketManager.AddCommentAsync(request));
        Assert.AreEqual(error.Message, "Ticket was not founded");
    }

    [Test]
    public async Task ShouldAddNewCommentToTicketWhenActionsIsFromSupport()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => client);

        var ticket = new Ticket { Id = ticketId, TicketStatus = TicketStatus.New, Title = "Title" };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket ticket) => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);
        var request =
            new AddCommentToTicketRequest(ticketId, "Comment Message", TicketAction.FromSupport, clientId, support.Id);
        var update = await ticketManager.AddCommentAsync(request);

        _ticketRepository.Verify(t => t.UpdateAsync(ticket), Times.Once);
        Assert.IsNotNull(update);
    }

    [Test]
    public async Task ShouldNotAddNewCommentToTicketIfTicketIsNotFoundWhenActionsIsFromSupport()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => client);

        var ticket = (Ticket)null;

        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket ticket) => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);
        var request =
            new AddCommentToTicketRequest(ticketId, "Comment Message", TicketAction.FromSupport, clientId, support.Id);
        var error = Assert.ThrowsAsync<TicketNotFoundedException>(async () =>
            await ticketManager.AddCommentAsync(request));
        Assert.AreEqual(error.Message, "Ticket was not founded");
    }

    [Test]
    public async Task
        ShouldNotAddNewCommentToTicketWIfTicketSupportIdIsNullAndSupportIsNotFoundedhenActionsIsFromSupport()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => client);

        var ticket = new Ticket { Id = ticketId, TicketStatus = TicketStatus.New, Title = "Title" };
        ticket.SetClient(client);

        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket ticket) => ticket);
        _supportRepository.Setup(s => s.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => (Support)null);
        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);
        var request =
            new AddCommentToTicketRequest(ticketId, "Comment Message", TicketAction.FromSupport, clientId, support.Id);
        var error = Assert.ThrowsAsync<InvalidSupportException>(
            async () => await ticketManager.AddCommentAsync(request));

        Assert.AreEqual(error.Message, "Support was not founded");
    }

    [Test]
    public async Task
        ShouldNotAddNewCommentToTicketIfSupportIdIsNotNullButIsDifferentThenRequestSupportIdWhenActionsIsFromSupport()
    {
        var clientId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = clientId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };
        _clientRepository.Setup(c => c.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => client);

        var ticket = new Ticket { Id = ticketId, TicketStatus = TicketStatus.New, Title = "Title" };
        ticket.SetClient(client);
        ticket.SetSupport(support);

        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket ticket) => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);
        var request =
            new AddCommentToTicketRequest(ticketId, "Comment Message", TicketAction.FromSupport, clientId,
                Guid.NewGuid());
        var error = Assert.ThrowsAsync<InvalidSupportException>(
            async () => await ticketManager.AddCommentAsync(request));

        Assert.AreEqual(error.Message, "You don't have permission to access this ticket! They already have a support");
    }

    [Test]
    public async Task ShouldCancelTicketWhenActionIsFromClient()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var supportId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = supportId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };

        var ticket = new Ticket
        {
            Id = ticketId, Title = "Title", TicketStatus = TicketStatus.New,
        };
        ticket.SetClient(client);
        ticket.SetSupport(support);
        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var updated = await ticketManager.CancelTicketAsync(ticketId, TicketAction.FromClient, clientId);
        Assert.AreEqual(TicketStatus.Cancelled.ToString(), updated.TicketStatus);
    }

    [Test]
    public async Task ShouldCancelTicketWhenActionIsFromSupport()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var supportId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = supportId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };

        var ticket = new Ticket
        {
            Id = ticketId, Title = "Title", TicketStatus = TicketStatus.New,
        };
        ticket.SetClient(client);
        ticket.SetSupport(support);
        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var updated = await ticketManager.CancelTicketAsync(ticketId, TicketAction.FromSupport, clientId);
        Assert.AreEqual(TicketStatus.Cancelled.ToString(), updated.TicketStatus);
    }

    [Test]
    public async Task ShouldNotCancelTicketIfTicketIsNotFound()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        var ticket = (Ticket)null;

        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var error = Assert.ThrowsAsync<TicketNotFoundedException>(async () =>
        {
            await ticketManager.CancelTicketAsync(ticketId, TicketAction.FromSupport, clientId);
        });

        Assert.AreEqual(error.Message, "Ticket was not founded");
    }

    [Test]
    public async Task ShouldFinishTicketWhenActionIsFromClient()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var supportId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = supportId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };

        var ticket = new Ticket
        {
            Id = ticketId, Title = "Title", TicketStatus = TicketStatus.New,
        };
        ticket.SetClient(client);
        ticket.SetSupport(support);
        _ticketRepository.Setup(t => t.GetOneFromClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var updated = await ticketManager.FinishTicketAsync(ticketId, TicketAction.FromClient, clientId);
        Assert.AreEqual(TicketStatus.Finished.ToString(), updated.TicketStatus);
    }

    [Test]
    public async Task ShouldFinishTicketWhenActionIsFromSupport()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var supportId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = supportId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };

        var ticket = new Ticket
        {
            Id = ticketId, Title = "Title", TicketStatus = TicketStatus.New,
        };
        ticket.SetClient(client);
        ticket.SetSupport(support);
        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var updated = await ticketManager.FinishTicketAsync(ticketId, TicketAction.FromSupport, clientId);
        Assert.AreEqual(TicketStatus.Finished.ToString(), updated.TicketStatus);
    }

    [Test]
    public async Task ShouldNotFinishTicketIfTicketIsNotFound()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        var ticket = (Ticket)null;

        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var error = Assert.ThrowsAsync<TicketNotFoundedException>(async () =>
        {
            await ticketManager.FinishTicketAsync(ticketId, TicketAction.FromSupport, clientId);
        });

        Assert.AreEqual(error.Message, "Ticket was not founded");
    }

    [Test]
    public async Task ShouldAddSupportToTicket()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var supportId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = supportId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };

        var ticket = new Ticket
        {
            Id = ticketId, Title = "Title", TicketStatus = TicketStatus.New,
        };
        ticket.SetClient(client);
        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _supportRepository.Setup(s => s.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => support);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var updated = ticketManager.AddSupportToTicket(supportId, ticketId);
        Assert.AreEqual(ticket.SupportId, supportId);
    }

    [Test]
    public async Task ShouldNotAddSupportToTicketIfTicketIsNotFound()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var supportId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = new Support
        {
            Id = supportId, Role = UserRole.Support,
            Email = "email@email.com", Name = "Name"
        };

        var ticket = (Ticket)null;
        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _supportRepository.Setup(s => s.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => support);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var error = Assert.ThrowsAsync<TicketNotFoundedException>(() =>
            ticketManager.AddSupportToTicket(supportId, ticketId));

        Assert.AreEqual(error.Message, "Ticket was not founded");
    }

    [Test]
    public async Task ShouldNotAddSupportToTicketIfSupportIsNotFound()
    {
        var ticketId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var supportId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId, Role = UserRole.Client,
            Email = "email@email.com", Name = "Name"
        };
        var support = (Support)null;

        var ticket = new Ticket
        {
            Id = ticketId, Title = "Title", TicketStatus = TicketStatus.New,
        };
        ticket.SetClient(client);
        _ticketRepository.Setup(t => t.GetOneAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => ticket);
        _supportRepository.Setup(s => s.GetOneByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => support);
        _ticketRepository.Setup(t => t.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(() => ticket);

        var ticketManager =
            new TicketManager(_clientRepository.Object, _ticketRepository.Object, _supportRepository.Object);

        var updated = ticketManager.AddSupportToTicket(supportId, ticketId);
        var error = Assert.ThrowsAsync<SupportNotFoundedException>(() =>
            ticketManager.AddSupportToTicket(supportId, ticketId));

        Assert.AreEqual(error.Message, "Support was not founded");
    }
}