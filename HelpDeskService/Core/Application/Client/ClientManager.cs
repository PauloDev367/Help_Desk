using Application.Auth.Request;
using Application.Client.Ports;
using Application.Client.Request;
using Application.Client.Response;
using Application.Dto;
using Application.Exceptions;
using Domain.Ports;

namespace Application.Client;
public class ClientManager : IClientManager
{
    private readonly IClientRepository _clientRepository;
    private readonly IAuthUserService _authUserService;

    public ClientManager(IClientRepository clientRepository, IAuthUserService authUserService)
    {
        _clientRepository = clientRepository;
        _authUserService = authUserService;
    }

    public async Task<CreatedClientResponse> CreateAsync(CreateClientRequest request)
    {
        var client = new Domain.Entities.Client
        {
            Email = request.Email,
            PasswordHash = request.Password,
            Role = Domain.Enums.UserRole.Client,
            Name = request.Name,
        };
        var response = new CreatedClientResponse();

        var registerUser = new RegisterUserRequest
        {
            Email = request.Email,
            Password = request.Password,
            Role = Domain.Enums.UserRole.Client,
        };
        var authUser = await _authUserService.RegisterAsync(registerUser);
        if (authUser.Errors.Count > 0)
            response.SetError(authUser.Errors);
        else
        {
            await _clientRepository.CreateAsync(client);
            response.Success = new ClientDto(client);
        }
        return response;
    }

    public async Task DeleteAsync(Guid clientId)
    {
        var client = await _clientRepository.GetOneByIdAsync(clientId);
        if (client == null)
            throw new UserNotFoundedException("User was not foundend!");

        await _authUserService.DeleteAsync(client);
        await _clientRepository.DeleteAsync(client);
    }

    public async Task<ClientDto> UpdateAsync(UpdateClientRequest request, Guid clientId)
    {
        var client = await _authUserService.GetOneByIdAsync(clientId);
        if (client == null)
            throw new UserNotFoundedException("User was not foundend!");

        var systemClient = await _clientRepository.GetOneByEmailAsync(client.Email);
        if (systemClient == null)
            throw new UserNotFoundedException("User was not foundend!");
        
        var userAuthRequest = new UpdateAuthUserRequest { Email = request.Email };
        await _authUserService.UpdateAuthUserAsync(systemClient, userAuthRequest);

        systemClient.Email = string.IsNullOrEmpty(request.Email) ? systemClient.Email : request.Email;
        systemClient.Name = string.IsNullOrEmpty(request.Name) ? systemClient.Name : request.Name;

        await _clientRepository.UpdateAsync(systemClient);
        return new ClientDto(systemClient);
    }
    public async Task<ClientDto> GetOneAsync(Guid clientId)
    {
        var client = await _clientRepository.GetOneByIdAsync(clientId);
        if (client == null)
            throw new UserNotFoundedException("User was not foundend!");

        return new ClientDto(client);
    }

    public async Task<PaginatedClientResponse> GetAllAsync(GetClientRequest request)
    {
        string[] orderParams = !string.IsNullOrEmpty(request.OrderBy) ? request.OrderBy.ToString().Split(",") : "id,desc".Split(",");
        var orderBy = orderParams[0];
        var order = orderParams[1];
        var data = await _clientRepository.GetAllAsync(
            request.PerPage, request.Page, orderBy, order
        );


        return new PaginatedClientResponse
        {
            Page = request.Page,
            PerPage = request.PerPage,
            TotalItems = data.Count,
            Clients = data.Select(u => new ClientDto(u)).ToList()
        };
    }
}
