using Domain.Enums;

namespace Domain.Entities;
public abstract class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Comment> Comments { get; set; } = new List<Comment>();
}
