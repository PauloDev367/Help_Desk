using Domain.Entities;

namespace Application.Dto;

public class TicketWithoutCommentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public SupportDto? Support { get; set; }
    public ClientDto Client { get; set; }
    public string TicketStatus { get; set; }

    public TicketWithoutCommentDto(Domain.Entities.Ticket ticket)
    {
        Id = ticket.Id;
        Title = ticket.Title;
        Support = ticket.SupportId == null ? null:new SupportDto(ticket.Support);
        Client = new ClientDto(ticket.Client);
        TicketStatus = ticket.TicketStatus.ToString();
    }
}