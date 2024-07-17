namespace Domain.DomainExceptions;
public class SupportCannotCreateNewTicketException : Exception
{
    public SupportCannotCreateNewTicketException(string? message) : base(message)
    {
    }
}
