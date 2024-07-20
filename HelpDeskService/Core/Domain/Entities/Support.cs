namespace Domain.Entities;
public class Support : User
{
    public List<ClientSupport> ClientSupport { get; set; } = new List<ClientSupport>();
    public List<Ticket> Tickets { get; set; } = new List<Ticket>();
}
