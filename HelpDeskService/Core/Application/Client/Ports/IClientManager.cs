using Application.Client.Request;
using Application.Dto;

namespace Application.Client.Ports;
public interface IClientManager
{
    public Task<ClientDto> CreateAsync(CreateClientRequest request);
    public Task<ClientDto> UpdateAsync(UpdateClientRequest request, Guid clientId);
    public Task DeleteAsync(Guid clientId);
    public Task<ClientDto> GetOneAsync(Guid clientId);

}
