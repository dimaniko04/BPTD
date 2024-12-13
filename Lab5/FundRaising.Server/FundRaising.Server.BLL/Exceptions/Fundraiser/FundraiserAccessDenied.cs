using System.Net;
using FundRaising.Server.BLL.Exceptions.Auth;

namespace FundRaising.Server.BLL.Exceptions.Fundraiser;

public class FundraiserAccessDenied: Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
    public string ErrorMessage => "You don't have access to this fundraiser";
}