using EmailSystem.Api.Models;

namespace EmailSystem.Api.Services;

public record EncryptedMessage(
    string CipherTextBase64,
    string EncryptedSymmetricKeyBase64,
    string NonceBase64,
    string TagBase64,
    string HashBase64,
    string SignatureBase64);

public record DecryptedMessage(string PlainText, bool HashValid, bool SignatureValid);

public interface ICryptoService
{
    (string PublicKeyPem, string PrivateKeyPem) GenerateRsaKeyPair();

    EncryptedMessage EncryptMessage(string plainText, string recipientPublicKeyPem, string senderPrivateKeyPem, out byte[] symmetricKey);

    DecryptedMessage DecryptMessage(Message message, string recipientPrivateKeyPem, string senderPublicKeyPem);
}

