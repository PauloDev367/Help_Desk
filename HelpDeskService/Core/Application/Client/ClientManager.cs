using Application.Client.Ports;
using Application.Client.Request;
using Application.Dto;
using Domain.Ports;

namespace Application.Client;
public class ClientManager : IClientManager
{
    private readonly IClientRepository _clientRepository;

    public ClientManager(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ClientDto> CreateAsync(CreateClientRequest request)
    {
        var client = new Domain.Entities.Client
        {
            Email = request.Email,
            PasswordHash = request.Password,
            Role = Domain.Enums.UserRole.Client,
            Name = request.Name,
        };

        await _clientRepository.CreateAsync(client);
        return new ClientDto(client);   
    }
}
