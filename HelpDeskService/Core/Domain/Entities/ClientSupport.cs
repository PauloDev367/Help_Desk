
namespace Domain.Entities;
public class ClientSupport
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; }
    public Guid SupportId { get; set; }
    public Support Support { get; set; }
}
