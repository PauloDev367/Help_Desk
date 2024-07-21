namespace Application.Auth.Response;
public class LoggedUserResponse
{
    public string Token { get; set; }
    public DateTime ExpirationTime { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public void AddError(string erro) => Errors.Add(erro);
    public void SetError(List<string> errors) => Errors = errors;
}
