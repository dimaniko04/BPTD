﻿using System.Security.Cryptography;
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
        return Convert.ToBase64String(Rsa.ExportSubjectPublicKeyInfo());
    }

    public string Decrypt(string encryptedData)
    {
        var decryptedBytes = Rsa.Decrypt(
            Convert.FromBase64String(encryptedData), 
            RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public string Encrypt(string data, string publicKey)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKey);
        
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var encryptedBytes = rsa.Encrypt(
            Encoding.UTF8.GetBytes(data), 
            RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(encryptedBytes);
    }
}