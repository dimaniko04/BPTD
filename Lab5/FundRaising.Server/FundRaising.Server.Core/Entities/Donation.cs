namespace FundRaising.Server.Core.Entities;

public class Donation: BaseEntity
{
    public string? Description { get; set; }
    public long Amount { get; set; }
    public Guid UserId { get; set; }
    public Guid FundraiserId { get; set; }
    
    public User User { get; set; } = null!;
    public Fundraiser Fundraiser { get; set; } = null!;
}