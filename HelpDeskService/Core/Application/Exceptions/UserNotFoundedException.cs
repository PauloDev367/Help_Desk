namespace Application.Exceptions;
public class UserNotFoundedException : Exception
{
    public UserNotFoundedException(string? message) : base(message)
    {
    }
}
