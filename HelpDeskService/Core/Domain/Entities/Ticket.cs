using Domain.DomainExceptions;
using Domain.Enums;

namespace Domain.Entities;
public class Ticket
{
    public Guid Id { get; set; }
    public Guid? SupportId { get; private set; }
    public Support? Support { get; private set; }
    public Client Client { get; private set; }
    public Guid ClientId { get; private set; }
    public TicketStatus TicketStatus { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();

    public void SetSupport(Support support)
    {
        if (SupportId != null && SupportId != Guid.Empty)
            throw new TicketAlreadyHaveASupportException("This ticket already have a support attendant");
        else
        {
            SupportId = support.Id;
            Support = support;
        }
    }
    public void SetClient(Client client)
    {
        if (!client.Role.Equals(UserRole.Client))
        {
            throw new SupportCannotCreateNewTicketException("Supports cannot create new tickets, only users");
        }
        else
        {
            Client = client;
            ClientId = client.Id;
        }
    }
    public void AddComment(Comment comment, MessageAction action)
    {
        this.TicketStatus = (this.TicketStatus, action) switch
        {
            (TicketStatus.New, MessageAction.FromClient) => TicketStatus.Waiting_Support,
            (TicketStatus.New, MessageAction.FromSupport) => TicketStatus.Waiting_Client,
            (TicketStatus.Waiting_Client, MessageAction.FromSupport) => TicketStatus.Waiting_Client,
            (TicketStatus.Waiting_Client, MessageAction.FromClient) => TicketStatus.Waiting_Support,
            (TicketStatus.Cancelled, MessageAction.FromClient) => throw new TicketCancelledException("This ticket was cancelled"),
            (TicketStatus.Cancelled, MessageAction.FromSupport) => throw new TicketCancelledException("This ticket was cancelled"),
            (TicketStatus.Finished, MessageAction.FromClient) => throw new TicketFinishedException("This ticket was cancelled"),
            (TicketStatus.Finished, MessageAction.FromSupport) => throw new TicketFinishedException("This ticket was cancelled"),
            _ => this.TicketStatus,
        };
        this.Comments.Add(comment);
    }
}
