namespace Domain.Entities;
public class Support : User
{
    public List<ClientSupport> ClientSupport { get; set; }
    public List<Ticket> Tickets { get; set; }
}
