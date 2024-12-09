using System.Text;

namespace HASH.Server.API.Util;

public static class HashUtil
{
    private const byte P = 31;

    private static async Task<byte[]> ReadFileBytes(IFormFile file)
    {
        byte[] fileBytes;
        
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
        }
        
        return fileBytes;
    }
    
    private static byte Hash(IEnumerable<byte> input, int bitSize)
    {
        if (bitSize != 2 && bitSize != 4 && bitSize != 8)
            throw new ArgumentException("Bit size must be 2, 4, or 8.");

        byte hash = 0;
        byte mask = (byte)((1 << bitSize) - 1);

        foreach (var b in input)
        {
            hash ^= b;
            hash = (byte)((hash << 3) | (hash >> 5));
        }

        return (byte)(hash & mask);
    }

    public static byte HashText(string input, int bitSize)
    {
        byte[] textBytes = Encoding.UTF8.GetBytes(input);
        return Hash(textBytes, bitSize);
    }

    public static async Task<byte> HashFile(IFormFile file, int bitSize)
    {
        byte[] fileBytes = await ReadFileBytes(file);
        return Hash(fileBytes, bitSize);
    }

    public static bool VerifyTextDigest(string input, byte digest, int bitSize)
    {
        byte inputDigest = HashText(input, bitSize);
        return inputDigest == digest;
    }

    public static async Task<bool> VerifyFileDigest(
        IFormFile file, byte digest, int bitSize)
    {
        byte inputDigest = await HashFile(file, bitSize);
        return inputDigest == digest;
    }

    public static async Task<byte[]?> GenerateFileCollision(
        IFormFile file, int bitSize)
    {
        var fileBytes = (await ReadFileBytes(file)).ToList();
        byte originalHash = Hash(fileBytes, bitSize);

        int attempts = 0;
        int maxAttempts = 100_000;

        while (attempts < maxAttempts)
        {
            fileBytes.Add((byte)' ');
            byte newHash = Hash(fileBytes, bitSize);

            if (newHash == originalHash)
            {
                return fileBytes.ToArray();
            }
            attempts++;
        }

        return null;
    }
}