using Application.Auth.Request;
using Application.Dto;
using Application.Exceptions;
using Application.Support.Ports;
using Application.Support.Request;
using Application.Support.Response;
using Domain.Ports;

namespace Application.Support;
public class SupportManager : ISupportManager
{
    private readonly IAuthUserService _authUserService;
    private readonly ISupportRepository _supportRepository;

    public SupportManager(ISupportRepository supportRepository, IAuthUserService authUserService)
    {
        _supportRepository = supportRepository;
        _authUserService = authUserService;
    }

    public async Task<CreatedSupportResponse> CreateAsync(CreateSupportRequest request)
    {
        var response = new CreatedSupportResponse();
        var client = new Domain.Entities.Support
        {
            Email = request.Email,
            PasswordHash = request.Password,
            Role = Domain.Enums.UserRole.Support,
            Name = request.Name,
        };
        var registerUser = new RegisterUserRequest
        {
            Email = request.Email,
            Password = request.Password,
            Role = Domain.Enums.UserRole.Support,
        };
        var created = await _authUserService.RegisterAsync(registerUser);
        if (created.Errors.Count > 0)
            response.SetError(created.Errors);
        else
        {
            await _supportRepository.CreateAsync(client);
            response.Success = new SupportDto(client);
        }

        return response;
    }
    public async Task DeleteAsync(Guid supportId)
    {
        var authUser = await _authUserService.GetOneByIdAsync(supportId);
        if (authUser == null)
            throw new UserNotFoundedException("User was not founded!");
        
        var client = await _supportRepository.GetOneByEmailAsync(authUser.Email);
        if (client == null)
            throw new UserNotFoundedException("User was not founded!");

        await _authUserService.DeleteAsync(client);
        await _supportRepository.DeleteAsync(client);
    }
    public async Task<SupportDto> UpdateAsync(UpdateSupportRequest request, Guid supportId)
    {
        var client = await _authUserService.GetOneByIdAsync(supportId);
        if (client == null)
            throw new UserNotFoundedException("Support was not founded!");

        var systemClient = await _supportRepository.GetOneByEmailAsync(client.Email);
        if (systemClient == null)
            throw new UserNotFoundedException("Support was not founded!");
        
        var userAuthRequest = new UpdateAuthUserRequest { Email = request.Email };
        await _authUserService.UpdateAuthUserAsync(systemClient, userAuthRequest);

        systemClient.Email = string.IsNullOrEmpty(request.Email) ? systemClient.Email : request.Email;
        systemClient.Name = string.IsNullOrEmpty(request.Name) ? systemClient.Name : request.Name;

        await _supportRepository.UpdateAsync(systemClient);
        return new SupportDto(systemClient);
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
