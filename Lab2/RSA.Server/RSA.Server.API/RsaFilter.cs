using System.Text.Json;
using RSA.Server.API;

public class RsaFilter : IEndpointFilter
{
    private readonly RsaUtility _rsaUtility;
    private readonly ClientKeyStore _clientKeyStore;

    public RsaFilter(RsaUtility rsaUtility, ClientKeyStore clientKeyStore)
    {
        _rsaUtility = rsaUtility;
        _clientKeyStore = clientKeyStore;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        if (httpContext.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(httpContext.Request.Body);
            var encryptedBody = await reader.ReadToEndAsync();
            var decryptedBody = _rsaUtility.Decrypt(encryptedBody);
            httpContext.Items["DecryptedBody"] = JsonSerializer.Deserialize<object>(decryptedBody);
        }

        var result = await next(context);

        if (result is IResult resultObj)
        {
            var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(clientIp))
            {
                return Results.BadRequest("Client public key not found.");
            }
            
            var clientPublicKey = _clientKeyStore.GetPublicKey(clientIp);
            if (string.IsNullOrWhiteSpace(clientPublicKey))
            {
                return Results.BadRequest("Client public key not found.");
            }

            var responseJson = JsonSerializer.Serialize(resultObj);
            var encryptedResponse = _rsaUtility.Encrypt(responseJson, clientPublicKey);
            return Results.Text(encryptedResponse, "text/plain");
        }

        return result;
    }
}