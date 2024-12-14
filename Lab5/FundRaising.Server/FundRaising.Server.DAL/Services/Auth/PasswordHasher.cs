using System.Security.Cryptography;
using FundRaising.Server.BLL.Interfaces.Services;

namespace FundRaising.Server.DAL.Services.Auth;

public class PasswordHasher: IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    
    private readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA512;
    
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt,
            Iterations, _algorithm, 
            HashSize);
        
        return $"{Convert.ToBase64String(salt)}-{Convert.ToBase64String(hash)}";
    }

    public bool VerifyHash(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split('-');
        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);
        
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt,
            Iterations, _algorithm,
            HashSize);
        
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}