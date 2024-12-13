namespace FundRaising.Server.BLL.Interfaces.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool VerifyHash(string password, string passwordHash);
}