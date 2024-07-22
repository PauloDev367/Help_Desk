using Domain.Enums;

namespace Application.Ticket.Request;

public class CreateTicketRequest
{
    public Guid ClientId { get; private set; }
    public string Title { get; set; }
    public void SetClientId(Guid id) => ClientId = id;
}