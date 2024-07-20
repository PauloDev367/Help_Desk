using System.ComponentModel.DataAnnotations;

namespace Application.Client.Request;
public class UpdateClientRequest
{
    [MinLength(3)]
    public string? Name { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
}
