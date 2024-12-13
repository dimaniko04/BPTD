using System.Net;

namespace FundRaising.Server.BLL.Exceptions.Auth;

public class DuplicateEmailException: Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public string ErrorMessage => "User with given email already exists";
}