namespace HASH.Server.API.Requests;

public record VerifyFileDigestRequest(
    IFormFile File, 
    string Digest, 
    int BitSize);