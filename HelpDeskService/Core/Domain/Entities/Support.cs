namespace Domain.Entities;
public class Support : User
{
    public List<Client> Clients { get; set; }
}
