namespace EmailSystem.Api.Contracts.Messages;

public class InboxMessageResponse
{
    public Guid MessageId { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime ReceivedAtUtc { get; set; }
    public bool IntegrityValid { get; set; }
    public bool SignatureValid { get; set; }
}

