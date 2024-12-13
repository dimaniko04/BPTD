namespace FundRaising.Server.BLL.DTOs.Fundraiser;

public record FundraiserDto(
    Guid Id,
    string Title,
    string Description,
    long Goal,
    long AmountRaised);
