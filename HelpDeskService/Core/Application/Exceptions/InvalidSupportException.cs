namespace Application.Exceptions;

public class InvalidSupportException : Exception
{
    public InvalidSupportException(string? message) : base(message)
    {
    }
}