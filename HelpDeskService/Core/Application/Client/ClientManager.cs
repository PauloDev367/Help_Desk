using Application.Client.Ports;
using Application.Client.Request;
using Application.Dto;
using Application.Exceptions;
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

    public async Task<ClientDto> UpdateAsync(UpdateClientRequest request, Guid clientId)
    {
        var client = await _clientRepository.GetOneByIdAsync(clientId);
        if (client == null)
            throw new UserNotFoundedException("User was not foundend!");

        client.Email = string.IsNullOrEmpty(request.Email) ? client.Email : request.Email;
        client.Name = string.IsNullOrEmpty(request.Name) ? client.Name : request.Name;

        await _clientRepository.UpdateAsync(client);
        return new ClientDto(client);
    }
}
