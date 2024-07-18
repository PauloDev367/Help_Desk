using Domain.DomainExceptions;

namespace Domain.Entities;
public class Comment
{
    public Guid Id { get; set; }
    public string Text
    {
        get { return Text; }
        set
        {
            Text = value;
            Validate();

        }
    }
    public bool IsClientComment { get; set; }
    public Guid? SupportId { get; set; }
    public Support? Support { get; set; }
    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }
    public void Validate()
    {
        if (IsClientComment && (ClientId == null || Client == null))
        {
            throw new InvalidCommentException("Client must be specified for client comments.");
        }

        if (!IsClientComment && (SupportId == null || Support == null))
        {
            throw new InvalidCommentException("Support must be specified for support comments.");
        }
    }
}
