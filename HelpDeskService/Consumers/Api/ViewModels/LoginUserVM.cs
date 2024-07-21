using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels;

public record LoginUserVM
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
