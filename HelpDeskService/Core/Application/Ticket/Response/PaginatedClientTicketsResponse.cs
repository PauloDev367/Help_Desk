using Application.Dto;

namespace Application.Ticket.Response;

public class PaginatedClientTicketsResponse
{
    public int PerPage { get; set; }
    public int Page { get; set; }
    public int TotalItems { get; set; }
    public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
}