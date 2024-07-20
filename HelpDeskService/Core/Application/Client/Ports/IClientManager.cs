﻿using Application.Client.Request;
using Application.Dto;

namespace Application.Client.Ports;
public interface IClientManager
{
    public Task<ClientDto> CreateAsync(CreateClientRequest request);
}