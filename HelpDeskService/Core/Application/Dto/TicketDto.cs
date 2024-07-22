using Domain.Entities;

namespace Application.Dto;

public class TicketDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Domain.Entities.Support? Support { get; set; }
    public ClientDto Client { get; set; }
    public string TicketStatus { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();

    public TicketDto(Domain.Entities.Ticket ticket)
    {
        Id = ticket.Id;
        Title = ticket.Title;
        Support = ticket.Support;
        Client = new ClientDto(ticket.Client);
        TicketStatus = ticket.TicketStatus.ToString();
        Comments = ticket.Comments;
    }
}