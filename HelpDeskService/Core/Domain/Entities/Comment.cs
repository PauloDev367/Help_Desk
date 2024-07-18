namespace Domain.Entities;
public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsClientComment { get; set; }
    //verificar se caso o client não existir, informar o support, o msm para o contrario
    public Guid? SupportId { get; set; }
    public Support? Support { get; set; }
    public Client? Client { get; set; }
    public Guid? ClientId { get; set; }
}
