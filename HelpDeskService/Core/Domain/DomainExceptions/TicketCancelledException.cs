namespace Domain.DomainExceptions;
public class TicketCancelledException : Exception
{
    public TicketCancelledException(string? message) : base(message)
    {
    }
}
