namespace RSA.Server.API.Response;

public record EncryptedFileResponse(
    string EncryptedContent, 
    string AesKey,
    string Iv);