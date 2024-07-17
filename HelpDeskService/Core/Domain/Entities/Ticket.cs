using Domain.Enums;

namespace Domain.Entities;
public class Ticket
{
    public Guid Id { get; set; }
    public Guid SupportId { get; set; }
    public Support? Support { get; set; }
    public Client Client { get; set; }
    public Guid ClientId { get; set; }
    public TicketStatus TicketStatus { get; set; }
    public List<Comment> Comments { get; set; }
}
