using System.Security.Cryptography;

namespace HASH.Server.API.Util;

public static class DigitalSignatureUtil
{
    public static (string ds, string publicKey) SignDigest(byte[] digest)
    {
        using var rsa = RSA.Create(2048);

        var ds = rsa.Encrypt(digest, RSAEncryptionPadding.OaepSHA256);
        var publicKey = rsa.ExportRSAPrivateKey();
        
        return (Convert.ToBase64String(ds), 
            Convert.ToBase64String(publicKey));
    }

    public static bool VerifySignature(
        string ds,
        string publicKey,
        byte[] digest)
    {
        var dsBytes = Convert.FromBase64String(ds);
        var publicKeyBytes = Convert.FromBase64String(publicKey);
        
        using var rsa = RSA.Create(2048);
        rsa.ImportRSAPrivateKey(publicKeyBytes, out _);

        try
        {
            var decryptedDigest = rsa.Decrypt(dsBytes,
                RSAEncryptionPadding.OaepSHA256);
            
            return decryptedDigest.SequenceEqual(digest);
        }
        catch (CryptographicException ex)
        {
            return false;
        }
    }
}