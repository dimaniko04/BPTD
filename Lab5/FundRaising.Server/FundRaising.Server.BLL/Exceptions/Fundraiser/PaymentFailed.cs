using System.Net;
using FundRaising.Server.BLL.Exceptions.Auth;

namespace FundRaising.Server.BLL.Exceptions.Fundraiser;

public class PaymentFailed: Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public string ErrorMessage => "Payment failed. Please try again.";
}