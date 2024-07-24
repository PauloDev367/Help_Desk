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
}