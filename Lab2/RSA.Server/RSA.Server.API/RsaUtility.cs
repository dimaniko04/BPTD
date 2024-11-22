using System.Security.Cryptography;
using System.Text;

namespace RSA.Server.API;

public class RsaUtility
{
    private static readonly System.Security.Cryptography.RSA Rsa;

    static RsaUtility()
    {
        Rsa = System.Security.Cryptography.RSA.Create(2048);
    }

    public string GetPublicKey()
    {
        return Convert.ToBase64String(Rsa.ExportRSAPublicKey());
    }

    public string Decrypt(string encryptedData)
    {
        var decryptedBytes = Rsa.Decrypt(Convert.FromBase64String(encryptedData), RSAEncryptionPadding.Pkcs1);
        return System.Text.Encoding.UTF8.GetString(decryptedBytes);
    }

    public string Encrypt(string data, string publicKey)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKey);
        
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(data), RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(encryptedBytes);
    }
}