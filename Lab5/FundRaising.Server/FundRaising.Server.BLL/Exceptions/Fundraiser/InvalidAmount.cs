using System.Net;
using FundRaising.Server.BLL.Exceptions.Auth;

namespace FundRaising.Server.BLL.Exceptions.Fundraiser;

public class InvalidAmount: Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public string ErrorMessage => "Invalid amount";
}