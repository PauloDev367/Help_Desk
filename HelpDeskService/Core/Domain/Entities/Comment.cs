namespace Domain.Entities;
public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsClientComment { get; set; }
    private Guid? supportId;
    private Guid? clientId;
    public Guid? SupportId
    {
        get => supportId;
        set
        {
            supportId = value;
            Validate();
        }
    }
    public Support? Support { get; set; }
    public Guid? ClientId
    {
        get => clientId;
        set
        {
            clientId = value;
            Validate();
        }
    }
    public Client? Client { get; set; }
    public void Validate()
    {
        if (IsClientComment && (ClientId == null || Client == null))
        {
            throw new InvalidOperationException("Client must be specified for client comments.");
        }

        if (!IsClientComment && (SupportId == null || Support == null))
        {
            throw new InvalidOperationException("Support must be specified for support comments.");
        }
    }
}
