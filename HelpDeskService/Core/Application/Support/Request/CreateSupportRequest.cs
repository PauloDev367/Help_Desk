using System.ComponentModel.DataAnnotations;

namespace Application.Support.Request;
public record CreateSupportRequest
{
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
