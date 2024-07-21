using Application.Dto;

namespace Application.Support.Response;
public class CreatedSupportResponse
{
    public SupportDto Success { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public void AddError(string error) => Errors.Add(error);
    public void SetError(List<string> errors) => Errors = errors;
}
