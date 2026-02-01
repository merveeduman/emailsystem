using System.Security.Cryptography;
using System.Text;
using EmailSystem.Api.Models;

namespace EmailSystem.Api.Services;

public class CryptoService : ICryptoService
{
    public (string PublicKeyPem, string PrivateKeyPem) GenerateRsaKeyPair()
    {
        using var rsa = RSA.Create(2048);
        var privateKey = rsa.ExportRSAPrivateKeyPem();
        var publicKey = rsa.ExportRSAPublicKeyPem();
        return (publicKey, privateKey);
    }

    public EncryptedMessage EncryptMessage(string plainText, string recipientPublicKeyPem, string senderPrivateKeyPem, out byte[] symmetricKey)
    {
        symmetricKey = RandomNumberGenerator.GetBytes(32); // AES-256
        var nonce = RandomNumberGenerator.GetBytes(12); // AES-GCM nonce length
        var plaintextBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = new byte[plaintextBytes.Length];
        var tag = new byte[16];

        using var aesGcm = new AesGcm(symmetricKey, tagSizeInBytes: 16);
        aesGcm.Encrypt(nonce, plaintextBytes, cipherBytes, tag);

        var hashBytes = SHA256.HashData(plaintextBytes);

        using var senderRsa = RSA.Create();
        senderRsa.ImportFromPem(senderPrivateKeyPem);
        var signature = senderRsa.SignData(hashBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        using var recipientRsa = RSA.Create();
        recipientRsa.ImportFromPem(recipientPublicKeyPem);
        var encryptedKey = recipientRsa.Encrypt(symmetricKey, RSAEncryptionPadding.OaepSHA256);

        return new EncryptedMessage(
            Convert.ToBase64String(cipherBytes),
            Convert.ToBase64String(encryptedKey),
            Convert.ToBase64String(nonce),
            Convert.ToBase64String(tag),
            Convert.ToBase64String(hashBytes),
            Convert.ToBase64String(signature));
    }

    public DecryptedMessage DecryptMessage(Message message, string recipientPrivateKeyPem, string senderPublicKeyPem)
    {
        using var recipientRsa = RSA.Create();
        recipientRsa.ImportFromPem(recipientPrivateKeyPem);
        var symmetricKey = recipientRsa.Decrypt(Convert.FromBase64String(message.EncryptedSymmetricKeyBase64), RSAEncryptionPadding.OaepSHA256);

        var nonce = Convert.FromBase64String(message.NonceBase64);
        var cipherBytes = Convert.FromBase64String(message.CipherTextBase64);
        var tag = Convert.FromBase64String(message.TagBase64);
        var plainBytes = new byte[cipherBytes.Length];

        using var aesGcm = new AesGcm(symmetricKey, tagSizeInBytes: 16);
        aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes, null);

        var hashBytes = SHA256.HashData(plainBytes);
        var storedHash = Convert.FromBase64String(message.HashBase64);
        var hashValid = CryptographicOperations.FixedTimeEquals(hashBytes, storedHash);

        using var senderRsa = RSA.Create();
        senderRsa.ImportFromPem(senderPublicKeyPem);
        var signatureValid = senderRsa.VerifyData(
            storedHash,
            Convert.FromBase64String(message.SignatureBase64),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pss);

        return new DecryptedMessage(
            Encoding.UTF8.GetString(plainBytes),
            hashValid,
            signatureValid);
    }
}

