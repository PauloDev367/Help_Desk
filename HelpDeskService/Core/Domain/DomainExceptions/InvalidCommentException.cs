namespace Domain.DomainExceptions;
public class InvalidCommentException : Exception
{
    public InvalidCommentException(string? message) : base(message)
    {
    }
}
