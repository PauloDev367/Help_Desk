using Domain.Enums;

namespace Application.Dto;

public record ClientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public ClientDto()
    {

    }
    public ClientDto(Domain.Entities.Client client)
    {
        Id = client.Id;
        Name = client.Name;
        Email = client.Email;
        Role = client.Role.ToString();
    }
};