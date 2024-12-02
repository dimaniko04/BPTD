namespace HASH.Server.API.Requests;

public record HashFileRequest(IFormFile File, int BitSize);