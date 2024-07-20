﻿using Application.Client.Ports;
using Application.Client.Request;
using Application.Client.Response;
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

    public async Task DeleteAsync(Guid clientId)
    {
        var client = await _clientRepository.GetOneByIdAsync(clientId);
        if (client == null)
            throw new UserNotFoundedException("User was not foundend!");

        await _clientRepository.DeleteAsync(client);
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
