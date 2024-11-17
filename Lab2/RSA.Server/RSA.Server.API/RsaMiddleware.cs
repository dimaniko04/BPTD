using System.Text;

namespace RSA.Server.API;

public class RsaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RsaUtility _rsaUtility;
    private readonly ClientKeyStore _clientKeyStore;

    public RsaMiddleware(
        RequestDelegate next, 
        RsaUtility rsaUtility, 
        ClientKeyStore clientKeyStore)
    {
        _next = next;
        _rsaUtility = rsaUtility;
        _clientKeyStore = clientKeyStore;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var encryptedRequestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrEmpty(encryptedRequestBody))
            {
                try
                {
                    var decryptedRequestBody = _rsaUtility.Decrypt(encryptedRequestBody);
                    var bytes = Encoding.UTF8.GetBytes(decryptedRequestBody);

                    context.Request.Body = new MemoryStream(bytes);
                    context.Request.ContentLength = bytes.Length;
                }
                catch
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid encrypted data");
                    return;
                }
            }
        }

        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        context.Response.Body = originalBodyStream;

        if (context.Response.ContentType is not null &&
            (context.Response.ContentType.StartsWith("application/json") ||
            context.Response.ContentType.StartsWith("text/plain")))
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(clientIp))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Unable to retrieve client IP");
                return;
            }

            var clientPublicKey = _clientKeyStore.GetPublicKey(clientIp);
            if (string.IsNullOrWhiteSpace(clientPublicKey))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Unable to retrieve client public key");
                return;
            }

            var encryptedResponse = _rsaUtility.Encrypt(responseBody, clientPublicKey);

            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(encryptedResponse);
        }
    }
}