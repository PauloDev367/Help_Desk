namespace Domain.DomainExceptions;

public class TicketAlreadyFinishedException : Exception
{
    public TicketAlreadyFinishedException(string? message) : base(message)
    {
    }
}