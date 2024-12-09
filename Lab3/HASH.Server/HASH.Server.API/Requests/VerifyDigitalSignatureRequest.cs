namespace HASH.Server.API.Requests;

public record VerifyDigitalSignatureRequest(
    string Digest,
    string Signature,
    string PublicKey);