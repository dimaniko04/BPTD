using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RSA.Server.API;

public class RsaActionFilter : IAsyncActionFilter
{
    private readonly RsaUtility _rsaUtility;
    private readonly ClientKeyStore _clientKeyStore;

    public RsaActionFilter(RsaUtility rsaUtility, ClientKeyStore clientKeyStore)
    {
        _rsaUtility = rsaUtility;
        _clientKeyStore = clientKeyStore;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        if (httpContext.Request.ContentLength > 0)
        {
            httpContext.Request.EnableBuffering();

            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            var encryptedRequestBody = await reader.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;

            if (!string.IsNullOrEmpty(encryptedRequestBody))
            {
                try
                {
                    var decryptedRequestBody = _rsaUtility.Decrypt(encryptedRequestBody);
                    var bytes = Encoding.UTF8.GetBytes(decryptedRequestBody);

                    httpContext.Request.Body = new MemoryStream(bytes);
                    httpContext.Request.ContentLength = bytes.Length;
                }
                catch
                {
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid encrypted data");
                    return;
                }
            }
        }

        // Proceed to the next action in the pipeline
        var executedContext = await next();

        // Encrypt response if needed
        if (httpContext.Response.ContentType is not null &&
            (httpContext.Response.ContentType.StartsWith("application/json") ||
            httpContext.Response.ContentType.StartsWith("text/plain")))
        {
            var responseBodyStream = executedContext.HttpContext.Response.Body;

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(clientIp))
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Unable to retrieve client IP");
                return;
            }

            var clientPublicKey = _clientKeyStore.GetPublicKey(clientIp);
            if (string.IsNullOrWhiteSpace(clientPublicKey))
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Unable to retrieve client public key");
                return;
            }

            var encryptedResponse = _rsaUtility.Encrypt(responseBody, clientPublicKey);

            httpContext.Response.ContentType = "text/plain";
            await httpContext.Response.WriteAsync(encryptedResponse);
        }
    }
}