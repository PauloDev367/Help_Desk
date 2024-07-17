namespace Domain.DomainExceptions;
public class TicketAlreadyHaveASupportException : Exception
{
    public TicketAlreadyHaveASupportException(string? message) : base(message)
    {
    }
}
