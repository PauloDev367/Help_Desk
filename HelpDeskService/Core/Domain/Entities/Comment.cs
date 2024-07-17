namespace Domain.Entities;
public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsClientComment { get; set; }
    public Guid SupportId { get; set; }
    public Support? Support { get; set; }
    public Client Client { get; set; }
    public Guid ClientId { get; set; }
}
