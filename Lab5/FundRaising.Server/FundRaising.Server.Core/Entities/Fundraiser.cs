namespace FundRaising.Server.Core.Entities;

public class Fundraiser: BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long Goal { get; set; }
    public long AmountRaised { get; set; }
    public Guid UserId { get; set; }
    
    public User User { get; set; } = null!;
    public ICollection<Donation> Donations { get; set; } 
        = new List<Donation>();
}