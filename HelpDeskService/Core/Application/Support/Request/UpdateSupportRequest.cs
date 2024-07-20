using System.ComponentModel.DataAnnotations;

namespace Application.Support.Request;
public class UpdateSupportRequest
{
    [MinLength(3)]
    public string? Name { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
}
