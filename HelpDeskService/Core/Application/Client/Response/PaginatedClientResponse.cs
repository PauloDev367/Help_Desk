using Application.Dto;

namespace Application.Client.Response;
public class PaginatedClientResponse
{
    public int PerPage { get; set; }
    public int Page { get; set; }
    public int TotalItems { get; set; }
    public List<ClientDto> Clients { get; set; } = new List<ClientDto>();

}
