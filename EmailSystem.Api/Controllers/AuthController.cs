using EmailSystem.Api.Contracts.Auth;
using EmailSystem.Api.Data;
using EmailSystem.Api.Models;
using EmailSystem.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    EmailDbContext dbContext,
    PasswordHasher<User> passwordHasher,
    ICryptoService cryptoService,
    TokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await dbContext.Users.AnyAsync(u => u.Username == request.Username))
        {
            return Conflict(new { message = "Username already exists" });
        }

        var keys = cryptoService.GenerateRsaKeyPair();
        var user = new User
        {
            Username = request.Username,
            PublicKeyPem = keys.PublicKeyPem,
            PrivateKeyPem = keys.PrivateKeyPem
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return Created(string.Empty, new { user.Username, user.PublicKeyPem });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var verifyResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = tokenService.CreateToken(user);
        return Ok(new
        {
            token,
            user.PublicKeyPem
        });
    }
}

