namespace FundRaising.Server.BLL.DTOs.Fundraiser;

public record CreateFundraiserDto(
    string Title,
    string Description,
    string Goal);