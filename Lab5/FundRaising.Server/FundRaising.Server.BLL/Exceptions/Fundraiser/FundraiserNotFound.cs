using System.Net;
using FundRaising.Server.BLL.Exceptions.Auth;

namespace FundRaising.Server.BLL.Exceptions.Fundraiser;

public class FundraiserNotFound: Exception, IServiceException
{
    private readonly Guid _id;
    
    public FundraiserNotFound(Guid id)
    {
        _id = id;
    }
    
    public HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    public string ErrorMessage => $"Fundraiser with id {_id} not found";
}