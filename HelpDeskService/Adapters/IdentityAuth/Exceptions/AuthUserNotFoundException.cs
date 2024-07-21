namespace IdentityAuth.Exceptions;
public class AuthUserNotFoundException : Exception
{
    public AuthUserNotFoundException(string? message) : base(message)
    {
    }
}
