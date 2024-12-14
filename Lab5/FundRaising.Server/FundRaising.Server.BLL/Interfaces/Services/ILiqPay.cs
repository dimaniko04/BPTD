namespace FundRaising.Server.BLL.Interfaces.Services;

public interface ILiqPay
{
    Task Payment(
        string description,
        string amount,
        string card,
        string cardExpiryDate,
        string cardCvv,
        string currency,
        Guid orderId,
        string action);
}