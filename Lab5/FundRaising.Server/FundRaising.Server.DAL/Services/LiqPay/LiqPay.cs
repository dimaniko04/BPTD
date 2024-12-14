using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FundRaising.Server.BLL.Exceptions.Fundraiser;
using FundRaising.Server.BLL.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace FundRaising.Server.DAL.Services.LiqPay;

public class LiqPay: ILiqPay
{
    private const string LiqPayUrl = "https://www.liqpay.ua/api/request";
    
    private readonly LiqPaySettings _liqPaySettings;
    
    public LiqPay(IOptions<LiqPaySettings> liqPayOptions)
    {
        _liqPaySettings = liqPayOptions.Value;
    }
    
    public async Task Payment(
        string description,
        string amount,
        string card,
        string cardExpiryDate,
        string cardCvv,
        string currency,
        Guid orderId,
        string action)
    {
        var publicKey = _liqPaySettings.PublicKey;
        var privateKey = _liqPaySettings.PrivateKey;
        var parts = Strings.Split(cardExpiryDate, "/");
        
        var jsonString = JsonSerializer.Serialize(new
        {
            public_key = publicKey,
            version = "3",
            action,
            amount, 
            currency,
            description,
            order_id = orderId.ToString(),
            card,
            card_exp_year = parts[1],
            card_exp_month = parts[0],
            card_cvv = cardCvv,
        });
        
        var data = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(jsonString));
        var combinedString = $"{privateKey}{data}{privateKey}";

        string signature;
        using (SHA1 sha1 = SHA1.Create())
        {
            var hashBytes = sha1.ComputeHash(
                Encoding.UTF8.GetBytes(combinedString));
            signature = Convert.ToBase64String(hashBytes);
        }
        
        await SendLinqPayRequest(data, signature);
    }

    private async Task SendLinqPayRequest(string data, string signature)
    {
        using HttpClient client = new HttpClient();
        
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("data", data),
            new KeyValuePair<string, string>("signature", signature)
        ]);
        
        HttpResponseMessage response = await client
            .PostAsync(LiqPayUrl, content);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response
                .Content.ReadAsStringAsync();
            string successString = "\"status\":\"success\"";

            if (!responseBody.Contains(successString))
            {
                throw new PaymentFailed();
            }
        }
        else
        {
            throw new PaymentApiRequestFailed();
        }
    }
}