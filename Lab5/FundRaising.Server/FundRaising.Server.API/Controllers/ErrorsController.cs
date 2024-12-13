using FundRaising.Server.BLL.Exceptions;
using FundRaising.Server.BLL.Exceptions.Auth;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FundRaising.Server.API.Controllers;

public class ErrorsController: ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        Exception? exception = HttpContext.Features
            .Get<IExceptionHandlerFeature>()?.Error;

        var (statusCode, message) = exception switch
        {
            IServiceException e => ((int)e.StatusCode, e.ErrorMessage),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occured"),
        };
        
        return Problem(statusCode: statusCode, title: message);
    }
}