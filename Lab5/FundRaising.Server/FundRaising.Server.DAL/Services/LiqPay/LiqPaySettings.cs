namespace FundRaising.Server.DAL.Services.LiqPay;

public class LiqPaySettings
{
    public const string SectionName = "LiqPay";
    
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
}