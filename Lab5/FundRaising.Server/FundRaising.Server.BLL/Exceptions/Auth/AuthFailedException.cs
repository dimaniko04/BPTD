using System.Net;

namespace FundRaising.Server.BLL.Exceptions.Auth;

public class AuthFailedException: Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public string ErrorMessage => "User with given email and password does not exist";
}