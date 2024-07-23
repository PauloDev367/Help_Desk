namespace Application.Exceptions;

public class TicketNotFoundedException : Exception
{
    public TicketNotFoundedException(string? message) : base(message)
    {
    }
}