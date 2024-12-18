using FundRaising.Server.BLL.Interfaces.Services;

namespace FundRaising.Server.DAL.Services.DateTimeProvider;

public class DateTimeProvider: IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}