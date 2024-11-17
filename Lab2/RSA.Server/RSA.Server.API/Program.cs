using RSA.Server.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ClientKeyStore>();
builder.Services.AddSingleton<RsaUtility>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/key/send", (
    string clientPublicKey, 
    HttpContext context,
    ClientKeyStore clientKeyStore) =>
{
    if (string.IsNullOrEmpty(clientPublicKey))
    {
        return Results.BadRequest("Invalid public key");
    }

    var clientIp = context.Connection.RemoteIpAddress?.ToString();
    if (string.IsNullOrEmpty(clientIp))
    {
        return Results.BadRequest("Unable to determine client IP");
    }

    clientKeyStore.AddOrUpdate(clientIp, clientPublicKey);

    return Results.Ok("Client public key received");
});

app.MapGet("/key/get", (RsaUtility rsaUtility) => rsaUtility.GetPublicKey());

app.MapGet("/files", () =>
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        var files = Directory.GetFiles(directory).Select(Path.GetFileName);
        return files;
    })
.WithName("Get Files")
.WithOpenApi();

app.Run();