namespace HASH.Server.API.Requests;

public record VerifyTextDigestRequest(
    string Text, 
    string Digest, 
    int BitSize);