using HASH.Server.API.Requests;
using HASH.Server.API.Util;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapPost("/hash/text", (HashTextRequest request) =>
{
    var text = request.Text;
    var bitSize = request.BitSize;

    if (string.IsNullOrEmpty(text))
    {
        return Results.BadRequest("Non empty text is required");
    }

    try
    {
        var hash= HashUtil.HashText(text, bitSize);
        var hashString = hash.ToString("B").PadLeft(bitSize, '0');
        
        return Results.Ok(hashString);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/verify/text", (VerifyTextDigestRequest request) =>
{
    var text = request.Text;
    var digestStr = request.Digest;
    var bitSize = request.BitSize;

    if (string.IsNullOrEmpty(text))
    {
        return Results.BadRequest("Text is required");
    }
    if (string.IsNullOrEmpty(digestStr))
    {
        return Results.BadRequest("Digest is required");
    }

    try
    {
        var digest = Convert.ToByte(digestStr, 2);
        var isValid = HashUtil.VerifyTextDigest(text, digest, bitSize);
        
        return Results.Ok(isValid);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/hash/file", async ([FromForm] HashFileRequest request) =>
{
    var file = request.File;
    var bitSize = request.BitSize;

    try
    {
        var hash = await HashUtil.HashFile(file, bitSize);
        var hashString = hash.ToString("B").PadLeft(bitSize, '0');
        
        return Results.Ok(hashString);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).DisableAntiforgery();

app.MapPost("/verify/file", async (
    [FromForm] VerifyFileDigestRequest request) =>
{
    var file = request.File;
    var digestStr = request.Digest;
    var bitSize = request.BitSize;

    if (string.IsNullOrEmpty(digestStr))
    {
        return Results.BadRequest("Digest is required");
    }

    try
    {
        var digest = Convert.ToByte(digestStr, 2);
        var isValid = await HashUtil.VerifyFileDigest(file, digest, bitSize);

        return Results.Ok(isValid);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).DisableAntiforgery();

app.MapPost("/collision/file", async (
    [FromForm] GenerateCollisionRequest request) =>
{
    var file = request.File;
    var bitSize = request.BitSize;

    try
    {
        var newFileBytes = await HashUtil
            .GenerateFileCollision(file, bitSize);

        if (newFileBytes == null)
        {
            return Results.BadRequest("Failed to generate collision for file");
        }
        
        return Results.File(
            newFileBytes,
            file.ContentType,
            "encrypted_" + file.FileName);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).DisableAntiforgery();

app.Run();

