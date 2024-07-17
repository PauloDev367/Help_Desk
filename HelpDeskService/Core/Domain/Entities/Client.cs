namespace Domain.Entities;
public class Client : User
{
    public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    
}
