using System.ComponentModel.DataAnnotations;

namespace EmailSystem.Api.Contracts.Messages;

public class SendMessageRequest
{
    [Required]
    public string RecipientUsername { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Body { get; set; } = string.Empty;
}

