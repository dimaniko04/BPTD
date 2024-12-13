namespace FundRaising.Server.Core.Entities;

public class User: BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    
    public ICollection<Donation> Donations { get; set; } 
        = new List<Donation>();
    public ICollection<Fundraiser> Fundraisers { get; set; }
        = new List<Fundraiser>();
}