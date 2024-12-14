namespace FundRaising.Server.BLL.DTOs.Fundraiser;

public record UpdateFundraiserDto(
    string Title,
    string Description,
    string Goal);