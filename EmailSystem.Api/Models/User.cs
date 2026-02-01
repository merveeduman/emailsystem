using System.ComponentModel.DataAnnotations;

namespace EmailSystem.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string PublicKeyPem { get; set; } = string.Empty;

    // For demo purposes the private key is stored server-side; in a production
    // system it should be protected with an HSM or client-side key handling.
    [Required]
    public string PrivateKeyPem { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<Message> SentMessages { get; set; } = [];
    public List<Message> ReceivedMessages { get; set; } = [];
}

