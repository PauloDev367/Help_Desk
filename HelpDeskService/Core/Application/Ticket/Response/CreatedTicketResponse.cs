using Application.Dto;

namespace Application.Ticket.Response;

public class CreatedTicketResponse
{
    public Guid Id { get; set; }
    public ClientDto ClientDto { get; set; }
    public string Title { get; set; }
    public string TicketStatus { get; set; }
}