using Domain.Enums;

namespace Application.Ticket.Request;

public class CreateTicketRequest
{
    public Guid ClientId { get; private set; }
    public string Title { get; set; }
    public TicketStatus TicketStatus { get; private set; } = TicketStatus.New;
    public void SetClientId(Guid id) => ClientId = id;
}