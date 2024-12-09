using System.Collections.Concurrent;

namespace RSA.Server.API;

public class ClientKeyStore
{
    private static readonly ConcurrentDictionary<string, string> ClientKeys = new();

    public void AddOrUpdate(string clientIp, string publicKey)
    {
        ClientKeys.AddOrUpdate(clientIp, publicKey, (_, _) => publicKey);
    }
    
    public string? GetPublicKey(string clientIp)
    {
        ClientKeys.TryGetValue(clientIp, out var publicKey);
        return publicKey;
    }

    public bool Remove(string clientIp)
    {
        return ClientKeys.TryRemove(clientIp, out _);
    }
}