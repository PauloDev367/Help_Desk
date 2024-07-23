using Domain.Enums;

namespace Application.Ticket.Request;

public class AddCommentToTicketRequest
{
    public Guid TicketId { get; set; }
    public Guid ClientId { get; set; }
    public Guid SupportId { get; set; }
    public string Message { get; set; }
    public TicketAction From { get; set; }

    public AddCommentToTicketRequest(Guid ticketId, string message, TicketAction from, Guid? clientId, Guid? supportId)
    {
        if (TicketAction.FromSupport.Equals(from) && supportId.Equals(null))
        {
            throw new ArgumentException("You need to pass the support id");
        }
        if (TicketAction.FromClient.Equals(from) && clientId.Equals(null))
        {
            throw new ArgumentException("You need to pass the client id");
        }
        TicketId = ticketId;
        ClientId = clientId ?? Guid.NewGuid();
        SupportId = supportId ?? Guid.NewGuid();
        Message = message;
        From = from;
    }

    public override string ToString()
    {
        return $"{TicketId} - {ClientId} - {SupportId} - {Message} - {From}";
    }
}