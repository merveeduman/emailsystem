using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmailSystem.Api.Contracts.Messages;
using EmailSystem.Api.Data;
using EmailSystem.Api.Models;
using EmailSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailSystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MessagesController(
    EmailDbContext dbContext,
    ICryptoService cryptoService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendMessage(SendMessageRequest request)
    {
        var sender = await GetCurrentUserAsync();
        if (sender is null) return Unauthorized();

        var recipient = await dbContext.Users.SingleOrDefaultAsync(u => u.Username == request.RecipientUsername);
        if (recipient is null)
        {
            return NotFound(new { message = "Recipient not found" });
        }

        var encrypted = cryptoService.EncryptMessage(
            request.Body,
            recipient.PublicKeyPem,
            sender.PrivateKeyPem,
            out _);

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Subject = request.Subject,
            CipherTextBase64 = encrypted.CipherTextBase64,
            EncryptedSymmetricKeyBase64 = encrypted.EncryptedSymmetricKeyBase64,
            NonceBase64 = encrypted.NonceBase64,
            TagBase64 = encrypted.TagBase64,
            HashBase64 = encrypted.HashBase64,
            SignatureBase64 = encrypted.SignatureBase64
        };

        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync();

        return Ok(new { message = "Message sent securely" });
    }

    [HttpGet("inbox")]
    public async Task<IActionResult> GetInbox()
    {
        var user = await GetCurrentUserAsync();
        if (user is null) return Unauthorized();

        var messages = await dbContext.Messages
            .Include(m => m.Sender)
            .Where(m => m.RecipientId == user.Id)
            .OrderByDescending(m => m.CreatedAtUtc)
            .ToListAsync();

        var response = new List<InboxMessageResponse>();
        foreach (var message in messages)
        {
            var decrypted = cryptoService.DecryptMessage(message, user.PrivateKeyPem, message.Sender!.PublicKeyPem);
            response.Add(new InboxMessageResponse
            {
                MessageId = message.Id,
                Sender = message.Sender!.Username,
                Subject = message.Subject,
                Body = decrypted.PlainText,
                ReceivedAtUtc = message.CreatedAtUtc,
                IntegrityValid = decrypted.HashValid,
                SignatureValid = decrypted.SignatureValid
            });
        }

        return Ok(response);
    }

    private async Task<User?> GetCurrentUserAsync()
{
    // Hem Sub hem de NameIdentifier kontrolü yaparak riski sıfırlayalım
    var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub) 
                       ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (Guid.TryParse(userIdString, out var userId))
    {
        return await dbContext.Users.FindAsync(userId);
    }

    return null;
}
}

