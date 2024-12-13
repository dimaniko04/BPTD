namespace FundRaising.Server.BLL.Interfaces.Services;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}