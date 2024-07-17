namespace Domain.DomainExceptions;
public class TicketFinishedException : Exception
{
    public TicketFinishedException(string? message) : base(message)
    {
    }
}
