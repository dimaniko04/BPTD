namespace HASH.Server.API.Requests;

public record GenerateCollisionRequest(IFormFile File, int BitSize);