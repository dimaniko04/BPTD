using System.Security.Cryptography;

namespace RSA.Server.API;

public static class HybridEncryptionUtility
{
    public static byte[] Encrypt(byte[] data, string rsaPublicKey)
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();

        var (encryptedData, key, iv) = EncryptWithAes(data);

        var encryptedAesKey = EncryptAesKeyWithRsa(key, iv, rsaPublicKey);
        
        var combinedDataAndKey = 
            new byte[encryptedData.Length + encryptedAesKey.Length];
        Buffer.BlockCopy(encryptedData, 0, 
            combinedDataAndKey, 0, 
            encryptedData.Length);
        Buffer.BlockCopy(encryptedAesKey, 0, 
            combinedDataAndKey, encryptedData.Length, 
            encryptedAesKey.Length);
        
        return combinedDataAndKey;
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
    
    private static byte[] EncryptAesKeyWithRsa(
        byte[] key, byte[] iv, string rsaPublicKey)
    {
        var publicKeyBytes = Convert.FromBase64String(rsaPublicKey);
        
        var combinedKeyAndIv = new byte[key.Length + iv.Length];
        Buffer.BlockCopy(key, 0, combinedKeyAndIv, 0, key.Length);
        Buffer.BlockCopy(iv, 0, combinedKeyAndIv, key.Length, iv.Length);
        
        using var rsa = System.Security.Cryptography.RSA.Create();

        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        return rsa.Encrypt(combinedKeyAndIv, RSAEncryptionPadding.Pkcs1);
    }
}