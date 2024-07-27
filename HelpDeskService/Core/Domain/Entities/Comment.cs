using Domain.DomainExceptions;

namespace Domain.Entities;
public class Comment
{
    public Guid Id { get; set; }
    public bool IsClientComment { get; set; }
    public Guid? SupportId { get; set; }
    public Support? Support { get; set; }
    public Guid ClientId { get; set; }
    public Client? Client { get; set; }
    private string _text;
    public string Text
    {
        get { return _text; }
        set
        {
            _text = value;
            Validate();

        }
    }
    public void Validate()
    {
        if (IsClientComment && (ClientId == null))
        {
            throw new InvalidCommentException("Client must be specified for client comments.");
        }

        if (!IsClientComment && (SupportId == null))
        {
            throw new InvalidCommentException("Support must be specified for support comments.");
        }
    }
}
