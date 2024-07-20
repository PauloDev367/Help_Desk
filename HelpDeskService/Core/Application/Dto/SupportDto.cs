namespace Application.Dto;

public record SupportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public SupportDto()
    {

    }
    public SupportDto(Domain.Entities.Support support)
    {
        Id = support.Id;
        Name = support.Name;
        Email = support.Email;
        Role = support.Role.ToString();
    }
};