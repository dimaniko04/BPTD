using System.Net;
using FundRaising.Server.BLL.Exceptions.Auth;

namespace FundRaising.Server.BLL.Exceptions.Fundraiser;

public class PaymentApiRequestFailed: Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
    public string ErrorMessage => "LiqPay payment request unexpectedly failed";
}