using Application.Client.Request;
using Application.Client.Response;
using Application.Dto;
using Application.Support.Request;
using Application.Support.Response;

namespace Application.Support.Ports;
public interface ISupportManager
{
    public Task<SupportDto> CreateAsync(CreateSupportRequest request);
    public Task<SupportDto> UpdateAsync(UpdateSupportRequest request, Guid supportId);
    public Task DeleteAsync(Guid supportId);
    public Task<SupportDto> GetOneAsync(Guid supportId);
    public Task<PaginatedSupportResponse> GetAllAsync(GetSupportRequest request);
}
