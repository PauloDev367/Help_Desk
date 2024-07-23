using Domain.DomainExceptions;
using Domain.Enums;

namespace Domain.Entities;
public class Ticket
{
    public Guid Id { get; set; }
    public string Title { get; set; }
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
            ClientId = client.Id;
        }
    }
    public void AddComment(Comment comment, TicketAction action)
    {
        this.TicketStatus = (this.TicketStatus, action) switch
        {
            (TicketStatus.New, TicketAction.FromClient) => TicketStatus.Waiting_Support,
            (TicketStatus.New, TicketAction.FromSupport) => TicketStatus.Waiting_Client,
            (TicketStatus.Waiting_Client, TicketAction.FromSupport) => TicketStatus.Waiting_Client,
            (TicketStatus.Waiting_Client, TicketAction.FromClient) => TicketStatus.Waiting_Support,
            (TicketStatus.Cancelled, TicketAction.FromClient) => throw new TicketCancelledException("This ticket was cancelled"),
            (TicketStatus.Cancelled, TicketAction.FromSupport) => throw new TicketCancelledException("This ticket was cancelled"),
            (TicketStatus.Finished, TicketAction.FromClient) => throw new TicketFinishedException("This ticket was cancelled"),
            (TicketStatus.Finished, TicketAction.FromSupport) => throw new TicketFinishedException("This ticket was cancelled"),
            _ => this.TicketStatus,
        };
        this.Comments.Add(comment);
    }

    public void FinishTicket(TicketAction action)
    {
        this.TicketStatus = (this.TicketStatus, action) switch
        {
            (TicketStatus.Finished, TicketAction.FromClient) => throw new TicketAlreadyFinishedException("The ticket was already finished"),
            (TicketStatus.Finished, TicketAction.FromSupport) => throw new TicketAlreadyFinishedException("The ticket was already finished"),
            (TicketStatus.Cancelled, TicketAction.FromClient) => throw new TicketAlreadyCancelledException("The ticket was already cancelled"),
            (TicketStatus.Cancelled, TicketAction.FromSupport) => throw new TicketAlreadyCancelledException("The ticket was already cancelled"),
            _=> TicketStatus.Finished
        };
    }
    public void CancelTicket(TicketAction action)
    {
        this.TicketStatus = (this.TicketStatus, action) switch
        {
            (TicketStatus.Finished, TicketAction.FromClient) => throw new TicketAlreadyFinishedException("The ticket was already finished"),
            (TicketStatus.Finished, TicketAction.FromSupport) => throw new TicketAlreadyFinishedException("The ticket was already finished"),
            (TicketStatus.Cancelled, TicketAction.FromClient) => throw new TicketAlreadyCancelledException("The ticket was already cancelled"),
            (TicketStatus.Cancelled, TicketAction.FromSupport) => throw new TicketAlreadyCancelledException("The ticket was already cancelled"),
            _=> TicketStatus.Cancelled
        };
    }
}
