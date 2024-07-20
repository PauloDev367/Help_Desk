using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Client.Request;
public record CreateClientRequest
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
