using System.Security.Cryptography;
using System.Text;
using RSA.Server.API;
using RSA.Server.API.Requests;
using RSA.Server.API.Response;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ClientKeyStore>();
builder.Services.AddSingleton<RsaUtility>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowAnyOrigin();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.MapPost("/key/send", (
    SendKeyRequest request, 
    HttpContext context,
    ClientKeyStore clientKeyStore) =>
{
    var key = request.Key;
    
    if (string.IsNullOrEmpty(key))
    {
        return Results.BadRequest("Invalid public key");
    }

    var clientIp = context.Connection.RemoteIpAddress?.ToString();
    if (string.IsNullOrEmpty(clientIp))
    {
        return Results.BadRequest("Unable to determine client IP");
    }

    clientKeyStore.AddOrUpdate(clientIp, key);

    return Results.Ok("Client public key received");
});

app.MapGet("/key/get", (RsaUtility rsaUtility) => rsaUtility.GetPublicKey());

app.MapGet("/files", () =>
{
    var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files");
    var files = Directory.GetFiles(directory).Select(Path.GetFileName);
    return Results.Ok(files);
});

app.MapPost("/download", async (
    FileRequest request,
    HttpContext context,
    RsaUtility rsaUtility,
    ClientKeyStore clientKeyStore) =>
{
    var fileName = rsaUtility.Decrypt(request.EncryptedFileName);
    var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files");

    var filePath = Path.Combine(directory, fileName);

    if (!File.Exists(filePath))
    {
        return Results.NotFound("File not found.");
    }

    var fileBytes = await File.ReadAllBytesAsync(filePath);
    var rsaKey = GetClientRsaKey(context, clientKeyStore);

    if (String.IsNullOrEmpty(rsaKey))
    {
        return Results.BadRequest("Client public key not found.");
    }
    var (encryptedData,
        encryptedAesKey,
        encryptedIv) = HybridEncryptionUtility.Encrypt(fileBytes, rsaKey);
    
    var response = new EncryptedFileResponse(
        Convert.ToBase64String(encryptedData), 
        Convert.ToBase64String(encryptedAesKey), 
        Convert.ToBase64String(encryptedIv));
    
    return Results.Ok(response);
});

string? GetClientRsaKey(HttpContext context, ClientKeyStore clientKeyStore)
{
    var clientIp = context.Connection.RemoteIpAddress?.ToString();
    if (string.IsNullOrWhiteSpace(clientIp))
    {
        return null;
    }
            
    var clientPublicKey = clientKeyStore.GetPublicKey(clientIp);
    if (string.IsNullOrWhiteSpace(clientPublicKey))
    {
        return null;
    }
    
    return clientPublicKey;
}

app.Run();