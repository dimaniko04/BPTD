using System.Security.Cryptography;

namespace RSA.Server.API;

public static class HybridEncryptionUtility
{
    public static (
        byte[] ecnryptedData,
        byte[] aesKey,
        byte[] iv) Encrypt(byte[] data, string rsaPublicKey)
    {
        var (encryptedData, key, iv) = EncryptWithAes(data);

        var (encryptedAesKey, 
                encryptedIv) = EncryptAesKeyWithRsa(key, iv, rsaPublicKey);
        
        return (encryptedData, encryptedAesKey, encryptedIv);
    }

    private static (
        byte[] EncryptedData, 
        byte[] Key, 
        byte[] Iv) EncryptWithAes(byte[] data)
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        return (memoryStream.ToArray(), aes.Key, aes.IV);
    }
    
    private static (byte[], byte[]) EncryptAesKeyWithRsa(
        byte[] key, byte[] iv, string rsaPublicKey)
    {
        var publicKeyBytes = Convert.FromBase64String(rsaPublicKey);
        
        using var rsa = System.Security.Cryptography.RSA.Create(2048);

        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var encryptedKey = rsa.Encrypt(key, RSAEncryptionPadding.OaepSHA256);
        var encryptedIv = rsa.Encrypt(iv, RSAEncryptionPadding.OaepSHA256);
        
        return (encryptedKey, encryptedIv);
    }
}