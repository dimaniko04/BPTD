using System.Net;

namespace FundRaising.Server.BLL.Exceptions.Auth;

public interface IServiceException
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage { get; }
}