namespace Application.Ticket.Request;

public class GetAllTicketsRequest
{
    private int _perPage { get; set; } = 10;
    private int _page { get; set; } = 0;

    public int PerPage
    {
        get { return _perPage; }
        set
        {
            if (value < 10)
            {
                _perPage = 10;
            }
            else if (value > 50)
            {
                _perPage = 50;
            }
            else
            {
                _perPage = value;
            }

        }
    }
    public int Page
    {
        get { return _page; }
        set { _page = value; }
    }
    public string? OrderBy { get; set; }
}