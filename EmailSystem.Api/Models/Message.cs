using System.ComponentModel.DataAnnotations;

namespace EmailSystem.Api.Models;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid SenderId { get; set; }

    public User? Sender { get; set; }

    [Required]
    public Guid RecipientId { get; set; }

    public User? Recipient { get; set; }

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string CipherTextBase64 { get; set; } = string.Empty;

    [Required]
    public string EncryptedSymmetricKeyBase64 { get; set; } = string.Empty;

    [Required]
    public string NonceBase64 { get; set; } = string.Empty;

    [Required]
    public string TagBase64 { get; set; } = string.Empty;

    [Required]
    public string HashBase64 { get; set; } = string.Empty;

    [Required]
    public string SignatureBase64 { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

