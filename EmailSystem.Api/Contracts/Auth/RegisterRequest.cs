using System.ComponentModel.DataAnnotations;

namespace EmailSystem.Api.Contracts.Auth;

public class RegisterRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}

