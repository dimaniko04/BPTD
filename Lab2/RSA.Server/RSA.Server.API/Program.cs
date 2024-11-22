using RSA.Server.API;

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
    return Results.Ok(files);
}).AddEndpointFilter<RsaFilter>();

app.MapGet("/download/{fileName}", async (string fileName) =>
{
    var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files");

    var filePath = Path.Combine(directory, fileName);

    if (!File.Exists(filePath))
    {
        return Results.NotFound("File not found.");
    }

    var memory = new MemoryStream();
    await using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    {
        await stream.CopyToAsync(memory);
    }

    memory.Position = 0;

    const string contentType = "application/octet-stream";
    var fileDownloadName = Path.GetFileName(filePath);

    return Results.File(memory, contentType, fileDownloadName);
}).AddEndpointFilter<RsaFilter>();

app.Run();