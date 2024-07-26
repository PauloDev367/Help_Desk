namespace Application.Exceptions;

public class SupportNotFoundedException : Exception
{
    public SupportNotFoundedException(string? message) : base(message)
    {
    }
}