using Application.Dto;

namespace Application.Support.Response;
public class PaginatedSupportResponse
{
    public int PerPage { get; set; }
    public int Page { get; set; }
    public int TotalItems { get; set; }
    public List<SupportDto> Clients { get; set; } = new List<SupportDto>();
}
