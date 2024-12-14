namespace FundRaising.Server.BLL.DTOs.Fundraiser;

public record PaymentDto(
    string Description,
    string Amount,
    string Card,
    string CardExpiryDate,
    string CardCvv);