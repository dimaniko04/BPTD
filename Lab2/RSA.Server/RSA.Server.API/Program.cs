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

/*app.MapPost("/download/{fileName}", async (
    string fileName,
    PrivateKeyRequest request,
    HttpContext context,
    RsaUtility rsaUtility,
    ClientKeyStore clientKeyStore) =>
{
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

    Decrypt(Convert.ToBase64String(encryptedData),
        Convert.ToBase64String(encryptedAesKey),
        Convert.ToBase64String(encryptedIv),
        request.Key);
    
    return Results.Ok(response);
});*/

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

/*string Decrypt(
    string encryptedData,
    string encryptedAesKey, 
    string encryptedIv,
    string privateKey)
{
    var privateKeyBytes = Convert.FromBase64String(privateKey);
        
    using var rsa = System.Security.Cryptography.RSA.Create();
    rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
    
    var key = rsa.Decrypt(Convert.FromBase64String(encryptedAesKey), RSAEncryptionPadding.Pkcs1);
    var iv = rsa.Decrypt(Convert.FromBase64String(encryptedIv), RSAEncryptionPadding.Pkcs1);
    
    using var aes = Aes.Create();
    aes.Key = key;
    aes.IV = iv;

    using var decryptor = aes.CreateDecryptor();
    using var memoryStream = new MemoryStream();
    using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

    var data = Convert.FromBase64String(encryptedData);
    
    cryptoStream.Write(data, 0, data.Length);
    cryptoStream.FlushFinalBlock();

    return Encoding.UTF8.GetString(memoryStream.ToArray());
}*/

app.Run();