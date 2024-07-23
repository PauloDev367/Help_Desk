namespace Domain.DomainExceptions;

public class TicketAlreadyCancelledException : Exception
{
    public TicketAlreadyCancelledException(string? message) : base(message)
    {
    }
}