using Application.Dto;
using Application.Exceptions;
using Application.Support.Ports;
using Application.Support.Request;
using Application.Support.Response;
using Domain.Ports;

namespace Application.Support;
public class SupportManager : ISupportManager
{
    private readonly ISupportRepository _supportRepository;

    public SupportManager(ISupportRepository supportRepository)
    {
        _supportRepository = supportRepository;
    }

    public async Task<SupportDto> CreateAsync(CreateSupportRequest request)
    {
        var client = new Domain.Entities.Support
        {
            Email = request.Email,
            PasswordHash = request.Password,
            Role = Domain.Enums.UserRole.Support,
            Name = request.Name,
        };

        await _supportRepository.CreateAsync(client);
        return new SupportDto(client);
    }
    public async Task DeleteAsync(Guid supportId)
    {
        var client = await _supportRepository.GetOneByIdAsync(supportId);
        if (client == null)
            throw new UserNotFoundedException("User was not foundend!");

        await _supportRepository.DeleteAsync(client);
    }
    public async Task<SupportDto> UpdateAsync(UpdateSupportRequest request, Guid supportId)
    {
        var client = await _supportRepository.GetOneByIdAsync(supportId);
        if (client == null)
            throw new UserNotFoundedException("User was not foundend!");

        client.Email = string.IsNullOrEmpty(request.Email) ? client.Email : request.Email;
        client.Name = string.IsNullOrEmpty(request.Name) ? client.Name : request.Name;

        await _supportRepository.UpdateAsync(client);
        return new SupportDto(client);
    }
    public async Task<SupportDto> GetOneAsync(Guid supportId)
    {
        var client = await _supportRepository.GetOneByIdAsync(supportId);
        if (client == null)
            throw new UserNotFoundedException("User was not foundend!");

        return new SupportDto(client);
    }
    public async Task<PaginatedSupportResponse> GetAllAsync(GetSupportRequest request)
    {
        string[] orderParams = !string.IsNullOrEmpty(request.OrderBy) ? request.OrderBy.ToString().Split(",") : "id,desc".Split(",");
        var orderBy = orderParams[0];
        var order = orderParams[1];
        var data = await _supportRepository.GetAllAsync(
            request.PerPage, request.Page, orderBy, order
        );


        return new PaginatedSupportResponse
        {
            Page = request.Page,
            PerPage = request.PerPage,
            TotalItems = data.Count,
            Clients = data.Select(u => new SupportDto(u)).ToList()
        };
    }
}
